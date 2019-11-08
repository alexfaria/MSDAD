using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTypes
{
    [Serializable]
    public class Room
    {
        public readonly string name;
        public readonly int capacity;
        public List<DateTime> booked;
        public Room(string name, int capacity)
        {
            this.name = name;
            this.capacity = capacity;
            this.booked = new List<DateTime>();
        }
    }
}
