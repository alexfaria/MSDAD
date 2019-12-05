using CommonTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

namespace ClientLibrary
{
    class RemoteClientObject : MarshalByRefObject, IClient
    {
        public string server_url;
        public List<Meeting> meetings = new List<Meeting>();
        public Dictionary<string, string> remoteClients = new Dictionary<string, string>();

        public RemoteClientObject(string server_url)
        {
            this.server_url = server_url;
        }
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
            List<string> gossip_clients = new List<string>();
            try
            {

                gossip_clients = ((IServer)Activator.GetObject(typeof(IServer), server_url)).GetGossipClients(meeting);
            }
            catch (SocketException e)
            {
                Console.WriteLine($"[GossipShareMeeting] [{e.GetType().Name}] Error trying to contact <{server_url}>");
            }
            Console.WriteLine("GossipClients:");
            foreach (string client_url in gossip_clients)
            {
                try
                {
                    Console.WriteLine($"  {client_url}");
                    ((IClient)Activator.GetObject(typeof(IClient), client_url)).GossipShareMeeting(meeting);
                }
                catch (SocketException e)
                {
                    Console.WriteLine($"[GossipShareMeeting] [{e.GetType().Name}] Error trying to contact <{client_url}>");
                }
            }
        }

        public void Status()
        {
            Console.WriteLine("[Status]");
            Console.WriteLine("Server: \n  " + server_url);
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
