using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJackC
{
    class Deck
    {
        private List<string> deck = new List<string>();
        
        int deckIndex = 0;
        public Deck()
        {
            string[] suits = { "Hearts", "Diamonds", "Clubs", "Spades" };
            string[] ranks = { "Ace", "2", "3", "4", "5", "6", "7", "8", "9", "10", "Jack", "Queen", "King" };

            foreach (string suit in suits)
            {
                foreach (string rank in ranks)
                {
                    deck.Add(rank + " of " + suit);
                }
            }
        }

        public void Shuffle()
        {
            deckIndex = 0;
            Random rand = new Random();
            for (int i = 0; i < deck.Count; i++)
            {
                int j = rand.Next(i, deck.Count);
                (deck[j], deck[i]) = (deck[i], deck[j]);
            }
        }

        public void DealCards(List<string> hand, int numCards)
        {
            for (int i = 0; i < numCards; i++)
            {
                hand.Add(deck[deckIndex++]);
            }
        }
    }
}
