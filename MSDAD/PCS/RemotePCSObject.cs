using CommonTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Remoting.Channels.Tcp;

namespace PCS
{
    class RemotePCSObject : MarshalByRefObject, IPCS
    {
        List<string> servers_urls = new List<string>();
        public void AddRoom(string location, int capacity, string name)
        {
            foreach (string server_url in servers_urls) 
            {
                ((IServer)Activator.GetObject(typeof(IServer), server_url)).AddRoom(location, capacity, name);
            }
        }

        public void Client(string username, string client_URL, string server_URL, string script_file)
        {
            Console.WriteLine($"starting client: {username} {client_URL} {server_URL} {script_file}");
            Process.Start(@"Client.exe", $"{username} {client_URL} {server_URL} {script_file}");
        }
        public void Server(string server_id, string server_url, int max_faults, int min_delay, int max_delay)
        {
            Console.WriteLine($"starting server: {server_id} {server_url} {max_faults} {min_delay} {max_delay}");
            Process.Start(@"Server.exe", $"{server_id} {server_url} {max_faults} {min_delay} {max_delay}");
            servers_urls.Add(server_url);
        }
        public string Status()
        {
            return "";
        }
    }
}
