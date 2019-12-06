using CommonTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

namespace ClientLibrary
{
    class RemoteClientObject : MarshalByRefObject, IClient
    {
        private Client client;
        public List<Meeting> meetings = new List<Meeting>();
        public Dictionary<string, string> remoteClients = new Dictionary<string, string>();

        public RemoteClientObject(Client client)
        {
            this.client = client;
        }
        public void ShareMeeting(Meeting meeting)
        {
            Console.WriteLine("[ShareMeeting] " + meeting);
            if (!meetings.Contains(meeting))
            {
                meetings.Add(meeting);
            }
        }

        public void GossipShareMeeting(string senderUrl, Meeting meeting)
        {
            Console.WriteLine("[GossipShareMeeting] " + meeting);
            if (!meetings.Contains(meeting))
            {
                meetings.Add(meeting);
            }
            else
            {
                List<string> gossipPeersUrl = new List<string>();
                try
                {
                    gossipPeersUrl = ((IServer) Activator.GetObject(typeof(IServer), client.ServerUrl)).GetGossipClients(client.ClientUrl, meeting);
                }
                catch (SocketException e)
                {
                    // TODO: reconnect on exception
                    Console.WriteLine($"[GossipShareMeeting] [{e.GetType().Name}] Error trying to contact <{client.ServerUrl}>");
                    client.Reconnect();
                    GossipShareMeeting(senderUrl, meeting);
                }
                Console.WriteLine("GossipClients:");
                foreach (string peerUrl in gossipPeersUrl)
                {
                    if (peerUrl != senderUrl)
                    {
                        try
                        {
                            Console.WriteLine($"  {peerUrl}");
                            ((IClient) Activator.GetObject(typeof(IClient), peerUrl)).GossipShareMeeting(client.ClientUrl, meeting);
                        }
                        catch (SocketException e)
                        {
                            Console.WriteLine($"[GossipShareMeeting] [{e.GetType().Name}] Error trying to contact <{peerUrl}>");
                        }
                    }
                }
            }
        }

        public void Status()
        {
            Console.WriteLine("[Status]");
            Console.WriteLine("Server: \n  " + client.ServerUrl);
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

