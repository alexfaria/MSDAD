using System;
using System.Collections.Generic;

namespace CommonTypes
{
    [Serializable]
    public class Meeting
    {
        public string id;
        string coordinator;
        public string topic;
        int min_participants;
        List<string> invitees;
        public List<Slot> slots;

        public Meeting(string coordinator, string topic, int min_participants, List<string> invitees, List<Slot> slots)
        {
            this.coordinator = coordinator;
            this.topic = topic;
            this.min_participants = min_participants;
            this.invitees = invitees;
            this.slots = slots;

            this.id = $"{coordinator.GetHashCode()}{topic.GetHashCode()}{min_participants.GetHashCode()}";
            if (invitees != null)
                foreach (string i in invitees)
                    id += i.GetHashCode();
            foreach (Slot s in slots)
                id += $"{s.date.GetHashCode()}{s.location.name.GetHashCode()}";           
        }

        public void AddParticipant(string user, Slot slot)
        {
            Slot sl = slots.Find((s) => s.Equals(slot));
            sl.participants.Add(user);
        }

        public override bool Equals(object obj)
        {
            Meeting other = (Meeting)obj;
            return this.id == other.id;
        }
        public override string ToString()
        {
            return string.Format("Meeting<{0},{1},{2}>", topic, coordinator, min_participants);
        }
    }
}
