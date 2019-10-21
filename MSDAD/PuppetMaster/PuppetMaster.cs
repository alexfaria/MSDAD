using CommonTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using System.Threading.Tasks;

namespace PuppetMaster
{
    class PuppetMaster
    {
        static void Main(string[] args)
        {

            TcpChannel channel = new TcpChannel();
            System.Runtime.Remoting.Channels.ChannelServices.RegisterChannel(channel, true);
            IPCS PCSObj = (IPCS)Activator.GetObject(typeof(IPCS), "tcp://localhost:10000/pcs");

            PCSObj.Server("server_1", "localhost:8080/Server", 2, 1, 1);
        }
    }
}
