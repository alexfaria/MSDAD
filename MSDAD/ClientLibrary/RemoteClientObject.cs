using CommonTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

namespace ClientLibrary
{
    class RemoteClientObject : MarshalByRefObject, IClient
    {
        private static readonly int GOSSIP_SHARE = 2;
        public List<Meeting> meetings = new List<Meeting>();
        public Dictionary<string, string> remoteClients = new Dictionary<string, string>();
        public void ShareMeeting(Meeting meeting)
        {
            Console.WriteLine("[ShareMeeting] " + meeting);
            if (!meetings.Contains(meeting))
            {
                meetings.Add(meeting);
            }
        }

        public void GossipShareMeeting(Meeting meeting)
        {
            Console.WriteLine("[GossipShareMeeting] " + meeting);
            if (!meetings.Contains(meeting))
            {
                meetings.Add(meeting);
            }
            else
            {
                for (int i = 0; i < GOSSIP_SHARE; i++)
                {
                    Random rand = new Random();
                    int j = rand.Next(remoteClients.Count);

                    try
                    {
                        ((IClient) Activator.GetObject(typeof(IClient), remoteClients.Values.ElementAt(j))).GossipShareMeeting(meeting);
                    }
                    catch (SocketException) { }

                }
            }
        }

        public void Status()
        {
            Console.WriteLine("[Status]");
            Console.WriteLine("Meetings:");
            foreach (Meeting m in meetings)
            {
                m.PrettyPrint();
            }

            Console.WriteLine("Clients:");
            foreach (KeyValuePair<string, string> entry in remoteClients)
            {
                Console.WriteLine($"  {entry.Key} \t @ {entry.Value}");
            }
        }
    }
}
