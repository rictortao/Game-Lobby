using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Pandemic.Game.DataTypes;

namespace Pandemic.Game.APIs
{
    class SPI : API
    {
        private game Game;

        public SPI(lobbyVals gameParams)
        {
            Game = new game(gameParams);
        }

        // Return Game State
        public override GameState updateUI()
        {
            return Game.State;
        }

        public override Mutex getMutex()
        {
            return Game.mux;
        }

    }
}
