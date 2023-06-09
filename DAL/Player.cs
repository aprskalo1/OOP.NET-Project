﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class Player
    {
        public Player(string name, bool captain, int shirtNumber, string position)
        {
            Name = name;
            Captain = captain;
            ShirtNumber = shirtNumber;
            Position = position;
        }

        public string Name { get; set; }
        public bool Captain { get; set; }
        public int ShirtNumber { get; set; }
        public string Position { get; set; }

        public string GetName()
            => $"{Name}";
    }
}
