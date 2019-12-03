namespace CommonTypes
{
    public interface IClient
    {
        public void ShareMeeting(Meeting meeting);
        void GossipShareMeeting(Meeting m);
        void Status();
    }
}
