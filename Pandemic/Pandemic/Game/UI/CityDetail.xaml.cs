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
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;

namespace Pandemic.Game.UI
{
    /// <summary>
    /// Interaction logic for City.xaml
    /// </summary>
    public partial class CityDetail : UserControl
    {
        private string namec;
        private CityOverlay overlay;

        private Storyboard fadein = new Storyboard();
        private Storyboard fadeout = new Storyboard();

        private Label[] lblArray;

        public CityDetail()
        {
            InitializeComponent();
            this.Visibility = Visibility.Collapsed;

            #region /* Init Animation */
            TimeSpan duration = new TimeSpan(0, 0, 1);

            DoubleAnimation animation = new DoubleAnimation();

            animation.From = 0.0;
            animation.To = 1.0;

            animation.Duration = new Duration(duration);

            Storyboard.SetTargetName(animation, this.grid.Name);
            Storyboard.SetTargetProperty(animation, new PropertyPath(Control.OpacityProperty));

            fadein.Children.Add(animation);

            animation = new DoubleAnimation();

            animation.From = 1.0;
            animation.To = 0.0;

            animation.Duration = new Duration(duration);

            Storyboard.SetTargetName(animation, this.grid.Name);
            Storyboard.SetTargetProperty(animation, new PropertyPath(Control.OpacityProperty));

            fadeout.Children.Add(animation);
            fadeout.Completed +=new EventHandler(fadeout_Completed);
            #endregion
        }

        public void visibleToggle()
        {
            overlay.visibleToggle();

            if (this.Visibility == Visibility.Visible)
                fadeout.Begin(this);
            else
            {
                fadein.Begin(this);
                this.Visibility = Visibility.Visible;
            }
            
        }

        public void setV(int color, int num)
        {
            if(lblArray == null)
                lblArray = new Label[] { lblBlue, lblYellow, lblBlack, lblRed };

            lblArray[color].Content = num;
        }

        public void setS(bool value)
        {
            if (value)
                imgStation.Visibility = Visibility.Visible;
            else
                imgStation.Visibility = Visibility.Hidden;
        }
        
        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)            {
                
                this.visibleToggle();
            }
        }

        private void fadeout_Completed(object sender, EventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }

        public CityOverlay Overlay { set { overlay = value; } }
        public string Namec { get { return namec; } }        

        private void control_Loaded(object sender, RoutedEventArgs e)
        {
            lblArray = new Label[] { lblBlue, lblYellow, lblBlack, lblRed };
        }

        
    }
}
