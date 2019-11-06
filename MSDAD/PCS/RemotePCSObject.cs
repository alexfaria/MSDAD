using CommonTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Remoting.Channels.Tcp;

namespace PCS
{
    class RemotePCSObject : MarshalByRefObject, IPCS
    {
        List<string> clients = new List<string>();
        Dictionary<string, string> servers = new Dictionary<string, string>();

        public void Client(string username, string client_URL, string server_URL, string script_file)
        {
            Console.WriteLine($"starting client: {username} {client_URL} {server_URL} {script_file}");
            Process.Start(@"Client.exe", $"{username} {client_URL} {server_URL} {script_file}");
            clients.Add(client_URL);
        }
        public void Server(string server_id, string server_url, int max_faults, int min_delay, int max_delay)
        {
            Console.WriteLine($"starting server: {server_id} {server_url} {max_faults} {min_delay} {max_delay}");
            Process.Start(@"Server.exe", $"{server_id} {server_url} {max_faults} {min_delay} {max_delay}");
            servers[server_id] = server_url;
        }
        public void AddRoom(string location, int capacity, string name)
        {
            foreach (string server_url in servers.Values)
            {
                ((IServer) Activator.GetObject(typeof(IServer), server_url)).AddRoom(location, capacity, name);
            }
        }
        public void Status()
        {
            foreach (string server_url in servers.Values)
            {
                ((IServer) Activator.GetObject(typeof(IServer), server_url)).Status();
            }
            foreach (string client_url in clients)
                ((IClient) Activator.GetObject(typeof(IClient), client_url)).Status();
        }

        public void Crash(string server_id)
        {
            if (!servers.TryGetValue(server_id, out string url))
                return;
            try
            {

                servers.Remove(server_id);
                ((IServer) Activator.GetObject(typeof(IServer), url)).Crash();
            }
            catch (Exception) { }
        }

        public void Freeze(string server_id)
        {
            if (!servers.TryGetValue(server_id, out string url))
                return;
            ((IServer) Activator.GetObject(typeof(IServer), url)).Freeze();
        }

        public void Unfreeze(string server_id)
        {
            if (!servers.TryGetValue(server_id, out string url))
                return;
            ((IServer) Activator.GetObject(typeof(IServer), url)).Unfreeze();
        }

        public string[] GetServers()
        {
            string[] ret = new string[servers.Keys.Count];
            servers.Keys.CopyTo(ret, 0);
            return ret;
        }
    }
}
