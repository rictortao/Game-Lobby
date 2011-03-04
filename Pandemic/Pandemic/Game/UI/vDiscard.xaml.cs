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

using Pandemic.Game.DataTypes;
using Pandemic.Game.DataTypes.Decks;
using Pandemic.Game.DataTypes.Cards;

namespace Pandemic.Game.UI
{
    /// <summary>
    /// Interaction logic for vDiscard.xaml
    /// </summary>
    public partial class vDiscard : Window
    {
        public vDiscard(List<Card> Deck)
        {
            InitializeComponent();

            List<Image> images = new List<Image>();

            //virusDeck Deck = Game.vDiscard;

            foreach (Card c in Deck)
            {
                Image temp = new Image();
                temp.Source = c.Image;
                images.Add(temp);
            }

            cardCarousel.ItemsSource = images;

            this.cardCarousel.ReflectionSettings.Visibility = Visibility.Visible;
            this.cardCarousel.ReflectionSettings.HiddenPercentage = .55;
        }

        private void cardCarousel_Loaded(object sender, RoutedEventArgs e)
        {
            this.cardCarousel.BringDataItemIntoView(cardCarousel.Items[0]);
        }
    }
}
