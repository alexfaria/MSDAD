using System;
using System.Collections.Generic;

namespace CommonTypes
{
    [Serializable]
    public class Meeting
    {
        string coordinator;
        string topic;
        int min_participants;
        List<string> invitees;
        Dictionary<string, Location> slots;

        public Meeting(string coordinator, string topic, int min_participants)
        {
            this.coordinator = coordinator;
            this.topic = topic;
            this.min_participants = min_participants;
            this.invitees = new List<string>();
        }

        public override string ToString()
        {
            return string.Format("Meeting<{0},{1},{2}>", topic, coordinator, min_participants);
        }
    }
}
