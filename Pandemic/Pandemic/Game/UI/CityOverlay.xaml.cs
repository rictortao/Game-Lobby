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
using System.Drawing.Drawing2D;
using System.Windows.Interop;

using Pandemic.Game.DataTypes;

namespace Pandemic.Game.UI
{
    /// <summary>
    /// Interaction logic for CityOverlay.xaml
    /// </summary>
    public partial class CityOverlay : UserControl
    {
        bool station = false;
        int[] virusArray = new int[] { 0, 0, 0, 0 }; //blue, yellow, black, red

        Image[] virusPicbox;

        private CityDetail details;
        private Storyboard fadein = new Storyboard();
        private Storyboard fadeout = new Storyboard();

        private Point startPoint;

        public CityOverlay()
        {
            InitializeComponent();

            virusPicbox = new Image[] { vBlue, vYellow, vBlack, vRed };            

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
            #endregion

        }

        public void setV(int color, int num)
        {
            virusArray[color] = num;

            virusPicbox[color].Source = virusPics.getImage(color, num);

            details.setV(color, num);
        }

        public void setDetail(CityDetail link)
        {
            details = link;
            details.Overlay = this;

        }

        public void visibleToggle()
        {
            if (this.grid.Opacity == 1)
                fadeout.Begin(this);
            else
            {
                fadein.Begin(this);
                //this.Visibility = Visibility.Visible;
            }
        }

        private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                gameBoard.cleanBoard();
                this.visibleToggle();
                this.details.visibleToggle();                
            }
        }

        public void expand(double scale)
        {
            foreach (Image i in virusPicbox)
            {
                i.Height = i.Height * scale;
                i.Width = i.Width * scale;                           

                i.Margin = new Thickness(i.Margin.Left / scale, i.Margin.Top / scale, 0, 0);
            }
        }

        public void shrink(double scale)
        {
            foreach (Image i in virusPicbox)
            {
                //i.Height = i.Height / scale;
                //i.Width = i.Width / scale;

                i.Margin = new Thickness(i.Margin.Left * scale, i.Margin.Top * scale, i.Margin.Right * scale, i.Margin.Bottom * scale);
            }
        }

        public bool Station
        {
            get { return station; }
            set { station = value; 
                if (station) 
                    imgStation.Visibility = Visibility.Visible; 
                else 
                    imgStation.Visibility = Visibility.Hidden;
                details.setS(station);
            }
        }

        private void pos_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            startPoint = e.GetPosition(null);
        }

        private void pos_MouseMove(object sender, MouseEventArgs e)
        {
            // Get the current mouse position
            Point mousePos = e.GetPosition(null);
            Vector diff = startPoint - mousePos;

            if (e.LeftButton == MouseButtonState.Pressed &&
                Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance &&
                Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)
            {
                // Get the dragged Item

                DataObject dragData = new DataObject();

                DragDrop.DoDragDrop(imgStation, dragData, DragDropEffects.Copy);
            }
        }
    }
}
