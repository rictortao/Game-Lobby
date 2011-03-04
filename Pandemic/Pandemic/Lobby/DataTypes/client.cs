using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Pandemic.DataTypes
{
    public class client
    {
        public string nick;
        public System.Net.Sockets.TcpClient connInfo;
        public System.Net.Sockets.TcpClient chatInfo;

        public client()
        {
            nick = "";
            connInfo = null;
            chatInfo = null;
        }
    }
}
