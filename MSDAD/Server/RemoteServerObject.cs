using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.Remoting;
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
            clients = new Dictionary<string, string>();
        }

        private void MessageHandler()
        {
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
                    // Replicate the operation
                    // TODO: reliable broadcast?
                    foreach (string server_url in servers_urls)
                    {
                        try
                        {
                            ((IServer) Activator.GetObject(typeof(IServer), server_url)).RegisterClient(username, client_url);
                        }
                        catch (SocketException e)
                        {
                            Console.WriteLine($"[{e.GetType().Name}] Error trying to contact <{server_url}>");
                        }
                    }
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
                    // Replicate the operation
                    // TODO: reliable broadcast?
                    foreach (string server_url in servers_urls)
                    {
                        try
                        {
                            ((IServer) Activator.GetObject(typeof(IServer), server_url)).UnregisterClient(username);
                        }
                        catch (SocketException e)
                        {
                            Console.WriteLine($"[{e.GetType().Name}] Error trying to contact <{server_url}>");
                        }
                    }
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

                //TODO: reliable brodcast
                // Replicate the operation
                ThreadPool.QueueUserWorkItem(state =>
                {
                    foreach (string server_url in servers_urls)
                    {
                        try
                        {
                            ((IServer) Activator.GetObject(typeof(IServer), server_url)).CreateMeeting(m);
                        }
                        catch (SocketException e)
                        {
                            Console.WriteLine($"[{e.GetType().Name}] Error trying to contact <{server_url}>");
                        }
                    }
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
            if (meeting.status > CommonTypes.Status.Open)
            {
                Monitor.Exit(meeting);
                throw new ApplicationException($"The meeting {meetingTopic} is either closing or closed/cancelled.");
            }
            bool joined = meeting.Join(user, slots);
            Monitor.Exit(meeting);
            if (joined)
            {
                List<EventWaitHandle> handles = new List<EventWaitHandle>(this.servers_urls.Count);
                for (int i = 0; i < servers_urls.Count; i++) // Replicate the operation
                {
                    Thread task = new Thread(() =>
                    {
                        ((IServer) Activator.GetObject(typeof(IServer), servers_urls[i])).RBJoinMeeting(server_url, user, meetingTopic, slots);
                        handles[i].Set();
                    });
                }

                for (int i = 0; i < handles.Count/* - max_faults */; i++) // Wait for the responses
                {
                    int idx = WaitHandle.WaitAny(handles.ToArray());
                    handles.RemoveAt(idx);
                }
            }
            Monitor.Exit(meeting);
        }
        public void RBJoinMeeting(string sender_url, string user, string meetingTopic, List<Slot> slots)
        {
            Meeting meeting = meetings.Find((m1) => m1.topic.Equals(meetingTopic));
            Monitor.Enter(meeting);
            if (meeting.status == CommonTypes.Status.Closed) // ??? I don't think it can happen ???
            {
                Monitor.Exit(meeting);
                return;
            }
            bool joined = meeting.Join(user, slots);
            Monitor.Exit(meeting);
            if (joined)
            {
                EventWaitHandle[] handles = new EventWaitHandle[this.servers_urls.Count];
                for (int i = 0; i < servers_urls.Count; i++)
                {
                    if (servers_urls[i] != sender_url)
                    {
                        Thread task = new Thread(() =>
                        {
                            ((IServer) Activator.GetObject(typeof(IServer), servers_urls[i])).RBJoinMeeting(server_url, user, meetingTopic, slots);
                            handles[i].Set();
                        });
                    }
                }
                for (int i = 0; i < handles.Length/* - max_faults */; i++) // Wait for the responses
                {
                    WaitHandle.WaitAny(handles);
                }
            }
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
            List<Room> locked = null;
            foreach (Slot s in meeting.slots) // Find the best slot for the meeting
            {
                Location location = locations.Find(l => l.name.Equals(s.location));
                Monitor.Enter(location.rooms); // Lock the rooms on the current location
                List<Room> free = location.rooms.FindAll(r => !r.booked.Contains(s.date));
                if (free.Count > 0) // There is a free room
                    if (slot == null || slot != null && s.participants.Count >= meeting.min_participants && s.participants.Count > slot.participants.Count)
                    {
                        if (locked != null) Monitor.Exit(locked); // Release the lock of previous location rooms locked
                        locked = location.rooms;
                        slot = s;
                        rooms = free;
                    }
                if (rooms != free) Monitor.Exit(location.rooms); // Unlock if the rooms of the current location were not selected
            }
            if (slot == null)
            {
                meeting.status = CommonTypes.Status.Cancelled;
            } else
            {
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
                Monitor.Exit(locked);
                if (room.capacity < slot.participants.Count)
                    // If there are more registered participants than the capacity of the selected meeting room
                    slot.participants.RemoveRange(room.capacity - 1, slot.participants.Count - room.capacity);

                meeting.status = CommonTypes.Status.Closing;
                meeting.room = room;
                meeting.slot = slot;
            }
            List<EventWaitHandle> handles = new List<EventWaitHandle>(this.servers_urls.Count);
            bool[] taskResults = new bool[this.servers_urls.Count];
            for (int i = 0; i < servers_urls.Count; i++) // Replicate the operation
            {
                Thread task = new Thread(() =>
                {
                    taskResults[i] = ((IServer) Activator.GetObject(typeof(IServer), servers_urls[i])).RBCloseMeeting(server_url, meeting);
                    handles[i].Set();
                });
            }
            Monitor.Exit(meeting);
            bool success = true;
            for (int i = 0; i < handles.Count/* - max_faults */; i++) // Wait for the responses
            {
                int idx = WaitHandle.WaitAny(handles.ToArray());
                handles.RemoveAt(idx);
                if (!taskResults[idx])
                {
                    success = false;
                    break;
                }
            }
            Monitor.Enter(meeting);
            if (success)
            {
                meeting.status = meeting.status == CommonTypes.Status.Closing ? CommonTypes.Status.Closed : CommonTypes.Status.Cancelled;
            }
            else
            {
                meeting.room.booked.Remove(meeting.slot.date);
                meeting.room = null;
                meeting.slot = null;
                meeting.status = CommonTypes.Status.Cancelled;
            }
            Monitor.Exit(meeting);
        }
        public bool RBCloseMeeting(string sender_url, Meeting meet)
        {
            Meeting meeting = meetings.Find((m1) => m1.topic.Equals(meet.topic));
            Monitor.Enter(meeting);
            if (meeting.status == CommonTypes.Status.Closing || meeting.status == CommonTypes.Status.Closed)
            {
                Monitor.Exit(meeting);
                return true;
            }
            Location location = locations.Find(l => l.name.Equals(meet.slot.location));
            Room room = location.rooms.Find(r => r.name.Equals(meet.room.name));
            Monitor.Enter(room);
            if (!room.booked.Contains(meet.slot.date))
            {
                Monitor.Exit(room);
                Monitor.Exit(meeting);
                return false;
            }
            room.booked.Add(meet.slot.date);
            meeting.status = CommonTypes.Status.Closing;
            Monitor.Exit(meeting);
            List<EventWaitHandle> handles = new List<EventWaitHandle>(this.servers_urls.Count);
            bool[] taskResults = new bool[this.servers_urls.Count];
            for (int i = 0; i < servers_urls.Count; i++)
            {
                if (servers_urls[i] != sender_url)
                {
                    Thread task = new Thread(() =>
                    {
                        taskResults[i] = ((IServer) Activator.GetObject(typeof(IServer), servers_urls[i])).RBCloseMeeting(server_url, meet);
                        handles[i].Set();
                    });
                }
            }
            bool success = true;
            for (int i = 0; i < handles.Count/* - max_faults */; i++) // Wait for the responses
            {
                int idx = WaitHandle.WaitAny(handles.ToArray());
                handles.RemoveAt(idx);
                if (!taskResults[idx])
                {
                    success = false;
                    break;
                }
            }
            Monitor.Enter(meeting);
            if (success)
            {
                meeting.status = CommonTypes.Status.Closed;
            } else {
                room.booked.Remove(meet.slot.date);
                meeting.status = CommonTypes.Status.Cancelled;
            }
            Monitor.Exit(room);
            Monitor.Exit(meeting);
            return success;
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
