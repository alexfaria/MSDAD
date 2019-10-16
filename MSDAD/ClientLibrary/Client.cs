using CommonTypes;
using System;
using System.Net.Sockets;
using System.Runtime.Remoting.Channels.Tcp;

namespace Client
{
    class Client
    {
        static void Main(string[] args)
        {
            TcpChannel channel = new TcpChannel();
            System.Runtime.Remoting.Channels.ChannelServices.RegisterChannel(channel, true);

            IServer serverObj = (IServer)Activator.GetObject(
                typeof(IServer),
                "tcp://localhost:8086/Server");

            try
            {
                string message = Console.ReadLine();
                serverObj.WriteLine(message);
            }
            catch (SocketException)
            {
                System.Console.WriteLine("Could not locate server");
            }

            Console.ReadLine();
        }
    }
}
