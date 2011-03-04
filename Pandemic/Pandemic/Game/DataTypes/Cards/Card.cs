using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Resources;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace Pandemic.Game.DataTypes.Cards
{
    public class Card
    {
        private string title;
        private BitmapImage img;

        public Card(string input)
        {
            title = input;

            try
            {
                img = new BitmapImage(new Uri(Environment.CurrentDirectory + "//Images//virusCards//" + title.Replace(' ', '_').Replace(".", "") + ".png"));
            }
            catch(Exception e)
            {
            }

            //imgSource = "/Pandemic;component/Images/virusCards/" + title.Replace(' ', '_');
        }

        public string Title { get { return title; } }
        public BitmapImage Image { get { return img; } }
    }
}
