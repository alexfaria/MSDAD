namespace CommonTypes
{
    public interface IPCS
    {
        void Server(string server_id, string URL, int max_faults, int min_delay, int max_delay);
        void Client(string username, string client_URL, string server_URL, string script_file);
        void AddRoom(string location, int capacity, string name);
        void Status();
        string[] GetServers();

        // Debugging commands
        void Crash(string server_id);
        void Freeze(string server_id);
        void Unfreeze(string server_id);
    }
}
