using ClientLibrary;
using CommonTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using System.Threading.Tasks;

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

            client = new Client(username, server_url);

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
                default:
                    Console.WriteLine($"Invalid command: {line}");
                    break;
            }
        }
    }
}
