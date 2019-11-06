using System;
using System.Collections.Generic;

namespace CommonTypes
{
    [Serializable]
    public class Location
    {
        public string name;
        public List<Room> rooms;

        public Location(string name)
        {
            this.name = name;
            this.rooms = new List<Room>();
        }
        public Location(string name, List<Room> rooms) : this(name)
        {
            this.rooms = rooms;
        }
    }
}
