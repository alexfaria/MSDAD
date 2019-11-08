using System;
using System.Collections.Generic;

namespace CommonTypes
{
    [Serializable]
    public enum Status
    {
        Open,
        Cancelled,
        Closing,
        Closed
    }
    [Serializable]
    public class Meeting
    {
        public string coordinator;
        public string topic;
        public int min_participants;
        public List<string> invitees;
        public List<Slot> slots;
        public Room room;
        public Status status;

        public Meeting(string coordinator, string topic, int min_participants, List<Slot> slots)
        {
            this.status = Status.Open;
            this.topic = topic;
            this.slots = slots;
            this.coordinator = coordinator;
            this.min_participants = min_participants;
        }
        public Meeting(string coordinator, string topic, int min_participants, List<string> invitees, List<Slot> slots) : this(coordinator, topic, min_participants, slots)
        {
            this.invitees = invitees;
        }
        public void Copy(Meeting other)
        {
            this.coordinator = other.coordinator;
            this.min_participants = other.min_participants;
            this.invitees = new List<string>(other.invitees);
            this.slots = new List<Slot>(other.slots);
        }
        public override bool Equals(object obj)
        {
            Meeting other = (Meeting)obj;
            return this.topic == other.topic;
        }
        public override string ToString()
        {
            return string.Format("Meeting<{0}, {1}, {2}, {3}>", topic, coordinator, min_participants, status);
        }
    }
}
