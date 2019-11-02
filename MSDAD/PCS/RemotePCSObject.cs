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
        public void AddRoom(string location, int capacity, string name)
        {
            foreach (KeyValuePair<string, string> pair in servers) 
            {
                TcpChannel channel = new TcpChannel();
                System.Runtime.Remoting.Channels.ChannelServices.RegisterChannel(channel, false);
                IServer server = (IServer)Activator.GetObject(typeof(IServer), pair.Value);
                server.AddRoom(location, capacity, name);
            }
        }

        public void Client(string username, string client_URL, string server_URL, string script_file)
        {
            Console.WriteLine($"starting client: {username} {client_URL} {server_URL} {script_file}");
            Process.Start(@"Client.exe", $"{username} {client_URL} {server_URL} {script_file}");
        }

        public void Crash(string server_id)
        {
            string url;
            if (!servers.TryGetValue(server_id, out url)) return;
            TcpChannel channel = new TcpChannel();
            System.Runtime.Remoting.Channels.ChannelServices.RegisterChannel(channel, false);
            IServer server = (IServer)Activator.GetObject(typeof(IServer), url);
            server.Crash();
        }

        public void Freeze(string server_id)
        {
            string url;
            if (!servers.TryGetValue(server_id, out url)) return;
            TcpChannel channel = new TcpChannel();
            System.Runtime.Remoting.Channels.ChannelServices.RegisterChannel(channel, false);
            IServer server = (IServer)Activator.GetObject(typeof(IServer), url);
            server.Freeze();
        }

        public string[] GetServers()
        {
            string[] ret = new string[servers.Keys.Count];
            servers.Keys.CopyTo(ret, 0);
            return ret;
        }

        public void Server(string server_id, string URL, int max_faults, int min_delay, int max_delay)
        {
            Console.WriteLine($"starting server: {server_id} {URL} {max_faults} {min_delay} {max_delay}");
            Process.Start(@"Server.exe", $"{server_id} {URL} {max_faults} {min_delay} {max_delay}");
            servers.Add(server_id, URL);
        }

        public string Status()
        {
            return "";
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
