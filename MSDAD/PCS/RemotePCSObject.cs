using CommonTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Remoting.Channels.Tcp;

namespace PCS
{
    class RemotePCSObject : MarshalByRefObject, IPCS
    {
        Dictionary<string, string> servers = new Dictionary<string, string>();

        public void Client(string username, string client_URL, string server_URL, string script_file)
        {
            Console.WriteLine($"starting client: {username} {client_URL} {server_URL} {script_file}");
            Process.Start(@"Client.exe", $"{username} {client_URL} {server_URL} {script_file}");
        }
        public void Server(string server_id, string server_url, int max_faults, int min_delay, int max_delay)
        {
            Console.WriteLine($"starting server: {server_id} {server_url} {max_faults} {min_delay} {max_delay}");
            Process.Start(@"Server.exe", $"{server_id} {server_url} {max_faults} {min_delay} {max_delay}");
            servers.Add(server_id, server_url);
        }
        public void AddRoom(string location, int capacity, string name)
        {
            foreach (string server_url in servers.Values)
            {
                ((IServer)Activator.GetObject(typeof(IServer), server_url)).AddRoom(location, capacity, name);
            }
        }
        public string Status()
        {
            return "";
        }

        public void Crash(string server_id)
        {
            if (!servers.TryGetValue(server_id, out string url)) return;
            ((IServer)Activator.GetObject(typeof(IServer), url)).Crash();
        }

        public void Freeze(string server_id)
        {
            if (!servers.TryGetValue(server_id, out string url)) return;
            ((IServer)Activator.GetObject(typeof(IServer), url)).Freeze();
        }

        public void Unfreeze(string server_id)
        {
            if (!servers.TryGetValue(server_id, out string url)) return;
            ((IServer)Activator.GetObject(typeof(IServer), url)).Unfreeze();
        }

        public string[] GetServers()
        {
            string[] ret = new string[servers.Keys.Count];
            servers.Keys.CopyTo(ret, 0);
            return ret;
        }

        public void Unfreeze(string server_id)
        {
            string url;
            if (!servers.TryGetValue(server_id, out url)) return;
            TcpChannel channel = new TcpChannel();
            System.Runtime.Remoting.Channels.ChannelServices.RegisterChannel(channel, false);
            IServer server = (IServer)Activator.GetObject(typeof(IServer), url);
            server.Unfreeze();
        }
    }
}
