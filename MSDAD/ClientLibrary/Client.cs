using CommonTypes;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Messaging;
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
            //this.GetClients();
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
                remoteClient.meetings = remoteServer.GetMeetings(remoteClient.meetings);
            }
            catch (Exception)
            {
                Reconnect();
            }

            foreach (Meeting m in remoteClient.meetings)
            {
                m.PrettyPrint();
            }
        }

        /**
         * meeting_topic min_attendees number_of_slots number_of_invitees slot_1 ... slot_n invitee_1 ... invitee_n
         * 
         * creates a new meeting identiﬁed by meeting topic with a min attendees required number of atendees,
         * with a number of slots large set of possible dates and locations and with a number of invitees large group of invited users.
         * meeting topic is a string which may contain letters and the underscore character such as ”budget 2020”.
         * Each slot n is a location followed by a date with all elements separated by a comma and hyphens such
         * as "Lisboa,2020-01-02". Each invitee n is the username of an invited client or user (see 4 below). 
         */
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
                remoteServer.CreateMeeting(meeting); // Synchronous call to ensure success
            }
            catch (ApplicationException e)
            {
                Console.WriteLine(e.Message);
                return;
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

            remoteServer.JoinMeeting(username, topic, slots);
        }

        public void CloseMeeting(string[] args)
        {
            string topic = args[1];
            remoteServer.CloseMeeting(username, topic);
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
            Console.WriteLine("Client.GetAlternativeServer()");
            alternativeServerUrl = remoteServer.GetAlternativeServer();
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
            remoteServer = (IServer) Activator.GetObject(typeof(IServer), alternativeServerUrl);
            GetAlternativeServer();
            Register();
        }
    }
}
