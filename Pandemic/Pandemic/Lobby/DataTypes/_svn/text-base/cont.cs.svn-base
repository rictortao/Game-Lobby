using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pandemic.DataTypes
{
    [Serializable]
    public class cont
    {
        private int status = 0;
        private int charClass = 0;
        private int playerPos = 0;

        public bool host;

        public bool chk = false;
        public int num;
        public int role;

        public int difficulty = 1;
        public bool chgEmpty = false;
        public int slot;

        public cont()
        {
        }



        public int Status
        {
            get
            {
                return status;
            }
            set
            {
                status = value;
            }
        }
        public int CharacterClass
        {
            get
            {
                return charClass;
            }
            set
            {
                charClass = value;
            }
        }
        public int PlayerPos
        {
            get
            {
                return playerPos;
            }
            set
            {
                playerPos = value;
            }
        }



        private string StatusLookup(int status)
        {
            string playerStatus = null;
            switch (status)
            {
                case 0:
                    playerStatus = null;
                    break;
                case 1:
                    playerStatus = "Connected";
                    break;
                case 2:
                    playerStatus = "Disconnected";
                    break;
                case 3:
                    playerStatus = "Ready";
                    break;
                default:
                    playerStatus = null;
                    break;
            }
            return playerStatus;

        }

        private string ClassLookup(int charClass)
        {
            string playerChar = null;
            switch (charClass)
            {
                case 0:
                    playerChar = "Undefined";
                    break;
                case 1:
                    playerChar = "Random";
                    break;
                case 2:
                    playerChar = "Dispatcher";
                    break;
                case 3:
                    playerChar = "Medic";
                    break;
                case 4:
                    playerChar = "Scientist";
                    break;
                case 5:
                    playerChar = "Researcher";
                    break;
                case 6:
                    playerChar = "Operations Expert";
                    break;
                default:
                    playerChar = "Random";
                    break;
            }
            return playerChar;
        }

    }

}
