using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pandemic.Game.DataTypes
{
    class Location
    {
        Virus infections;
        GameState Game;

        // blue; // 0
        // yellow; // 1
        // black; // 2
        // red; // 3

        int[] Viruses = new int[]{0,0,0,0};

        string name;
        int color;
        bool station;

        bool outBreaking = false;

        List<Location> connections;

        public Location(string inName, int inColor, ref GameState Game)
        {
            this.Game = Game;
            connections = new List<Location>();

            name = inName;
            color = inColor;
        }

        public void addConn(ref Location newCity)
        {
            connections.Add(newCity);
        }

        // Add Locations Color
        public void addInfection(int number)
        {
            Viruses[color] += number;
            Game.vBlocks[color] -= number;

            // Check for Outbreak

            if (Viruses[color] > 3)
            {
                Viruses[color] = 3;
                outBreak(color);
            }
        }

        // Add Specific Color
        public void addInfection(int Color, int Number)
        {
            Viruses[Color] += Number;
            Game.vBlocks[color] -= Number;

            if (Viruses[color] > 3)
            {
                Viruses[color] = 3;
                outBreak(color);
            }
        }

        // Build Station
        public bool buildStation()
        {
            if (!station)
            {
                station = true;
                return true;
            }
            else
                return false;
        }

        public void resetOutbreak()
        {
            outBreaking = false;
        }

        // Handle Outbreaks
        private void outBreak(int color)
        {
            if (!outBreaking)
            {
                outBreaking = true;
                if (++Game.Outbreaks > 7)
                {
                    // Sig Game Over Man
                }
                else
                {

                    foreach (Location city in connections)
                    {
                        city.addInfection(color, 1);
                    }
                }
            }
        }

        public string Name { get { return name; } }
        public bool Station { get { return station; } }
        public int[] viruses { get { return Viruses; } }
    }
}