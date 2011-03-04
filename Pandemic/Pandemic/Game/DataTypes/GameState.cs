using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Pandemic.Game.DataTypes.Decks;
using Pandemic.Game.DataTypes.Players;

namespace Pandemic.Game.DataTypes
{
    class GameState
    {
        public virusDeck vDeck;
        public virusDeck vDiscard;

        public playerDeck pDeck;
        public playerDeck pDiscard;

        public List<Location> locations;
        public List<Player> players;

        public int Outbreaks = 0;
        public int infectionRate = 0;
        public int[] InfectionRateConv = new int[] { 2, 2, 2, 3, 3, 4, 4 }; //Number of cards drawn on infection phase

        public int[] vBlocks = new int[] { 24, 24, 24, 24 };

        public bool[] cures = new bool[] { false, false, false, false };
        public bool[] cured = new bool[] { false, false, false, false };

        public GameState()
        {
        }

        public List<Location> Locations { get { return locations; } }
    }
}
