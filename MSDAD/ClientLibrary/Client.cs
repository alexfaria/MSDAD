using CommonTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Threading;

namespace Client
{
    class Client
    {
        private static string username;
        private static string client_url;
        private static string server_url;
        private static string script_file;

        private static IClient remoteClient;
        private static IServer remoteServer;
        static void Main(string[] args)
        {
            if (args.Length <= 0)
            {
                Console.WriteLine("usage: ./Client.exe <username> <client_URL> <server_URL> <script_file>");
                Console.WriteLine("<enter> para sair...");
                Console.ReadLine();
                return;
            }

            username = args[0];
            client_url = args[1];
            server_url = args[2];
            script_file = args[3];

            TcpChannel channel = new TcpChannel();
            ChannelServices.RegisterChannel(channel, false);
            remoteServer = (IServer)Activator.GetObject(typeof(IServer), server_url);

            try
            {
                using (StreamReader sr = new StreamReader(script_file))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        CommandParser(line);
                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine($"Could not read the file: {e.Message}");
            }

            Console.WriteLine("<enter> para sair...");
            Console.ReadLine();
        }

        private static void CommandParser(string line)
        {
            string[] commandLine = line.Split(' ');
            if (commandLine.Length <= 0)
                return;

            Console.WriteLine($"--> Running command: {line}");
            switch (commandLine[0])
            {
                case "list":
                    ListMeetings();
                    break;
                case "create":
                    CreateMeeting(commandLine);
                    break;
                case "join":
                    JoinMeeting(commandLine);
                    break;
                case "close":
                    CloseMeeting(commandLine);
                    break;
                case "wait":
                    Wait(commandLine);
                    break;
                default:
                    Console.WriteLine($"Invalid command: {line}");
                    break;
            }
        }

        private static void ListMeetings()
        {
            List<Meeting> meetings = remoteServer.GetMeetings();
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
        private static void CreateMeeting(string[] args)
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
            remoteServer.CreateMeeting(m);
        }

        private static void JoinMeeting(string[] args)
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
                new Location(slot[0]));

            remoteServer.JoinMeeting(username, topic, s);
        }

        private static void CloseMeeting(string[] args)
        {
            string topic = args[1];
            remoteServer.CloseMeeting(username, topic);
        }

        private static void Wait(string[] args)
        {
            int time = Int32.Parse(args[1]);
            Thread.Sleep(time);
        }
    }
}
