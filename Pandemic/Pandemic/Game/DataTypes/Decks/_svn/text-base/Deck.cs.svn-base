using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Pandemic.Game.DataTypes.Cards;

namespace Pandemic.Game.DataTypes.Decks
{
    class Deck
    {
        public List<Card> cards = new List<Card>();

        public Deck()
        {
        }

        public void Shuffle()
        {
            Random rnd = new Random();
            int k;

            for (int i = cards.Count - 1; i > 1; i--)
            {
                k = rnd.Next(i + 1);
                Card value = cards[k];
                cards[k] = cards[i];
                cards[i] = value;
            }
        }

        /* 
         * If Cards in Deck return top Card and remove from deck
         * otherwise return null
         */
        public Card Draw()
        {
            if (cards.Count > 0)
            {
                Card rtn = cards[0];
                cards.Remove(cards[0]);
                return rtn;
            }
            else
                return null;
        }
    }
}
