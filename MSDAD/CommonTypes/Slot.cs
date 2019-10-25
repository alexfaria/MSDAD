using System;
using System.Collections.Generic;

namespace CommonTypes
{
    [Serializable]
    public class Slot
    {
        public DateTime date;
        public string location;
        public List<string> participants;

        public Slot(DateTime date, string location) 
        {
            this.date = date;
            this.location = location;
            this.participants = new List<string>();
        }

        public override bool Equals(object obj)
        {
            Slot other = (Slot)obj;
            return this.date.Equals(other.date) && this.location.Equals(other.location);
        }
        public override string ToString()
        {
            return $"{location},{date.Year}-{date.Month}-{date.Day} ({participants.Count} participants)";
        }
    }
}
