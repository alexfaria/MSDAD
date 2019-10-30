using System.Collections.Generic;

namespace CommonTypes
{
    public interface IServer
    {
        List<Meeting> GetMeetings(List<Meeting> clientMeetings);

        void CreateMeeting(Meeting m);

        void JoinMeeting(string user, string meetingTopic, Slot slot);

        void CloseMeeting(string user, string meetingTopic);

        void AddRoom(string location, int capacity, string room_name);

        void Status();
        /*
         * Debugging Commands
         */
        void Crash();
        void Freeze();
        void Unfreeze();
        /*
         * Additional Commands
         */
        Dictionary<string, string> GetClients();
        void RegisterClient(string username, string client_url);
    }
}
