using CommonTypes;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Threading;

namespace ClientLibrary
{
    public class Client
    {
        private readonly string username;
        private readonly string clientUrl;
        private string serverUrl;
        private string alternativeServerUrl;

        private IServer remoteServer;
        private RemoteClientObject remoteClient;

        private Dictionary<string, int> vector_clock;

        public Client(string username, string clientUrl, string serverUrl)
        {
            this.username = username;
            this.clientUrl = clientUrl;
            this.serverUrl = serverUrl;
            this.remoteClient = new RemoteClientObject();

            Uri uri = new Uri(clientUrl);

            TcpChannel channel = new TcpChannel(uri.Port);
            ChannelServices.RegisterChannel(channel, false);

            RemotingServices.Marshal(remoteClient, uri.LocalPath.Trim('/'), typeof(IClient));

            this.Reconnect();
        }

        public void GetClients()
        {
            Console.WriteLine("Client.GetClients()");
            remoteClient.remoteClients = remoteServer.GetClients();
            //remoteClients.Remove(this.username);

            foreach (KeyValuePair<string, string> kvp in remoteClient.remoteClients)
            {
                Console.WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
            }
        }

        public void UpdateVectorClock()
        {
            try
            {
                vector_clock = remoteServer.UpdateVectorClock(vector_clock);
            }
            catch (Exception)
            {
                Reconnect();
            }
        }

        public void Register()
        {
            Console.WriteLine("Client.Register()");

            //TODO: retry op
            try
            {
                remoteServer.RegisterClient(this.username, this.clientUrl);
            }
            catch (Exception)
            {
                Reconnect();
                Register();
            }
        }

        public void Unregister()
        {
            Console.WriteLine("Client.Unregister()");
            remoteServer.UnregisterClient(this.username);
        }

        public void ListMeetings()
        {
            try
            {
                remoteClient.meetings = remoteServer.GetMeetings(vector_clock, remoteClient.meetings);
                UpdateVectorClock();
            }
            catch (Exception)
            {
                Reconnect();
                ListMeetings();
            }

            foreach (Meeting m in remoteClient.meetings)
            {
                m.PrettyPrint();
            }
        }

        public void CreateMeeting(string[] args)
        {
            int length;
            int idx = 1;
            string topic = args[idx++];
            int minAttendees = Int32.Parse(args[idx++]);
            int numSlots = Int32.Parse(args[idx++]);
            int numInvitees = Int32.Parse(args[idx++]);

            List<Slot> slots = new List<Slot>(numSlots);
            length = numSlots + idx;

            for (; idx < length; ++idx)
            {
                string[] slot = args[idx].Split(',');
                string[] date = slot[1].Split('-');
                slots.Add(new Slot(
                    new DateTime(
                        Int32.Parse(date[0]),
                        Int32.Parse(date[1]),
                        Int32.Parse(date[2])),
                    slot[0]));
            }
            List<string> invitees = new List<string>(numInvitees);
            length = numInvitees + idx;
            for (; idx < length; ++idx)
            {
                invitees.Add(args[idx]);
            }
            Meeting meeting = new Meeting(username, topic, minAttendees, invitees, slots);
            try
            {
                remoteServer.CreateMeeting(vector_clock, meeting); // Synchronous call to ensure success
                UpdateVectorClock();
            }
            catch (ApplicationException e)
            {
                Console.WriteLine(e.Message);
                return;
            }
            catch (Exception)
            {
                Reconnect();
            }

            // Replicate meeting between clients
            this.GetClients();
            if (numInvitees > 0)
            {
                foreach (string user in meeting.invitees)
                {
                    if (remoteClient.remoteClients.TryGetValue(user, out string client_url))
                    {
                        try
                        {
                            ((IClient) Activator.GetObject(typeof(IClient), client_url)).GossipShareMeeting(meeting);
                        }
                        catch (SocketException) { }
                    }
                }
            }
            else
            {
                // TODO: idk
                // gossip if there are no invitees
                remoteClient.GossipShareMeeting(meeting);
            }
        }

        public void JoinMeeting(string[] args)
        {
            int idx = 1;
            string topic = args[idx++];
            int numSlots = Int32.Parse(args[idx++]);
            List<Slot> slots = new List<Slot>(numSlots);
            int length = numSlots + idx;
            for (; idx < length; ++idx)
            {
                string[] slot = args[idx].Split(',');
                string[] date = slot[1].Split('-');
                slots.Add(new Slot(
                    new DateTime(
                        Int32.Parse(date[0]),
                        Int32.Parse(date[1]),
                        Int32.Parse(date[2])),
                    slot[0]));
            }
            try
            {
                remoteServer.JoinMeeting(username, vector_clock, topic, slots);
                UpdateVectorClock();
            }
            catch (ApplicationException e)
            {
                Console.WriteLine(e.Message);
                return;
            }
            catch (Exception)
            {
                Reconnect();
            }
        }

        public void CloseMeeting(string[] args)
        {
            string topic = args[1];
            try
            {
                remoteServer.CloseMeeting(vector_clock, username, topic);
                UpdateVectorClock();
            }
            catch (ApplicationException e)
            {
                Console.WriteLine(e.Message);
                return;
            }
            catch (Exception)
            {
                Reconnect();
            }

        }

        public void Wait(string[] args)
        {
            int time = Int32.Parse(args[1]);
            Thread.Sleep(time);
        }

        public void Status()
        {
            this.remoteClient.Status();
        }

        private void GetAlternativeServer()
        {
            Console.Write("GetAlternativeServer: ");
            alternativeServerUrl = remoteServer.GetAlternativeServer();
            Console.WriteLine(alternativeServerUrl);
        }

        private bool Connected()
        {
            try
            {
                remoteServer.Ping();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        private void Reconnect()
        {
            if (!Connected())
            {
                try
                {
                    remoteServer = (IServer) Activator.GetObject(typeof(IServer), serverUrl);
                }
                catch (Exception)
                {
                    remoteServer = (IServer) Activator.GetObject(typeof(IServer), alternativeServerUrl);
                    serverUrl = alternativeServerUrl;
                    GetAlternativeServer();
                    Register();
                }
                finally
                {
                    Console.WriteLine($"Connected to '{serverUrl}'");
                }
            }
        }
    }
}
