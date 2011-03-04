using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pandemic.Game.DataTypes.Cards
{
    class virusCard : Card
    {
        int color;
        string location;

        public virusCard(int inColor, string inLoc) : base(inLoc)
        {
            color = inColor;
            location = inLoc;
        }
    }
}
