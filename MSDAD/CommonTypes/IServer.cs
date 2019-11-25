using System.Collections.Generic;

namespace CommonTypes
{
    public interface IServer
    {
        List<Meeting> GetMeetings(List<Meeting> clientMeetings);

        void CreateMeeting(Meeting m);
        void RBCreateMeeting(string sender_url, int seq, Meeting m);

        void JoinMeeting(string user, string meetingTopic, List<Slot> slots);

        void RBJoinMeeting(string sender_url, string user, string meetingTopic, List<Slot> slots);

        void CloseMeeting(string user, string meetingTopic);

        bool RBCloseMeeting(string sender_url, Meeting meet);

        void AddRoom(string location, int capacity, string room_name);

        void Status();
        /*
         * Debugging Commands
         */
        void Crash();
        void Freeze();
        void Unfreeze();
        /*
         * Client Management Commands
         */
        Dictionary<string, string> GetClients();
        void RegisterClient(string username, string client_url);
        void UnregisterClient(string username);
        /*
         * Leader Election
         */
        void Election();
        void Elected(string leader);
        /*
         * Sequence Commands
         */
        int GetSequenceNumber();
    }
}
