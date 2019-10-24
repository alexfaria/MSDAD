using Commontypes;
using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace Server
{
    class Server
    {
        static void Main(string[] args)
        {
            if (args.Length <= 0)
            {
                Console.WriteLine("usage: ./Server.exe <server_id> <url> <max_faults> <max_delay> <min_delay>");
                Console.WriteLine("<enter> para sair...");
                Console.ReadLine();
                return;
            }

            string server_id = args[0];
            string url = args[1];
            int max_faults = Int32.Parse(args[2]);
            int min_delay = Int32.Parse(args[3]);
            int max_delay = Int32.Parse(args[4]);
            Uri uri = new Uri(url);

            Console.WriteLine($"server: {server_id} {url} {max_faults} {max_delay} {min_delay}");

            TcpChannel channel = new TcpChannel(uri.Port);
            ChannelServices.RegisterChannel(channel, true);
            RemoteServerObject remoteServerObj = new RemoteServerObject();
            RemotingServices.Marshal(remoteServerObj, uri.LocalPath.Trim('/'), typeof(RemoteServerObject));

            Console.WriteLine("<enter> para sair...");
            Console.ReadLine();
        }
    }
}
