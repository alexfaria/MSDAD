namespace CommonTypes
{
    public interface IClient
    {
        void ShareMeeting(Meeting meeting);
        void GossipShareMeeting(string senderUrl, Meeting m);
        void Status();
    }
}
