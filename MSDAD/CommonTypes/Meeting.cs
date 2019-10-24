using System;
using System.Collections.Generic;

namespace CommonTypes
{
    [Serializable]
    public class Meeting
    {
        string coordinator;
        public string topic;
        int min_participants;
        List<string> invitees;
        public List<Slot> slots;
        bool closed;

        public Meeting(string coordinator, string topic, int min_participants, List<Slot> slots)
        {
            closed = false;
            this.topic = topic;
            this.slots = slots;
            this.coordinator = coordinator;
            this.min_participants = min_participants;
        }
        public Meeting(string coordinator, string topic, int min_participants, List<string> invitees, List<Slot> slots) : this(coordinator, topic, min_participants, slots)
        {
            this.invitees = invitees;
        }

        public void AddParticipant(string user, Slot slot)
        {
            Slot sl = slots.Find((s) => s.Equals(slot));
            sl.participants.Add(user);
        }

        public override bool Equals(object obj)
        {
            Meeting other = (Meeting)obj;
            return this.topic == other.topic;
        }
        public override string ToString()
        {
            return string.Format("Meeting<{0}, {1}, {2}, {3}>", topic, coordinator, min_participants, closed ? "closed" : "open");
        }
    }
}
