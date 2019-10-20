using Commontypes;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace Server
{
    class Server
    {
        static void Main(string[] args)
        {
            TcpChannel channel = new TcpChannel(8086);
            ChannelServices.RegisterChannel(channel, true);
            RemoteServerObject mo = new RemoteServerObject();
            RemotingServices.Marshal(mo,
            "Server",
            typeof(RemoteServerObject));
           /* RemotingConfiguration.RegisterWellKnownServiceType(
                typeof(RemoteServerObject),
                "Server",
                WellKnownObjectMode.Singleton);
            */
            System.Console.WriteLine("<enter> para sair...");
            System.Console.ReadLine();
        }
    }
}
