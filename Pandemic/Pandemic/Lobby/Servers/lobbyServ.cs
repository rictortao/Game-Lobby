using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Chat = System.Net;
using Conn = System.Net;
using System.Net.Sockets;
using System.Collections;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;

using Pandemic.DataTypes;
using Pandemic.Servers;

namespace Pandemic.Servers
{
    class lobbyServ
    {
        static int hostnum = 0;
        static bool running;

        private static Mutex mux = new Mutex();

        public static client[] clients;

        const int lobbyPort = 4297;
        const int chatPort = 4296;

        public static lobbyVals curr = new lobbyVals();

        public static Hashtable nickName;
        public static Hashtable nickNameByConnect;

        public static Hashtable uiName;
        public static Hashtable uiNameByConnect;

        public static Hashtable idByNick;

        static System.Net.Sockets.TcpListener lobbyServer;
        static System.Net.Sockets.TcpListener chatServer;

        public static List<Thread> threads;

        public lobbyServ()
        {
            running = true;

            nickName = new Hashtable(4);
            nickNameByConnect = new Hashtable(4);
            uiName = new Hashtable(4);
            uiNameByConnect = new Hashtable(4);
            idByNick = new Hashtable(4);

            threads = new List<Thread>();

            Thread chatThread = new Thread(new ThreadStart(chatServ));
            chatThread.Start();

            clients = new client[4] { new client(), new client(), new client(), new client()};

            lobbyServer = new Conn.Sockets.TcpListener(Conn.IPAddress.Any, lobbyPort);

            while (running)
            {
                // Start lobby server
                lobbyServer.Start();
                // Check for pending connections
                if (lobbyServer.Pending())
                {
                    // If pending connection create new connection
                    Conn.Sockets.TcpClient clientConnection = lobbyServer.AcceptTcpClient();
                    lobbyHelper newHelper = new lobbyHelper(clientConnection);
                }
            }

            lobbyServer.Stop();
        }

        public void chatServ()
        {
            chatServer = new Chat.Sockets.TcpListener(Chat.IPAddress.Any, chatPort);

            while (running)
            {
                chatServer.Start();

                if (chatServer.Pending())
                {
                    Chat.Sockets.TcpClient chatConnection = chatServer.AcceptTcpClient();
                    chatHelper newHelper = new chatHelper(chatConnection);
                }
            }
            chatServer.Stop();
        }

        // Process Client Requests, sends lobby-state updates to all connected clients
        public static void sendUIupdate(cont request)
        {
            mux.WaitOne();

            // Update Curr
            if (request != null)
            {
                int id = request.num;                

                // If Client is Host
                if (request.host)
                {                   
                    curr.difficulty = request.difficulty;
                }

                // If a state change on empty slot occured
                if (request.chgEmpty)
                {
                    curr.roles[request.slot] = request.role;
                    if (request.role == 1) // Closing slot
                        curr.slots[request.slot] = false;
                    else
                        curr.slots[request.slot] = true;
                }
                // if client requests a role change that is non-random
                else if (curr.roles[id] != request.role && request.role > 0)
                {
                    bool deny = false;
                    for (int i = 0; i < 4; i++)
                    {
                        if (curr.roles[i] == request.role)
                        {
                            deny = true;
                            break;
                        }
                    }
                    if(!deny)
                        curr.roles[id] = request.role;
                }
                else
                    curr.roles[id] = request.role;

                curr.ready[id] = request.chk;                
            }

            Conn.Sockets.TcpClient[] TcpClient = new Conn.Sockets.TcpClient[lobbyServ.uiName.Count];

            lobbyServ.uiName.Values.CopyTo(TcpClient, 0);

            BinaryFormatter formatter = new BinaryFormatter();

            for (int i = 0; i < TcpClient.Length; i++)
            {
                try
                {
                    NetworkStream netstream = new NetworkStream(TcpClient[i].Client);

                    formatter = new BinaryFormatter();
                    MemoryStream mem = new MemoryStream();

                    formatter.Serialize(mem, curr);

                    byte[] bytData = mem.GetBuffer();
                    byte[] size = System.BitConverter.GetBytes(bytData.Length);

                    netstream.Write(size, 0, size.Length);
                    netstream.Write(bytData, 0, bytData.Length);
                    netstream.Flush();
                }
                catch(Exception)
                {
                    //MessageBox.Show(e.Message);
                }
            }
            mux.ReleaseMutex();
        }

