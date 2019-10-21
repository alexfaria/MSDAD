using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using System.Threading.Tasks;

namespace PCS
{
    class PCS
    {
        RemotePCSObject remotePCS;
        static void Main(string[] args)
        {
            TcpChannel channel = new TcpChannel(10000);
            ChannelServices.RegisterChannel(channel, true);
            RemotePCSObject mo = new RemotePCSObject();
            RemotingServices.Marshal(mo, "Server", typeof(RemotePCSObject));

        }
    }
}
