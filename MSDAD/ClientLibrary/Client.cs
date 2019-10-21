﻿using CommonTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
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

        private static IServer remoteServer;
        static void Main(string[] args)
        {
            if (args.Length <= 0)
                Console.WriteLine("Please enter the command line arguments.");
            username = args[0];
            client_url = args[1];
            server_url = args[2];
            try
            {
                TcpChannel channel = new TcpChannel();
                ChannelServices.RegisterChannel(channel, true);
                remoteServer = (IServer)Activator.GetObject(
                    typeof(IServer),
                    server_url);
                using (StreamReader sr = new StreamReader(args[3]))
                {
                    string line = sr.ReadLine();
                    while (line != null)
                    {
                        CommandParser(line);
                    }
                }
            } catch (IOException e)
            {
                Console.WriteLine($"Could not read the file: {e.Message}");
            }

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

        private static void CreateMeeting(string[] args) 
        {
            int idx = 1; int length;
            string topic = args[idx++];
            int minAttendees = Int32.Parse(args[idx++]);
            int numSlots = Int32.Parse(args[idx++]);
            int numInvitees = Int32.Parse(args[idx++]);
            List<Slot> slots = new List<Slot>(numSlots);
            length = numSlots + idx;
            for( ; idx < length; ++idx)
            {
                string[] slot = args[idx].Split(',');
                string[] date = args[idx].Split('-');
                Slot s = new Slot(
                    new DateTime(
                        Int32.Parse(date[0]), 
                        Int32.Parse(date[1]), 
                        Int32.Parse(date[2])), 
                    new Location(slot[0]));
            }
            List<string> invitees = new List<string>(numInvitees);
            length = numInvitees + idx;
            for ( ; idx < length; ++idx)
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
            int idx = 1;
            string topic = args[idx];
            remoteServer.CloseMeeting(username, topic);
        }

        private static void Wait(string[] args)
        {
            int time = Int32.Parse(args[1]);
            Thread.Sleep(time);
        }
    }
}
