using CommonTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace Server
{
    class Server
    {
        private const string CONFIG_FILE = "../serverlist.txt";
        static void Main(string[] args)
        {
            if (args.Length < 5)
            {
                Console.WriteLine("usage: ./Server.exe <server_id> <url> <max_faults> <max_delay> <min_delay>");
                return;
            }

            string server_id = args[0];
            string url = args[1];
            int max_faults = Int32.Parse(args[2]);
            int min_delay = Int32.Parse(args[3]);
            int max_delay = Int32.Parse(args[4]);
            Uri uri = new Uri(url);

            List<IServer> servers = new List<IServer>();

            Console.WriteLine($"starting server: {server_id} {url} {max_faults} {max_delay} {min_delay}");
            TcpChannel channel = new TcpChannel(uri.Port);
            ChannelServices.RegisterChannel(channel, false);

            try
            {
                using (StreamReader sr = new StreamReader(CONFIG_FILE))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (!line.Equals(url))
                            servers.Add((IServer)Activator.GetObject(typeof(IServer), line));
                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine($"Could not read the configuration file: {e.Message}");
            }

            RemoteServerObject remoteServerObj = new RemoteServerObject(max_faults, max_delay, min_delay, servers);
            RemotingServices.Marshal(remoteServerObj, uri.LocalPath.Trim('/'), typeof(RemoteServerObject));

            Console.ReadLine();
        }
    }
}
