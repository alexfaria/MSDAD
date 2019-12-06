using CommonTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

namespace ClientLibrary
{
    class RemoteClientObject : MarshalByRefObject, IClient
    {
        public string serverUrl;
        public string clientUrl;
        public List<Meeting> meetings = new List<Meeting>();
        public Dictionary<string, string> remoteClients = new Dictionary<string, string>();

        public RemoteClientObject(string clientUrl, string serverUrl)
        {
            this.serverUrl = serverUrl;
            this.clientUrl = clientUrl;
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
            else
            {
                List<string> gossipPeersUrl = new List<string>();
                try
                {
                    gossipPeersUrl = ((IServer)Activator.GetObject(typeof(IServer), serverUrl)).GetGossipClients(clientUrl, meeting);
                }
                catch (SocketException e)
                {
                    Console.WriteLine($"[GossipShareMeeting] [{e.GetType().Name}] Error trying to contact <{serverUrl}>");
                }
                Console.WriteLine("GossipClients:");
                foreach (string peerUrl in gossipPeersUrl)
                {
                    try
                    {
                        Console.WriteLine($"  {peerUrl}");
                        ((IClient)Activator.GetObject(typeof(IClient), peerUrl)).GossipShareMeeting(meeting);
                    }
                    catch (SocketException e)
                    {
                        Console.WriteLine($"[GossipShareMeeting] [{e.GetType().Name}] Error trying to contact <{peerUrl}>");
                    }
                }
            }
        }

        public void Status()
        {
            Console.WriteLine("[Status]");
            Console.WriteLine("Server: \n  " + serverUrl);
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
