using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Chat = System.Net;
using Conn = System.Net;
using System.Threading;
using System.Runtime.InteropServices;
using System.Data;
using System.Collections;
using System.IO;
using System.Net.Sockets;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.VisualBasic;

using Pandemic.DataTypes;

namespace Pandemic.UI
{
    /// <summary>
    /// Interaction logic for lobby.xaml
    /// </summary>
    /// 

    delegate void leave();
    delegate void chat(string msg);
    delegate void setPlayer(int playernum);
    delegate void UpdateUI(lobbyVals uidata);

    public partial class lobby : Window
    {
        cont controls = new cont();
        bool ignore = true;

        Mutex mux = new Mutex();
        private Window main;
        static Chat.Sockets.TcpClient tcpChat;
        static Chat.Sockets.TcpClient tcpUI;
        string playerName;
        int playerNum;
        public static bool host = false;
        string hostIP;
        bool closing = false;

        private Control[] player1;
        private Control[] player2;
        private Control[] player3;
        private Control[] player4;

        private Control[][] players;

        public lobby(Window input, string name)
        {
            InitializeComponent();

            main = input;
            playerName = name;
            DataContext = new Data();

            player1 = new Control[4] { lblPlayer1, char1, ready1, clr1 };
            player2 = new Control[4] { lblPlayer2, char2, ready2, clr2 };
            player3 = new Control[4] { lblPlayer3, char3, ready3, clr3 };
            player4 = new Control[4] { lblPlayer4, char4, ready4, clr4 };

            players = new Control[4][] { player1, player2, player3, player4 };

            for (int i = 0; i < 4; i++)
            {
                players[i][1].IsEnabled = true;
            }

            optGrp.IsEnabled = true;
            cmbDifficulty.SelectedIndex = 1;

            host = true;

            hostIP = "127.0.0.1";
            chatInput.KeyUp += new KeyEventHandler(key_up);
            tcpUI = new Chat.Sockets.TcpClient();
            tcpUI.Connect("127.0.0.1", 4297);

            Thread uiThread = new Thread(new ParameterizedThreadStart(uihandler));
            uiThread.Start(this);

            tcpChat = new Chat.Sockets.TcpClient();
            tcpChat.Connect("127.0.0.1", 4296);
        }

        public lobby(Window input, string name, string inhostIp)
        {
            InitializeComponent();

            main = input;
            hostIP = inhostIp;
            playerName = name;
            DataContext = new Data();

            player1 = new Control[4] { lblPlayer1, char1, ready1, clr1 };
            player2 = new Control[4] { lblPlayer2, char2, ready2, clr2 };
            player3 = new Control[4] { lblPlayer3, char3, ready3, clr3 };
            player4 = new Control[4] { lblPlayer4, char4, ready4, clr4 };

            players = new Control[4][] { player1, player2, player3, player4 };

            chatInput.KeyUp += new KeyEventHandler(key_up);
            tcpUI = new Conn.Sockets.TcpClient();

            try
            {
                tcpUI.Connect(hostIP, 4297);
                Thread uiThread = new Thread(new ParameterizedThreadStart(uihandler));
                uiThread.Start(this);

                tcpChat = new Chat.Sockets.TcpClient();
                tcpChat.Connect(hostIP, 4296);
            }
            catch (Exception)
            {
                MessageBox.Show("Connection to host timeout");
                this.Close();
                //this.startClose();
            }            
        }        

