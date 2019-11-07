using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using CommonTypes;

namespace Server
{
    class RemoteServerObject : MarshalByRefObject, IServer
    {
        readonly Dictionary<string, string> clients;
        readonly List<string> servers_urls;

        readonly List<Meeting> meetings;
        readonly List<Location> locations;

        readonly private string server_url;
        readonly private int max_faults;
        readonly private int max_delay;
        readonly private int min_delay;

        private bool frozen;
        private int currentPosition;
        private int lastPosition;
        DateTime delayUntil;

        Dictionary<string, object> rb_locks; // Dictionary of Reliable Broadcast locks to avoid broadcast replication

        public RemoteServerObject(string server_url, int max_faults, int max_delay, int min_delay, List<string> servers_urls)
        {
            this.server_url = server_url;
            this.max_faults = max_faults;
            this.max_delay = max_delay;
            this.min_delay = min_delay;
            this.servers_urls = servers_urls;
            this.frozen = false;
            this.currentPosition = 0;
            this.lastPosition = 0;

            meetings = new List<Meeting>();
            locations = new List<Location>();
            //locations.Add(new Location("Lisboa", new List<Room> {
            //    new Room("Room A", 2),
            //    new Room("Room B", 4),
            //    new Room("Room C", 10)
            //}));
            clients = new Dictionary<string, string>();
        }

        private void MessageHandler()
        {
            //TODO: lidar com _frozen_
            lock (this)
            {
                if (frozen)
                {
                    int position = lastPosition++;
                    while (frozen || (!frozen && position != currentPosition))
                    {
                        Monitor.Wait(this);
                        if (!frozen && position == currentPosition)
                        {
                            ++currentPosition;
                            Monitor.PulseAll(this);
                            break;
                        }
                    }
                }
            }
            Random rnd = new Random();
            int delay = rnd.Next(min_delay, max_delay);

            if (DateTime.Now.AddMilliseconds(delay).CompareTo(delayUntil) < 0)
            {
                delay = DateTime.Now.Subtract(delayUntil).Milliseconds;
            }
            else
            {
                lock (this) //TODO: error lock(delayUntil) idk
                {
                    delayUntil = DateTime.Now.AddMilliseconds(delay);
                }
            }
            Thread.Sleep(delay);
        }

        public void RegisterClient(string username, string client_url)
        {
            if (!clients.ContainsKey(username))
            {
                clients[username] = client_url;
                Console.WriteLine($"Added client '{username}' at '{client_url}'");
                ThreadPool.QueueUserWorkItem(state =>
                {
                    foreach (string server_url in servers_urls) // Replicate the operation
                        ((IServer) Activator.GetObject(typeof(IServer), server_url)).RegisterClient(username, client_url);
                });
            }
        }
        public void UnregisterClient(string username)
        {
            if (clients.Remove(username))
            {
                Console.WriteLine($"Removed client '{username}'");
                ThreadPool.QueueUserWorkItem(state =>
                {
                    foreach (string server_url in servers_urls) // Replicate the operation
                        ((IServer) Activator.GetObject(typeof(IServer), server_url)).UnregisterClient(username);
                });
            }
        }
        public Dictionary<string, string> GetClients()
        {
            return this.clients;
        }

