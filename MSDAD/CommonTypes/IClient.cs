namespace CommonTypes
{
    public interface IClient
    {
        void ShareMeeting(Meeting meeting);
        void GossipShareMeeting(Meeting m);
        void Status();
    }
}