        private void uihandler(object input)
        {
            NetworkStream netStream = new NetworkStream(tcpUI.Client);
            StreamReader reader = new StreamReader(tcpUI.GetStream());
            StreamWriter writer = new StreamWriter(tcpUI.GetStream());

            byte[] size;

            bool accepted = false;
            

            while (!accepted)
            {
                #region /* Player Login */
                writer.WriteLine(playerName);
                writer.Flush();

                size = new byte[4];
                netStream.Read(size, 0, 4);
                playerNum = System.BitConverter.ToInt32(size, 0);

                switch (playerNum)
                {
                    case -1: // Name Already Exists
                        MessageBox.Show("Name is Already in Use.");
                        startClose();
                        return;
                    case -2: // Lobby is Full
                        MessageBox.Show("Lobby is Full");
                        startClose();
                        return;
                    default: // Name Accepted
                        accepted = !accepted;
                        break;
                }
            }            

            Thread chatThread = new Thread(new ThreadStart(run));
            chatThread.Start();

            // Disable other players controls
            this.Dispatcher.BeginInvoke(
                        DispatcherPriority.Normal,
                        (setPlayer)delegate(int pnum)
                        {
                            for (int i = 0; i < 4; i++)
                            {
                                if (i != pnum)
                                {
                                    players[i][1].IsEnabled = false;
                                    players[i][2].IsEnabled = false;
                                }
                            }
                        }, playerNum);

            controls.num = playerNum;
            controls.host = lobby.host;
                
            #endregion


            while (true)
            {
                try
                {
                    byte[] data;
                    size = new byte[4];
                    BinaryFormatter formatter = new BinaryFormatter();
                    object temp = null;

                    netStream.Read(size, 0, 4);

                    int len = System.BitConverter.ToInt32(size, 0);

                    data = new byte[len];

                    netStream.Read(data, 0, len);

                    MemoryStream mem = new MemoryStream(data);

                    temp = formatter.Deserialize(mem);

                    lobbyVals uiupdate = (lobbyVals)temp;

                    // Update UI
                    this.Dispatcher.BeginInvoke(
                        DispatcherPriority.Normal,
                        (UpdateUI)delegate(lobbyVals uidata)
                        {
                            try
                            {
                                ignore = true;
                                for (int i = 0; i < 4; i++)
                                {
                                    if (uidata.players[i] != "")
                                    {
                                        (players[i][0] as Label).Content = uidata.players[i];
                                        players[i][1].SetBinding(ComboBox.ItemsSourceProperty, new Binding("IList"));
                                        if (host && i != playerNum)
                                            players[i][1].IsEnabled = false;
                                        
                                    }
                                    else
                                    {
                                        (players[i][0] as Label).Content = "Player " + (i + 1).ToString();
                                        players[i][1].SetBinding(ComboBox.ItemsSourceProperty, new Binding("empty"));
                                        if (host)
                                            players[i][1].IsEnabled = true;
                                    }

                                    (players[i][2] as CheckBox).IsChecked = (bool?)uidata.ready[i];
                                    (players[i][1] as ComboBox).SelectedIndex = uidata.roles[i];

                                    if (uidata.players[i] != "")
                                    {
                                        switch(uidata.roles[i])
                                        {
                                            case 0:
                                                (players[i][3] as Label).Background = new SolidColorBrush(Colors.Transparent);
                                                break;
                                            case 1:
                                                (players[i][3] as Label).Background = new SolidColorBrush(Colors.Pink);
                                                break;
                                            case 2:
                                                (players[i][3] as Label).Background = new SolidColorBrush(Colors.Orange);
                                                break;
                                            case 3:
                                                (players[i][3] as Label).Background = new SolidColorBrush(Colors.White);
                                                break;
                                            case 4:
                                                (players[i][3] as Label).Background = new SolidColorBrush(Colors.Brown);
                                                break;
                                            case 5:
                                                (players[i][3] as Label).Background = new SolidColorBrush(Colors.LimeGreen);
                                                break;
                                        }
                                    }
                                    else
                                        (players[i][3] as Label).Background = new SolidColorBrush(Colors.Transparent);
                                }
                                // Ready Check
                                if ((players[playerNum][2] as CheckBox).IsChecked == true)
                                    players[playerNum][1].IsEnabled = false;
                                else
                                    players[playerNum][1].IsEnabled = true;


                                // Host Updates
                                cmbDifficulty.SelectedIndex = uidata.difficulty;                                
                                ignore = false;
                            }
                            catch (Exception e)
                            {
                                MessageBox.Show(e.Message);
                            }
                        }, uiupdate);
                }
                catch (Exception)
                {
                    if(!host && !closing)
                        MessageBox.Show("Host has Left the Lobby");
                    startClose();
                    return;
                }
            }
        }

