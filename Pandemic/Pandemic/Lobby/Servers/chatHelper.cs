using System.IO;
using System.Net;
using System;
using System.Threading;
using Chat = System.Net;
using System.Collections;

namespace Pandemic.Servers
{
    class chatHelper
    {
        Chat.Sockets.TcpClient client;
        System.IO.StreamReader reader;
        System.IO.StreamWriter writer;

        int clientId;
        string name;

        public chatHelper(Chat.Sockets.TcpClient tcpClient)
        {
            client = tcpClient;

            Thread chatThread = new Thread(new ThreadStart(startChat));
            chatThread.Start();
        }

        private void startChat()
        {
            try
            {
                //Thread.Sleep(1000);
                bool done = false;
                //create our StreamReader object to read the current stream
                reader = new System.IO.StreamReader(client.GetStream());
                //create our StreamWriter objec to write to the current stream
                writer = new System.IO.StreamWriter(client.GetStream());

                writer.WriteLine("sig;");
                writer.Flush();
                // Gets User Name
                name = reader.ReadLine();

                // Assign chat info based on matching nicks
                // Store Player Id
                while (!done)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        if (lobbyServ.clients[i].nick == name)
                        {
                            lobbyServ.clients[i].chatInfo = client;
                            Pandemic.Servers.lobbyServ.nickName.Add(name, client);
                            Pandemic.Servers.lobbyServ.nickNameByConnect.Add(client, name);

                            clientId = i;
                            done = true;
                            break;
                        }
                    }
                }

                lobbyServ.SendSystemMessage("** " + name + " ** Has joined the lobby");
                runChat();
            }
            catch (Exception)
            {                
            }
        }

        private void runChat()
        {
            try
            {
                string line = "";
                while (true)
                {
                    line = reader.ReadLine();
                    if (line == null)
                        return;
                    Pandemic.Servers.lobbyServ.SendMsgToAll(name, line);
                }
            }
            catch (Exception)
            {
                return;
            }
        }
    }
}
