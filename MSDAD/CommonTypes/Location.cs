﻿using System;
using System.Collections.Generic;

namespace CommonTypes
{
    [Serializable]
    public class Location
    {
        public string name;
        Dictionary<string, int> rooms;
        List<DateTime> booked;

        public Location(string name)
        {
            this.name = name;
            this.rooms = new Dictionary<string, int>();
            this.booked = new List<DateTime>();
        }

        public override bool Equals(object obj)
        {
            Location other = (Location)obj;
            return this.name == other.name;
        }
    }
}
