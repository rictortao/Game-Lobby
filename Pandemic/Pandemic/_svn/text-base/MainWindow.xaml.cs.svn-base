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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Pandemic.UI;
using System.Threading;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using Pandemic.Lobby.DataTypes;
using Pandemic.Game.UI;

namespace Pandemic
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Thread th;

        public MainWindow()
        {
            InitializeComponent();

            txtName.Text = readConfig("defaultname");
            ipConnect.Text = readConfig("defaultip");

            this.txtName.AddHandler(Control.MouseDownEvent,
                new MouseButtonEventHandler(TextBox_MouseDown), true);
            this.ipConnect.AddHandler(Control.MouseDownEvent,
                new MouseButtonEventHandler(TextBox_MouseDown), true);

            try
            {
                FileStream input = new FileStream("defaults",
                    FileMode.Open, FileAccess.Read);

                BinaryFormatter formatter = new BinaryFormatter();

                dvals defaults = (dvals)formatter.Deserialize(input);

                txtName.Text = defaults.playerName;
                ipConnect.Text = defaults.hostIP;

                input.Close();
            }
            catch (Exception)
            {
            }
        }

        private void btnMulti_Click(object sender, RoutedEventArgs e)
        {
            main.Visibility = Visibility.Hidden;
            multi.Visibility = Visibility.Visible;
        }

        private void btnSingle_Click(object sender, RoutedEventArgs e)
        {
            //main.Visibility = Visibility.Hidden;

            lobbyVals test = new lobbyVals();

            test.difficulty = 2;
            test.players[0] = "Dave";
            test.roles[0] = 3;
            test.players[1] = "Bob";
            test.roles[1] = 2;

            test.slots[2] = false;
            test.slots[3] = false;

            //Game.game gameTest = new Game.game(test);
            gameBoard board = new gameBoard();
            board.Show();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            multi.Visibility = Visibility.Hidden;
            main.Visibility = Visibility.Visible;            
        }

        private void btnHost_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;

            th = new Thread(serverStart);
            th.Start();

            lobby mLobby = new lobby(this, txtName.Text);
            mLobby.Show();
        }
        
        private void btnJoin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                lobby mLobby = new lobby(this, txtName.Text, ipConnect.Text);
                mLobby.Show();
            }
            catch (Exception)
            {
            }
        }

        static void serverStart()
        {
            Pandemic.Servers.lobbyServ gameServ = new Pandemic.Servers.lobbyServ();
        }

        private string readConfig(string id)
        {   
            try
            {
                TextReader tr = new StreamReader("config.dat");
                string line;
                int div;
                string val;

                while ((line = tr.ReadLine()) != null)
                {
                    div = line.IndexOf(":");
                    val = line.Substring(div + 1);
                    if(line.Substring(0, div) == id)
                        return val;
                }
                return "";
            }
            catch (Exception)
            {
                return "";
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            dvals defaults = new dvals();

            BinaryFormatter formatter = new BinaryFormatter();
            FileStream output;

            defaults.playerName = txtName.Text;
            defaults.hostIP = ipConnect.Text;

            output = new FileStream("defaults",
                FileMode.OpenOrCreate,
                FileAccess.Write);
            formatter.Serialize(output, defaults);
            output.Close();
        }

        /* TextBox Controls */

        private void TextBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if ((sender as TextBox).Text.Length > 1)
            {
                (sender as TextBox).Focus();
                (sender as TextBox).SelectAll();
            }
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
