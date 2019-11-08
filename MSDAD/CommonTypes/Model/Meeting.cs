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
        public Slot slot;
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
        public bool Join(string user, List<Slot> slots)
        {
            bool addedParticipants = false;
            foreach (Slot s in this.slots.FindAll(s => slots.Contains(s)))
            {
                if (!s.participants.Contains(user))
                {
                    s.participants.Add(user);
                    addedParticipants = true;
                }
            }
            return addedParticipants;
        }
        public override bool Equals(object obj)
        {
            Meeting other = (Meeting) obj;
            return this.topic == other.topic;
        }
        public override string ToString()
        {
            return PrettyToString();
            //return string.Format("Meeting<{0}, {1}, {2}, {3}>", topic, coordinator, min_participants, status);
        }
        public string PrettyToString()
        {
            string prettyPrint;
            prettyPrint = $"'{topic}': '{status}\n";
            prettyPrint += $"  coord: {coordinator}\n";
            prettyPrint += $"  min: {min_participants}\n";
            if (invitees.Count > 0)
            {
                prettyPrint += "  invitiees: \n";
                foreach (string inv in invitees)
                {
                    prettyPrint += "   " + inv + " \n";
                }
            }

            if (status == Status.Closed)
            {
                prettyPrint += $"  room: {room.name}";

                prettyPrint += $"  {slot.location},{slot.date.Year}-{slot.date.Month}-{slot.date.Day}\n";
                foreach (string participant in slot.participants)
                {
                    prettyPrint += $"    {participant}\n";
                }

            }
            else
            {
                if (slots.Count > 0)
                {
                    prettyPrint += $"  slots: ";
                    foreach (Slot slot in slots)
                    {
                        prettyPrint += $"   {slot.location},{slot.date.Year}-{slot.date.Month}-{slot.date.Day}\n";
                        foreach (string participant in slot.participants)
                        {
                            prettyPrint += $"    {participant}\n";
                        }
                    }
                }
            }

            return prettyPrint;
        }
        public void PrettyPrint()
        {
            Console.Write(PrettyToString());
        }
    }
}
