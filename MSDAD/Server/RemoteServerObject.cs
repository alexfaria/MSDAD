using System;
using System.Collections.Generic;
using CommonTypes;

namespace Server
{
    class RemoteServerObject : MarshalByRefObject, IServer
    {
        public void WriteLine(string message)
        {
            Console.WriteLine(message);
        }

        public List<Meeting> getMeetings()
        {
            List<Meeting> meetings = new List<Meeting>();
            Meeting meeting = new Meeting("coordinator", "Cenas2020", 2);
            meetings.Add(meeting);
            return meetings;
        }
    }
}
