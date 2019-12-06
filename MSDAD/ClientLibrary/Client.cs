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
        private readonly RemoteClientObject remoteClient;

        private VectorClock vector_clock;

        public Client(string username, string clientUrl, string serverUrl)
        {
            this.username = username;
            this.clientUrl = clientUrl;
            this.serverUrl = serverUrl;
            this.remoteClient = new RemoteClientObject(clientUrl, serverUrl);
            this.vector_clock = new VectorClock();

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
            catch (SocketException)
            {
                Reconnect();
                UpdateVectorClock();
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
            catch (SocketException)
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
            catch (SocketException)
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
                remoteClient.meetings.Add(meeting); 
                UpdateVectorClock();
            }
            catch (ApplicationException e)
            {
                Console.WriteLine(e.Message);
                return;
            }
            catch (SocketException)
            {
                Reconnect();
                CreateMeeting(args);
            }

            // Gossip meeting between clients
            remoteClient.GossipShareMeeting(meeting);
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
            catch (SocketException)
            {
                Reconnect();
                JoinMeeting(args);
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
            catch (SocketException)
            {
                Reconnect();
                CloseMeeting(args);
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
                if (remoteServer != null)
                {
                    remoteServer.Ping();
                }
                else
                {
                    return false;
                }
            }
            catch (SocketException)
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
                    Register();
                }
                catch (SocketException)
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
