using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Pandemic.Game.DataTypes;

namespace Pandemic.Game.APIs
{
    class API
    {
        public virtual GameState updateUI()
        {
            return null;
        }

        public virtual Mutex getMutex()
        {
            return null;
        }
    }
}
