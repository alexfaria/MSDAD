using System.Collections.Generic;

namespace CommonTypes
{
    public interface IServer
    {
        void WriteLine(string message);
        List<Meeting> GetMeetings();

        void CreateMeeting(Meeting m);

        void JoinMeeting(string user, string meetingTopic, Slot slot);

        void CloseMeeting(string user, string meetingTopic);

        void AddRoom(string location, int capacity, string room_name);
    }
}
