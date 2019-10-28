using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using CommonTypes;

namespace Server
{
    class RemoteServerObject : MarshalByRefObject, IServer
    {
        List<IClient> clients = new List<IClient>();
        List<IServer> servers = new List<IServer>();

        List<Meeting> meetings = new List<Meeting>();
        List<Location> locations = new List<Location>();

        private int max_faults;
        private int max_delay;
        private int min_delay;
        private bool freezed;

        public RemoteServerObject(int max_faults, int max_delay, int min_delay, List<IServer> servers)
        {
            this.max_faults = max_faults;
            this.max_delay = max_delay;
            this.min_delay = min_delay;

            this.servers = servers;
        }
        public List<Meeting> GetMeetings()
        {
            Console.WriteLine("getMeetings()");
            return meetings;
        }
        public List<IClient> CreateMeeting(Meeting m)
        {
            if (!meetings.Contains(m))
            {
                meetings.Add(m);
                foreach (IServer s in servers)
                    s.CreateMeeting(m);
                return clients;
            }
            return null;
        }
        public void JoinMeeting(string user, string meetingTopic, Slot slot)
        {
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
            freezed = true;
        }
        public void Unfreeze()
        {
            freezed = false;
        }
        /*
         * Additional Commands
         */
        public void ShareClient(string client_url)
        {
            IClient client = (IClient)Activator.GetObject(typeof(IClient), client_url);
            clients.Add(client);
        }
    }
}
