using System.IO;
using System.Net;
using System;
using System.Threading;
using Conn = System.Net;
using System.Collections;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net.Sockets;

using Pandemic.DataTypes;

namespace Pandemic.Servers
{
    class lobbyHelper
    {
        Conn.Sockets.TcpClient client;
        System.IO.StreamReader reader;

        int clientId = -1;
        const int intSize = 4;

        public lobbyHelper(Conn.Sockets.TcpClient tcpClient)
        {
            client = tcpClient;

            Thread connThread = new Thread(new ThreadStart(startConn));
            lobbyServ.threads.Add(connThread);
            connThread.Start();
        }

        private void startConn()
        {
            string name;
            byte[] size;

            bool pass = true;

            //create our StreamReader object to read the current stream
            reader = new System.IO.StreamReader(client.GetStream());

            NetworkStream netstream = new NetworkStream(client.Client);

            try
            {
                // Attempt to Join Player to Lobby
                while (clientId == -1)
                {
                    pass = true;
                    // Receive Player Name
                    name = reader.ReadLine();

                    // Check for Exisiting Player Name
                    for (int i = 0; i < 4; i++)
                    {
                        if (lobbyServ.clients[i].nick == name)
                        {
                            netstream.Write(System.BitConverter.GetBytes(-1), 0, intSize);
                            pass = false;
                            break;
                        }
                    }
                    // if valid name find open slot
                    if (pass && name != null)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            if (lobbyServ.clients[i].nick == "" && lobbyServ.curr.slots[i])
                            {
                                lobbyServ.clients[i].nick = name;
                                lobbyServ.clients[i].connInfo = client;

                                lobbyServ.uiName.Add(name, client);
                                lobbyServ.uiNameByConnect.Add(client, name);
                                lobbyServ.idByNick.Add(name, i);

                                lobbyServ.curr.players[i] = name;
                                clientId = i;
                                break;
                            }
                        }

                        // Lobby Full
                        if (clientId == -1)
                            netstream.Write(System.BitConverter.GetBytes(-2), 0, intSize);
                        else
                            break;
                    }
                }

                size = System.BitConverter.GetBytes(clientId);
                netstream.Write(size, 0, intSize);

                lobbyServ.sendUIupdate(null);

                runConn();
            }
            catch (Exception)
            {
                reader.Close();
            }
        }

        // Waits for new client requests
        private void runConn()
        {
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                while (true)
                {
                    NetworkStream netstream = new NetworkStream(client.Client);

                    byte[] size = new byte[4];

                    // Receive packet size
                    netstream.Read(size, 0, intSize);

                    int len = System.BitConverter.ToInt32(size, 0);

                    // Client Signalling Disconnect
                    if (len < 0)
                    {
                        lobbyServ.removePlayer(clientId);
                        return;
                    }

                    byte[] data = new byte[len];                    

                    // Receive Packet
                    netstream.Read(data, 0, len);

                    MemoryStream mem = new MemoryStream(data);

                    object temp = formatter.Deserialize(mem);
                    cont chng = (cont)temp;

                    lobbyServ.sendUIupdate(chng);
                }
            }
            catch (Exception)
            {
                return;
            }
        }
    }
}
