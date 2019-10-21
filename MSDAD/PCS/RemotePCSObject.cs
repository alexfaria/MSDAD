using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTypes;

namespace PCS
{
    class RemotePCSObject : MarshalByRefObject, IPCS
    {
        public void AddRoom(string Location, int capacity, string room_name)
        {
            throw new NotImplementedException();
        }

        public void Client(string username, string client_URL, string server_URL, string script_file)
        {
            throw new NotImplementedException();
        }

        public void Server(string server_id, string URL, int max_faults, int min_delay, int max_delay)
        {
            // throw new NotImplementedException();
            Process.Start(@"bin\Server.exe", $"{server_id} {URL} {max_faults} {min_delay} {max_delay}");
        }

        public void Status()
        {
            throw new NotImplementedException();
        }
    }
}
