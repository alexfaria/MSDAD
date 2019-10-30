using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using CommonTypes;

namespace Server
{
    class RemoteServerObject : MarshalByRefObject, IServer
    {
        Dictionary<string, string> clients;
        List<IServer> servers;

        List<Meeting> meetings;
        List<Location> locations;

        private int max_faults;
        private int max_delay;
        private int min_delay;

        private bool frozen;
        DateTime delayUntil;

        public RemoteServerObject(int max_faults, int max_delay, int min_delay, List<IServer> servers)
        {
            this.max_faults = max_faults;
            this.max_delay = max_delay;
            this.min_delay = min_delay;
            this.servers = servers;

            servers = new List<IServer>();
            meetings = new List<Meeting>();
            locations = new List<Location>();
            clients = new Dictionary<string, string>();
        }

        private void DelayMessageHandling()
        {
            //TODO: thread sync
            Random rnd = new Random();
            int delay = rnd.Next(min_delay, max_delay);
            delayUntil = DateTime.Now.AddMilliseconds(delay);
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
            DelayMessageHandling();
            Console.WriteLine("getMeetings()");
            return meetings.FindAll(m => clientMeetings.Exists(m2 => m.topic.Equals(m2.topic)));
        }
        public void CreateMeeting(Meeting m)
        {
            DelayMessageHandling();
            if (!meetings.Contains(m))
            {
                meetings.Add(m);
                foreach (IServer s in servers)
                    s.CreateMeeting(m);
            }
        }
        public void JoinMeeting(string user, string meetingTopic, Slot slot)
        {
            DelayMessageHandling();
            Meeting meeting = meetings.Find((m1) => m1.topic.Equals(meetingTopic));
            if (meeting != null)
            {
                meeting.AddParticipant(user, slot);
                foreach (IServer s in servers)
                    s.JoinMeeting(user, meetingTopic, slot);
            }
        }
        public void CloseMeeting(string user, string meetingTopic)
        {

            DelayMessageHandling();
            Meeting meeting = meetings.Find((m1) => m1.topic.Equals(meetingTopic));
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
                // meeting.Cancel();
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
    }
}