        public List<Meeting> GetMeetings(List<Meeting> clientMeetings)
        {
            MessageHandler();
            Console.WriteLine("GetMeetings()");
            return meetings.FindAll(m => clientMeetings.Exists(m2 => m.topic.Equals(m2.topic)));
        }
        public void CreateMeeting(Meeting m)
        {
            MessageHandler();
            if (!meetings.Contains(m))
            {
                foreach (Slot s in m.slots)
                    if (!locations.Exists(l => l.name.Equals(s.location)))
                        throw new ApplicationException($"The meeting {m.topic} has a slot with an unknown location {s.location}.");
                meetings.Add(m);
                ThreadPool.QueueUserWorkItem(state =>
                {
                    foreach (string server_url in servers_urls) // Replicate the operation
                        ((IServer) Activator.GetObject(typeof(IServer), server_url)).CreateMeeting(m);
                });
            }
        }
        public void JoinMeeting(string user, string meetingTopic, List<Slot> slots)
        {
            MessageHandler();
            Meeting meeting = meetings.Find((m1) => m1.topic.Equals(meetingTopic));
            if (meeting == null)
                throw new ApplicationException($"The meeting {meetingTopic} does not exist.");
            Monitor.Enter(meeting);
            if (meeting.status == CommonTypes.Status.Closed)
                throw new ApplicationException($"The meeting {meetingTopic} is closed.");
            bool addedParticipants = false;
            foreach (Slot s in meeting.slots.FindAll(s => slots.Contains(s)))
            {
                if (!s.participants.Contains(user))
                {
                    s.participants.Add(user);
                    addedParticipants = true;
                }
            }
            if (addedParticipants)
                ThreadPool.QueueUserWorkItem(state =>
                {
                    foreach (string server_url in servers_urls) // Replicate the operation
                        ((IServer) Activator.GetObject(typeof(IServer), server_url)).JoinMeeting(user, meetingTopic, slots);
                });
            Monitor.Exit(meeting);
        }
        public void CloseMeeting(string user, string meetingTopic)
        {
            MessageHandler();
            Meeting meeting = meetings.Find((m1) => m1.topic.Equals(meetingTopic));
            if (meeting == null)
                throw new ApplicationException($"The meeting {meetingTopic} do not exist.");
            if (!user.Equals(meeting.coordinator))
                throw new ApplicationException($"You are not authorized to close the meeting {meetingTopic}.");
            Monitor.Enter(meeting);
            Slot slot = null;
            List<Room> rooms = null;
            foreach (Slot s in meeting.slots) // Find the best slot for the meeting
            {
                Location location = locations.Find(l => l.name.Equals(s.location));
                List<Room> free = location.rooms.FindAll(r => !r.booked.Contains(s.date));
                if (free.Count > 0) // There is a free room
                    if (slot == null || slot != null && s.participants.Count >= meeting.min_participants && s.participants.Count > slot.participants.Count)
                    {
                        slot = s;
                        rooms = free;
                    }
            }
            if (slot == null)
            {
                meeting.status = CommonTypes.Status.Cancelled;
                return;
            }
            Room room = null;
            foreach (Room r in rooms) // Find the best room for the meeting
            {
                if (room == null || slot.participants.Count > room.capacity && room.capacity < r.capacity)
                {
                    room = r;
                    if (room.capacity == slot.participants.Count)
                        break;
                }
            }
            room.booked.Add(slot.date); // Book the room
            if (room.capacity < slot.participants.Count)
                // If there are more registered participants than the capacity of the selected meeting room
                slot.participants.RemoveRange(room.capacity - 1, slot.participants.Count - room.capacity);

            // The meeting is scheduled
            if (!rb_locks.ContainsKey(meetingTopic)) // Create lock to Reliable Broadcast this meeting
            {
                object lck = new object();
                rb_locks.Add(meetingTopic, lck);
            }

            Monitor.Enter(rb_locks[meetingTopic]);
            EventWaitHandle[] handles = new EventWaitHandle[this.servers_urls.Count];
            for (int i = 0; i < servers_urls.Count; i++) // Replicate the operation
            {
                Thread task = new Thread(() =>
                {
                    ((IServer) Activator.GetObject(typeof(IServer), servers_urls[i])).RBCloseMeeting(server_url, meeting);
                    handles[i].Set();
                });
            }

            for (int i = 0; i < handles.Length/* - max_faults */; i++) // Wait for the responses
            {
                WaitHandle.WaitAny(handles);
            }
            meeting.status = CommonTypes.Status.Closed;
            Monitor.Exit(rb_locks[meetingTopic]);
            Monitor.Exit(meeting);
        }
        public void RBCloseMeeting(string sender_url, Meeting meet)
        {
            if (!rb_locks.ContainsKey(meet.topic)) // Create lock to Reliable Broadcast this meeting
            {
                object lck = new object();
                rb_locks.Add(meet.topic, lck);
            }

            bool lock_taken = false;
            Monitor.TryEnter(rb_locks[meet.topic], ref lock_taken);
            if (lock_taken)
            {
                Meeting meeting = meetings.Find((m1) => m1.topic.Equals(meet.topic));
                if (meeting.status == CommonTypes.Status.Closed)
                    return;

                Monitor.Enter(meeting);
                EventWaitHandle[] handles = new EventWaitHandle[this.servers_urls.Count];

                for (int i = 0; i < servers_urls.Count; i++)
                {
                    if (servers_urls[i] != sender_url)
                    {
                        Thread task = new Thread(() =>
                        {
                            ((IServer) Activator.GetObject(typeof(IServer), servers_urls[i])).RBCloseMeeting(server_url, meet);
                            handles[i].Set();
                        });
                    }
                }

                for (int i = 0; i < handles.Length/* - max_faults */; i++) // Wait for the responses
                {
                    WaitHandle.WaitAny(handles);
                }
                meeting.status = CommonTypes.Status.Closed;
                Monitor.Exit(meeting);
                Monitor.Exit(rb_locks[meet.topic]);
            }
        }
        public void AddRoom(string location_name, int capacity, string room_name)
        {
            Location location = locations.Find(l => l.name.Equals(location_name));
            if (location == null)
            {
                location = new Location(location_name);
                locations.Add(location);
            }

            Room room = new Room(room_name, capacity);
            if (!location.rooms.Contains(room))
            {
                location.rooms.Add(room);
            }

            Console.WriteLine($"[AddRoom] Added room <{location_name},{room_name},{capacity}>");
        }
        public void Status()
        {
            Console.WriteLine("Clients:");
            foreach (string client in clients.Values)
            {
                Console.WriteLine($"\t{client}");
            }
            Console.WriteLine("Servers:");
            foreach (string server in servers_urls)
            {
                Console.WriteLine($"\t{server}");
            }
        }
        /*
         * Debuggings Commands
         */
        public void Crash()
        {
            Process.GetCurrentProcess().Kill();
        }
        public void Freeze()
        {
            lock (this)
            {
                frozen = true;
            }
        }
        public void Unfreeze()
        {
            lock (this)
            {
                frozen = false;
                Monitor.PulseAll(this);
            }
        }
        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}
