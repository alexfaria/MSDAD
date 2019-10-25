using CommonTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PCS
{
    class RemotePCSObject : MarshalByRefObject, IPCS
    {
        List<IServer> serverList;
        List<IClient> clientList;
        public RemotePCSObject()
        {
            clientList = new List<IClient>();
            serverList = new List<IServer>();            
        }
        public void Client(string username, string client_URL, string server_URL, string script_file)
        {
            Console.WriteLine($"starting client: {username} {client_URL} {server_URL} {script_file}");
            Process.Start(@"Client.exe", $"{username} {client_URL} {server_URL} {script_file}");
        }
        public void Server(string server_id, string URL, int max_faults, int min_delay, int max_delay)
        {
            Console.WriteLine($"starting server: {server_id} {URL} {max_faults} {min_delay} {max_delay}");
            Process.Start(@"Server.exe", $"{server_id} {URL} {max_faults} {min_delay} {max_delay}");
        }
        public void AddRoom(string location, int capacity, string room_name)
        {
            foreach (IServer server in serverList)
            {
                server.AddRoom(location, capacity, room_name);
            }
        }
    }
}
