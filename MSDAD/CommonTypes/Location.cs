using System;
using System.Collections.Generic;

namespace CommonTypes
{
    [Serializable]
    public class Location
    {
        public string name;
        public List<Room> rooms;
        
        public Location(string name, List<Room> rooms)
        {
            this.name = name;
            this.rooms = rooms;
        }

    }
}