        void run()
        {
            //create our StreamReader Object, based on the current NetworkStream
            StreamReader reader = new StreamReader(tcpChat.GetStream());

            string buff;
            string line;
            string method;

            try
            {
                /* Message Processing */
                while (true)
                {
                    buff = reader.ReadLine();
                    method = buff.Substring(0, buff.IndexOf(';')).ToLower();
                    line = buff.Substring(buff.IndexOf(';')+1);

                    switch (method)
                    {
                        case "sig":
                            {
                                StreamWriter writer = new StreamWriter(tcpChat.GetStream());
                                writer.WriteLine(playerName);
                                writer.Flush();
                                break;
                            }                    
                        case "chat":
                            {
                                this.Dispatcher.BeginInvoke(
                                    DispatcherPriority.Normal,
                                    (chat)delegate(string msg)
                                    {
                                        chatBox.AppendText(msg);
                                        chatBox.ScrollToEnd();
                                    }, line + "\r\n");
                                break;
                            }
                    }
                    
                }
            }
            catch(Exception)
            {
                return;
            }
        }

        private void startClose()
        {
            this.Dispatcher.BeginInvoke(
                        DispatcherPriority.Normal,
                        (leave)delegate()
                        {
                            this.Close();
                        });
        }
        private static void key_up(object s, KeyEventArgs e)
        {
            //create our textbox value variable
            TextBox txtChat = (TextBox)s;
            //check to make sure the length of the text
            //in the TextBox is greater than 1 (meaning it has text in it)
            
            if (txtChat.Text.Length > 1 && e.Key == Key.Enter)
            {
                //create a StreamWriter based on the current NetworkStream
                StreamWriter writer = new StreamWriter(tcpChat.GetStream());
                //write our message
                writer.WriteLine(txtChat.Text);
                //ensure the buffer is empty
                writer.Flush();
                //clear the textbox for our next message
                txtChat.Text = "";
            }
        }

        public class Data
        {
            public Data()
            {
                IList = new List<string>();
                IList.Add("Random");
                IList.Add("Dispatcher");
                IList.Add("Medic");
                IList.Add("Scientist");
                IList.Add("Researcher");
                IList.Add("Operations Expert");

                empty = new List<string>();
                empty.Add("Open");
                empty.Add("Closed");

            }
            public List<string> IList { get; set; }
            public List<string> empty { get; set; }
        }        

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            closing = true;
            try
            {
                tcpChat.Close();
                tcpUI.Close();                
            }
            catch (Exception)
            {
            }
            main.Visibility = Visibility.Visible;
        }

        private void btnDscnt_Click(object sender, RoutedEventArgs e)
        {
            closing = true;
            
            NetworkStream netstream = new NetworkStream(tcpUI.Client);

            if(!host)
                netstream.Write(System.BitConverter.GetBytes(-1), 0, 4);
            else
                netstream.Write(System.BitConverter.GetBytes(-2), 0, 4);

            tcpUI.Close();
            tcpChat.Close();

            this.Close();
        }

        private void ready1_Click(object sender, RoutedEventArgs e)
        {
            CheckBox chk = (sender as CheckBox);

            controls.chk = (bool)chk.IsChecked;

            sendUpdate();
        }

        private void char_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!ignore)
            {
                if ((players[playerNum][1] as Control).Name != (sender as Control).Name)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        if ((players[i][1] as Control).Name == (sender as Control).Name)
                        {
                            controls.chgEmpty = true;
                            controls.slot = i;
                            break;
                        }
                    }
                }

                controls.role = (sender as ComboBox).SelectedIndex;

                sendUpdate();
            }
        }

        private void cmbDifficulty_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!ignore)
            {       
                controls.difficulty = (sender as ComboBox).SelectedIndex;
                sendUpdate();
            }
        }

        private void sendUpdate()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream mem = new MemoryStream();
            formatter.Serialize(mem, controls);

            byte[] data = mem.GetBuffer();

            NetworkStream netstream = new NetworkStream(tcpUI.Client);

            byte[] size = System.BitConverter.GetBytes(data.Length);
            netstream.Write(size, 0, size.Length);

            netstream.Write(data, 0, data.Length);

            netstream.Flush();

            controls.chgEmpty = false;            
        }
        
    }
}
