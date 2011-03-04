using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pandemic.Game.DataTypes
{
    class Virus
    {
        int blue; // 0
        int yellow; // 1
        int black; // 2
        int orange; // 3

        public Virus()
        {
        }

        public Virus(int[] viruses)
        {
            blue = viruses[0];
            yellow = viruses[1];
            black = viruses[2];
            orange = viruses[3];
        }

        public void add(int[] viruses)
        {
            blue += viruses[0];
            yellow += viruses[1];
            black += viruses[2];
            orange += viruses[3];

            //check for epidemic
        }
    }
}
