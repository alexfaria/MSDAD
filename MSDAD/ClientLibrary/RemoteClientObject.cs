using CommonTypes;
using System;
using System.Collections.Generic;

namespace ClientLibrary
{
    class RemoteClientObject : MarshalByRefObject, IClient
    {
        public List<Meeting> meetings = new List<Meeting>();
        public void ShareMeeting(Meeting m)
        {
            Console.WriteLine("[ShareMeeting] " + m);
            if (!meetings.Contains(m))
            {
                meetings.Add(m);
            }
        }

        public void Status()
        {
            Console.WriteLine("[Status]");
            foreach (Meeting m in meetings)
            {
                m.PrettyPrint();
            }
        }
    }
}
