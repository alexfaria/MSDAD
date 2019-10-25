using System;
using System.Collections.Generic;
using CommonTypes;

namespace Server
{
    class RemoteServerObject : MarshalByRefObject, IServer
    {
        List<Meeting> meetings = new List<Meeting>();
        List<Location> locations = new List<Location>();
        private int max_faults;
        private int max_delay;
        private int min_delay;

        public RemoteServerObject(int max_faults, int max_delay, int min_delay)
        {
            this.max_faults = max_faults;
            this.max_delay = max_delay;
            this.min_delay = min_delay;
        }

        public void WriteLine(string message)
        {
            Console.WriteLine(message);
        }

        public List<Meeting> GetMeetings()
        {
            Console.WriteLine("getMeetings()");
            return meetings;
        }

        public void CreateMeeting(Meeting m)
        {
            if (!meetings.Contains(m))
            {
                meetings.Add(m);
            }
        }

        public void JoinMeeting(string user, string meetingTopic, Slot slot)
        {
            Meeting meeting = meetings.Find((m1) => m1.topic.Equals(meetingTopic));
            if (meeting != null)
            {
                meeting.AddParticipant(user, slot);
            }
            else
            {
                // Try to sync state asking for the meeting in other servers
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
    }
}