        public static void SendMsgToAll(string nick, string msg)
        {
            mux.WaitOne();
            //create a StreamWriter Object
            StreamWriter writer;
            ArrayList ToRemove = new ArrayList(0);
            //create a new TCPClient Array
            Chat.Sockets.TcpClient[] tcpClient = new Chat.Sockets.TcpClient[Pandemic.Servers.lobbyServ.nickName.Count];
            //copy the users nickname to the CHatServer values
            Pandemic.Servers.lobbyServ.nickName.Values.CopyTo(tcpClient, 0);
            //loop through and write any messages to the window
            for (int cnt = 0; cnt < tcpClient.Length; cnt++)
            {
                try
                {
                    //check if the message is empty, of the particular
                    //index of out array is null, if it is then continue
                    if (msg == null || tcpClient[cnt] == null)
                        continue;
                    //Use the GetStream method to get the current memory
                    //stream for this index of our TCPClient array
                    writer = new StreamWriter(tcpClient[cnt].GetStream());
                    //white our message to the window

                    writer.WriteLine("chat;" + nick + ": " + msg);

                    //make sure all bytes are written
                    writer.Flush();
                    //dispose of the writer object until needed again
                    writer = null;
                }
                //here we catch an exception that happens
                //when the user leaves the chatroow
                catch (Exception)
                {
                    return;
                }
            }
            mux.ReleaseMutex();
        }

        public static void SendSystemMessage(string msg)
        {
            //create our StreamWriter object
            StreamWriter writer;
            ArrayList ToRemove = new ArrayList(0);
            //create our TcpClient array
            Chat.Sockets.TcpClient[] tcpClient = new Chat.Sockets.TcpClient[lobbyServ.nickName.Count];
            //copy the nickname value to the chat servers list
            lobbyServ.nickName.Values.CopyTo(tcpClient, 0);
            //loop through and write any messages to the window
            for (int i = 0; i < tcpClient.Length; i++)
            {
                try
                {
                    //check if the message is empty, of the particular
                    //index of out array is null, if it is then continue
                    if (msg.Trim() == "" || tcpClient[i] == null)
                        continue;
                    //Use the GetStream method to get the current memory
                    //stream for this index of our TCPClient array
                    writer = new StreamWriter(tcpClient[i].GetStream());
                    //send our message
                    writer.WriteLine("chat;" + msg);
                    //make sure the buffer is empty
                    writer.Flush();
                    //dispose of our writer
                    writer = null;
                }
                catch (Exception)
                {
                    return;
                }
            }
        }

        public static void removePlayer(int id)
        {
            string str = clients[id].nick;
            TcpClient UIclient = clients[id].connInfo;
            TcpClient chatClient = clients[id].chatInfo;

            if (id == hostnum)
            {
                curr.players[id] = "";
                curr.ready[id] = false;
                curr.roles[id] = 0;

                running = false;

                foreach (client c in clients)
                {
                    c.connInfo.Close();
                    c.chatInfo.Close();
                }

                foreach (Thread th in threads)
                {
                    th.Abort();
                }

                //remove the nickname from the list
                lobbyServ.nickName.Remove(str);
                //remove that index of the array, thus freeing it up
                //for another user
                nickNameByConnect.Remove(chatClient);
                uiNameByConnect.Remove(UIclient);
                uiName.Remove(str);
                idByNick.Remove(str);           
            }
            else
            {
                clients[id].nick = "";
                clients[id].connInfo = null;
                clients[id].chatInfo = null;

                lobbyServ.SendSystemMessage("** " + str + " ** Has Left The Lobby.");

                curr.players[id] = "";
                curr.ready[id] = false;
                curr.roles[id] = 0;

                //remove the nickname from the list
                lobbyServ.nickName.Remove(str);
                //remove that index of the array, thus freeing it up
                //for another user
                nickNameByConnect.Remove(chatClient);
                uiNameByConnect.Remove(UIclient);
                uiName.Remove(str);
                idByNick.Remove(str);

                sendUIupdate(null);
            }
        }
    }
}
