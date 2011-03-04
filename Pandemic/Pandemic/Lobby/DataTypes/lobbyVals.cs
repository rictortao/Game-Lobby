﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pandemic
{
    [Serializable]
    public class lobbyVals
    {

        //public string[] players = new string[4]{"Player 1","Player 2","Player 3","Player 4"};

        public string[] players = new string[4] { "", "", "", "" };

        public int[] roles = new int[4]{0,0,0,0};

        public bool[] ready = new bool[4]{false, false, false, false};

        public bool[] slots = new bool[4] { true, true, true, true };

        public int difficulty = 1;

        public lobbyVals()
        {

        }

        
    }
}
