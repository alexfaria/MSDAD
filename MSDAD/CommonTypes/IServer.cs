using System.Collections.Generic;

namespace CommonTypes
{
    public interface IServer
    {
        List<Meeting> GetMeetings(Dictionary<string, int> vector, List<Meeting> clientMeetings);

        void CreateMeeting(Dictionary<string, int> vector, Meeting m);

        void RBCreateMeeting(string sender_url, Dictionary<string, int> vector, Meeting m);

        void JoinMeeting(string user, Dictionary<string, int> vector, string meetingTopic, List<Slot> slots);

        void RBJoinMeeting(string sender_url, Dictionary<string, int> vector, string user, string meetingTopic, List<Slot> slots);

        void CloseMeeting(Dictionary<string, int> vector, string user, string meetingTopic);

        bool RBCloseMeeting(string sender_url, Dictionary<string, int> vector, Meeting meet);

        void RBCloseSequence(string topic, int seq);

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
        Dictionary<string, int> UpdateVectorClock(Dictionary<string, int> vector);
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
        int GetSequenceNumber(string topic);
        string GetAlternativeServer();
        void Ping();
    }
}
