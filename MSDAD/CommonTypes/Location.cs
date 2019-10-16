using System;
using System.Collections.Generic;

namespace CommonTypes
{
    [Serializable]
    public class Location
    {
        string name;
        Dictionary<string, int> rooms;

        public Location(string name)
        {
            this.name = name;
            this.rooms = new Dictionary<string, int>();
        }
    }
}
