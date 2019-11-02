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
            if (!meetings.Contains(m))
                meetings.Add(m);
        }
    }
}
