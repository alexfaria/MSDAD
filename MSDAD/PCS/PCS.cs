using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace PCS
{
    class PCS
    {
        private static RemotePCSObject remotePCS;
        static readonly string remoteObjectName = "pcs";

        static void Main(string[] args)
        {
            remotePCS = new RemotePCSObject();

            TcpChannel channel = new TcpChannel(10000);
            ChannelServices.RegisterChannel(channel, false);
            RemotingServices.Marshal(remotePCS, remoteObjectName, typeof(RemotePCSObject));

            string[] urls = channel.GetUrlsForUri(remoteObjectName);

            Console.WriteLine("channel urls:");
            foreach (string url in urls)
                Console.WriteLine("\t{0}", url);

            Console.ReadLine();
        }
    }
}
