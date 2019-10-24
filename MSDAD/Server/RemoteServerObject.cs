using System;
using System.Collections.Generic;
using CommonTypes;

namespace Server
{
    class RemoteServerObject : MarshalByRefObject, IServer
    {
        List<Meeting> meetings = new List<Meeting>();
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
            bool exist = false;
            foreach (Meeting meeting in meetings)
                if (m.Equals(meeting))
                    exist = true;
            if (!exist) meetings.Add(m);
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
            // ToDo
        }

        public void AddRoom(string location, int capacity, string room_name)
        {
            throw new NotImplementedException();
        }
    }
}
