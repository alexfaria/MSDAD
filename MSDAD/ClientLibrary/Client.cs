﻿using CommonTypes;
using System;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Threading;

namespace ClientLibrary
{
    public class Client
    {
        private readonly string username;
        private readonly string server_url;
        private readonly string client_url;

        private List<IClient> remoteClients;
        private IServer remoteServer;
        private RemoteClientObject remoteClient;

        private List<Meeting> meetings = new List<Meeting>();

        public Client(string username, string client_url, string server_url)
        {
            this.username = username;
            this.client_url = client_url;
            this.server_url = server_url;

            remoteClients = new List<IClient>();

            Uri uri = new Uri(client_url);
            remoteClient = new RemoteClientObject();

            TcpChannel channel = new TcpChannel();
            ChannelServices.RegisterChannel(channel, false);
            remoteServer = (IServer)Activator.GetObject(typeof(IServer), server_url);
            RemotingServices.Marshal(remoteClient, uri.LocalPath.Trim('/'), typeof(IClient));
        }
        public void ListMeetings()
        {
            meetings = remoteServer.GetMeetings(meetings);
            foreach (Meeting m in meetings)
            {
                Console.WriteLine(m);
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
                string[] date = args[idx].Split('-');
                Slot s = new Slot(
                    new DateTime(
                        Int32.Parse(date[0]),
                        Int32.Parse(date[1]),
                        Int32.Parse(date[2])),
                    slot[0]);
            }
            List<string> invitees = new List<string>(numInvitees);
            length = numInvitees + idx;
            for (; idx < length; ++idx)
            {
                invitees.Add(args[idx]);
            }
            Meeting m = new Meeting(username, topic, minAttendees, invitees, slots);
            remoteClients = remoteServer.CreateMeeting(m);
            // Replicate meeting between clients
        }

        public void JoinMeeting(string[] args)
        {
            int idx = 1;
            string topic = args[idx++];
            string[] slot = args[idx].Split(',');
            string[] date = args[idx].Split('-');
            Slot s = new Slot(
                new DateTime(
                    Int32.Parse(date[0]),
                    Int32.Parse(date[1]),
                    Int32.Parse(date[2])),
                slot[0]);

            remoteServer.JoinMeeting(username, topic, s);
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
    }
}
