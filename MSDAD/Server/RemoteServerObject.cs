using System;
using System.Collections.Generic;
using CommonTypes;

namespace Server
{
    class RemoteServerObject : MarshalByRefObject, IServer
    {
        List<Meeting> meetings = new List<Meeting>();
        public void WriteLine(string message)
        {
            Console.WriteLine(message);
        }

        public List<Meeting> GetMeetings()
        {
            return meetings;
        }

        public void CreateMeeting(Meeting m)
        {
            bool exist = false;
            foreach (Meeting meeting in meetings)
                if (m.id.Equals(meeting.id))
                    exist = true;
            if (!exist) meetings.Add(m);
        }

        public void JoinMeeting(string user, string meetingTopic, Slot slot)
        {
            Meeting meeting = meetings.Find((m1) => m1.topic.Equals(meetingTopic));
            if (meeting != null)
            {
                meeting.AddParticipant(user, slot);
            } else
            {
                // Try to sync state asking for the meeting in other servers
            }
        }

        public void CloseMeeting(string user, string meetingTopic)
        {
            Meeting meeting = meetings.Find((m1) => m1.topic.Equals(meetingTopic));
            // ToDo
        }
    }
}
