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

        readonly private int max_faults;
        readonly private int max_delay;
        readonly private int min_delay;

        private bool frozen;
        private readonly List<KeyValuePair<MethodInfo, object[]>> pending;
        DateTime delayUntil;

        public RemoteServerObject(int max_faults, int max_delay, int min_delay, List<string> servers_urls)
        {
            this.max_faults = max_faults;
            this.max_delay = max_delay;
            this.min_delay = min_delay;
            this.servers_urls = servers_urls;

            meetings = new List<Meeting>();
            locations = new List<Location>();
            clients = new Dictionary<string, string>();
            pending = new List<KeyValuePair<MethodInfo, object[]>>();
        }

        private void MessageHandler()
        {
            //TODO: lidar com _frozen_
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
            clients[username] = client_url;
            Console.WriteLine($"Added client '{username}' at '{client_url}'");
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
                meetings.Add(m);
                foreach (string server_url in servers_urls) // Replicate the operation
                    ((IServer)Activator.GetObject(typeof(IServer), server_url)).CreateMeeting(m);
            }
        }
        public void JoinMeeting(string user, string meetingTopic, Slot slot)
        {
            MessageHandler();
            Meeting meeting = meetings.Find((m1) => m1.topic.Equals(meetingTopic));
            if (meeting == null || meeting.status == CommonTypes.Status.Closed) throw new InvalidMeetingException(meetingTopic);
            Slot sl = meeting.slots.Find((s) => s.Equals(slot));
            if (!sl.participants.Contains(user))
            {
                sl.participants.Add(user);
                foreach (string server_url in servers_urls) // Replicate the operation
                    ((IServer)Activator.GetObject(typeof(IServer), server_url)).JoinMeeting(user, meetingTopic, slot);
            }
        }
        public void CloseMeeting(string user, string meetingTopic)
        {
            MessageHandler();
            Meeting meeting = meetings.Find((m1) => m1.topic.Equals(meetingTopic));
            if (meeting == null)
                throw new InvalidMeetingException(meetingTopic);
            if (!user.Equals(meeting.coordinator))
                throw new UnauthorizedException(user);
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
                if (r.capacity >= slot.participants.Count)
                {
                    room = r;
                    break;
                }
                if (r.capacity > room.capacity)
                    room = r;
            }
            room.booked.Add(slot.date); // Book the room
            if (room.capacity < slot.participants.Count)
                // If there are more registered participants than the capacity of the selected meeting room
                slot.participants.RemoveRange(room.capacity - 1, slot.participants.Count - room.capacity);
            // The meeting is scheduled
            foreach (string server_url in servers_urls) // Replicate the operation
                ((IServer)Activator.GetObject(typeof(IServer), server_url)).CloseMeeting(user, meetingTopic);
            meeting.status = CommonTypes.Status.Closed;
        }
        public void AddRoom(string location, int capacity, string room_name)
        {
            Location loc = locations.Find(l => l.name.Equals(location));
            if (loc == null) return;
            Room room = new Room(room_name, capacity);
            if (!loc.rooms.Contains(room))
                loc.rooms.Add(room);
        }
        public void Status()
        {

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
            frozen = true;
        }
        public void Unfreeze()
        {
            frozen = false;
        }
        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}
