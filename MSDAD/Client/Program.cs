using ClientLibrary;
using System;
using System.Collections.Generic;
using System.IO;

namespace ClientScript
{
    class Program
    {
        private static string username;
        private static string client_url;
        private static string server_url;
        private static string script_file;

        private static Client client;

        static void Main(string[] args)
        {
            if (args.Length < 4)
            {
                Console.WriteLine("usage: ./Client.exe <username> <client_URL> <server_URL> <script_file>");
                return;
            }

            username = args[0];
            client_url = args[1];
            server_url = args[2];
            script_file = args[3];

            Console.Title = $"{username} at {client_url} connected to {server_url}";

            client = new Client(username, client_url, server_url);

            try
            {
                bool continueFlag = false;
                string[] fileLines = File.ReadAllLines(script_file);
                List<string> commands = new List<string>(fileLines);

                Console.WriteLine("Keyboard Commands:");
                Console.WriteLine("\tc: (continue) run all remaining commands");
                Console.WriteLine("\tn: (run) run command");
                Console.WriteLine("\ts: (skip) skip command");
                Console.WriteLine("\te: (exit) skip all commands");

                for (int i = 0; i < commands.Count; i++)
                {
                    //if (i > 0)
                    //{
                    //    Console.WriteLine("  " + commands[i - 1]);
                    //}

                    Console.WriteLine("> " + commands[i]);

                    for (int j = i + 1; j < 5 && j < commands.Count; j++)
                    {
                        Console.WriteLine("  " + commands[j]);
                    }

                    ConsoleKeyInfo key = Console.ReadKey(true);

                    switch (key.Key)
                    {
                        case ConsoleKey.C:
                            continueFlag = true;
                            break;
                        case ConsoleKey.N:
                            CommandParser(commands[i]);
                            break;
                        case ConsoleKey.E:
                            i = commands.Count + 1;
                            break;
                        case ConsoleKey.S:
                            break;
                        default:
                            break;
                    }

                    Console.WriteLine();
                    if (continueFlag)
                    {
                        for (; i < commands.Count; i++)
                        {
                            CommandParser(commands[i]);
                        }
                        break;
                    }
                }
            }
            catch (IOException)
            {
                Console.WriteLine($"Could not read the file: {script_file}");
            }

            string command = "";
            while (command != "exit")
            {
                Console.Write("> ");
                command = Console.ReadLine();
                CommandParser(command);
            }

            client.Unregister();
        }

        private static void CommandParser(string line)
        {
            string[] commandLine = line.Split(' ');
            if (commandLine.Length <= 0)
                return;

            Console.WriteLine($"--> Running command: {line}");
            try
            {
                switch (commandLine[0])
                {
                    case "list":
                        client.ListMeetings();
                        break;
                    case "create":
                        client.CreateMeeting(commandLine);
                        break;
                    case "join":
                        client.JoinMeeting(commandLine);
                        break;
                    case "close":
                        client.CloseMeeting(commandLine);
                        break;
                    case "wait":
                        client.Wait(commandLine);
                        break;
                    case "exit":
                        break;
                    default:
                        Console.WriteLine($"Invalid command: {line}");
                        break;
                }
            }
            catch (ApplicationException e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
