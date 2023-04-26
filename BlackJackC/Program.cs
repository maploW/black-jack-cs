using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace BlackJackC
{
    internal class Program
    {
        static readonly string[] Options = new[] {"Start Game", "Stats", "Instructions", "Exit"};
        
        static List<string> playerCards = new List<string>();
        static List<string> dealerCards = new List<string>();

        static readonly string filePath = @"C:\Users\Public\Documents\BlackJackUserData.txt";

        static int playerWins = 0;
        static int playerLosses = 0;
        static int playerTies = 0;

        static Deck cardDeck = new Deck();

        static void Main()
        {
            if (new FileInfo(filePath).Length > 0)
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string userData = reader.ReadToEnd();
                    string[] parts = userData.Split(',');
                    playerWins = int.Parse(parts[0]);
                    playerLosses = int.Parse(parts[1]);
                    playerTies = int.Parse(parts[2]);
                }
            }
            
            do
            {
                int previousLineIndex = -1, selectedLineIndex = 0;
                ConsoleKey pressedKey;
                do
                {
                    if (previousLineIndex != selectedLineIndex)
                    {
                        UpdateMenu(selectedLineIndex);
                        previousLineIndex = selectedLineIndex;
                    }
                    pressedKey = Console.ReadKey().Key;

                    if (pressedKey == ConsoleKey.DownArrow && selectedLineIndex + 1 < Options.Length)
                    {
                        selectedLineIndex++;
                    }                   
                    else if (pressedKey == ConsoleKey.UpArrow && selectedLineIndex - 1 >= 0)
                    {
                        selectedLineIndex--;
                    }

                } while (pressedKey != ConsoleKey.Enter && pressedKey != ConsoleKey.RightArrow);

                switch (selectedLineIndex)
                {
                    case 0:
                        Console.Clear();
                        cardDeck.Shuffle();
                        StartGame();
                        SaveStatsToFile();
                        Console.WriteLine("\nPress any key to return to the main menu...");
                        Console.ReadKey();
                        break;
                    case 1:
                        Console.Clear();
                        DisplayStats();
                        Console.WriteLine("\nPress any key to return to the main menu...");
                        Console.ReadKey();
                        break;
                    case 2:
                        Console.Clear();
                        PrintGameInstructions();
                        Console.WriteLine("\nPress any key to return to the main menu...");
                        Console.ReadKey();
                        break;
                    case 3:
                        Environment.Exit(1);
                        break;
                }
            } while (true);
        }
        static void UpdateMenu(int index)
        {
            Console.Clear();
            Console.WriteLine("-BlackJack-\n");
            foreach (var option in Options)
            {
                bool isSelected = option == Options[index];
                if (isSelected)
                    PrintInverseColors($"> {option}");
                else
                    Console.WriteLine($"  {option}");
            }
        }
        static void SaveStatsToFile()
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine("{0},{1},{2}", playerWins, playerLosses, playerTies);
            }
        }

        static void StartGame()
        {
            playerCards.Clear();
            dealerCards.Clear();

            cardDeck.DealCards(playerCards, 2);
            cardDeck.DealCards(dealerCards, 2);

            int playerCardTotal = CalculateHandTotal(playerCards);
            int dealerCardTotal = CalculateHandTotal(dealerCards);

            do
            {   
                PrintInverseColors("Player's cards:");

                foreach (string card in playerCards)
                {
                    Console.WriteLine(card);
                }                
                Console.WriteLine("Total card value: " + playerCardTotal);

                PrintInverseColors("Dealer's card:");

                Console.WriteLine(dealerCards[0]);
                if (dealerCards[0] == "Ace")
                {
                    Console.WriteLine("Card value: 1/11");
                }
                else
                {
                    Console.WriteLine("Card value: " + CalculateHandTotal(dealerCards.Take(1).ToList()));
                }

                Console.WriteLine("\nDo you want to hit (h) or stand (s)?");
                string hitOrStandOption = Console.ReadLine().ToLower().Trim();

                if (hitOrStandOption == "h")
                {
                    cardDeck.DealCards(playerCards, 1);
                    playerCardTotal = CalculateHandTotal(playerCards);
                }
                else if (hitOrStandOption == "s")
                {
                    break;
                }

            } while (playerCardTotal < 21);

            while (dealerCardTotal < 17 && playerCardTotal <= 21)
            {
                cardDeck.DealCards(dealerCards, 1);
                Console.WriteLine("\nThe dealer hits. They get: " + dealerCards.Last());
                dealerCardTotal = CalculateHandTotal(dealerCards);
            }

            PrintHands(playerCardTotal, dealerCardTotal);
            Console.WriteLine(DetermineWinner(playerCardTotal, dealerCardTotal));
        }

        static void PrintGameInstructions()
        {
            Console.WriteLine("You will be dealt two cards. The dealer will also be dealt two cards, but one of them will be face down.");
            Console.WriteLine("\nThe goal is to have a higher hand value than the dealer without going over 21.");
            Console.WriteLine("\nYou have the option to \"hit\" and receive another card, or \"stand\" and keep your current hand.");
            Console.WriteLine("\nIf your hand exceeds 21, you \"bust\" and automatically lose the game.");
            Console.WriteLine("\nOnce you have chosen to stand, the dealer will reveal their face down card and hit until their hand reaches a value of 17 or higher.");
            Console.WriteLine("\nIf the dealer busts, you automatically win the game. If not, the values of your hands will be compared, and the player with the higher value wins.");
            Console.WriteLine("\nAces will have a value of 11 unless they're going to make the player/dealer bust, in which case they're worth 1.");
            Console.WriteLine("\nFace cards (Kings, Queens, and Jacks) have a value of 10 and all other cards are worth their face value.");
        }

        static string DetermineWinner(int playerCardTotal, int dealerCardTotal)
        {
            if (playerCardTotal > 21)
            {
                playerLosses++;
                return "\nBust! You lose.";
            }
            else if (dealerCardTotal > 21)
            {
                playerWins++;
                return "\nDealer busts! You win.";
            }
            else if (playerCardTotal > dealerCardTotal)
            {
                playerWins++;
                return "\nYou win!";
            }
            else if (playerCardTotal < dealerCardTotal)
            {
                playerLosses++;
                return "\nYou lose.";
            }
            else
            {
                playerTies++;
                return "\nYou tied.";
            }
        }

        static void PrintHands(int playerCardTotal, int dealerCardTotal)
        {
            PrintInverseColors("Player's cards:");
            foreach (string card in playerCards)
            {
                Console.WriteLine(card);
            }
            Console.WriteLine("Total value: " + playerCardTotal);

            PrintInverseColors("\nDealer's cards:");
            foreach (string card in dealerCards)
            {
                Console.WriteLine(card);
            }
            Console.WriteLine("Total value: " + dealerCardTotal);
        }

        static int CalculateHandTotal(List<string> hand)
        {
            int total = 0;
            int numAces = 0;

            foreach (string card in hand)
            {
                string rank = card.Split()[0];

                if (rank == "Ace")
                {
                    numAces++;
                    total += 11;
                }
                else if (rank == "Jack" || rank == "Queen" || rank == "King")
                {
                    total += 10;
                }
                else
                {
                    total += int.Parse(rank);
                }
            }

            while (total > 21 && numAces > 0)
            {
                total -= 10;
                numAces--;
            }

            return total;
        }

        static void PrintInverseColors(string inputToInverseColor)
        {
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine(inputToInverseColor);
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
        }

        static void DisplayStats()
        {
            Console.WriteLine("Wins: " + playerWins);
            Console.WriteLine("Losses: " + playerLosses);
            Console.WriteLine("Ties: " + playerTies);
            if (playerWins + playerLosses + playerTies > 0)
            {
                Console.WriteLine("Winrate: {0:F2}%", (double)playerWins / (playerWins + playerLosses + playerTies) * 100);
            }
            else
            {
                Console.WriteLine("Winrate: No games played");
            }
        }
    }
}
