using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace WordFamilies
{
    public class Game
    {
        //If Y, show wordfamily count. If N, don't show wordfamily count.
        static char displayListToUser { get; set; }
        //Store number of guesses left.
        static int guesses { get; set; }
        //If Y then play the game again. If N then stop the game.
        public char playAgain { get; set; }
        //Store word length from user.
        static int wordLength { get; set; }
        //Store all words from dictionary.txt to array.
        static String[] wordArray { get; set; } = File.ReadAllLines("dictionary.txt");
        //This list holds the best wordfamily that was chosen.
        static List<string> primaryWordList { get; set; } = new List<string>();
        //Store letters which user has already guessed.
        static List<char> usedLettersList { get; set; } = new List<char>();
        //Store all words into their respective wordfamily.
        static List<List<string>> listOfLists { get; set; } = new List<List<string>>();
        //Store positions of letter that user has guessed in a word.
        static List<int> listOfPositionNumbers { get; set; } = new List<int>();
        //Store the hidden word.
        static List<char> lettersInUserGuesses { get; set; } = new List<char>();
        //Store random word from primarywordlist.
        string word { get; set; }
        static bool firstTime { get; set; }


        static AI ai { get; set; } = new AI();

        public void MainMenu()
        {
            Console.Clear();
            PromptUserWordLength();
            PromptUserShowList();
            PromptUserGuesses();
            //Get all words with certain wordlegnth and store in primarylist.
            ai.CreateWordList(wordLength, wordArray, primaryWordList);
        }

        public void StartGame()
        {
            //Reset every game.
            firstTime = true;
            do
            {
                ShowStats();
                Console.WriteLine();
                //If firstTime == true then show hidden word and add '_' to create hidden word list. 
                if (firstTime == true)
                {
                    for (int i = 0; i < wordLength; i++)
                    {
                        Console.Write('_');
                        lettersInUserGuesses.Add('_');
                        firstTime = false;
                    }
                }
                else
                {
                    //Show blanked word.
                    ai.ShowHiddenWord(lettersInUserGuesses);
                }
                Console.WriteLine();
                //Get letter from user.
                ai.PromptUserLetter(usedLettersList);
                //Get all words and create list that doesn't contain the letter user has guessed and store it in primarywordlist.
                primaryWordList = ai.CreateWordListWithoutLetter(primaryWordList, listOfLists).ToList();
                //Get all words and create list that does contain the letter user has guessed and store it in primarywordlist.
                primaryWordList = ai.CreateWordListWithLetter(primaryWordList, listOfPositionNumbers, listOfLists, lettersInUserGuesses, usedLettersList).ToList();
                //Choose best wordfamily.
                primaryWordList = ai.ChooseBestWordFamily(listOfLists, primaryWordList, guesses).ToList();
                //Choose word.
                word = ai.ChooseWord(primaryWordList, word);
                Console.WriteLine();
                //Deterine if user guess is right or wrong.
                guesses = ai.DetermineGuesses(word, guesses, lettersInUserGuesses);
                Console.WriteLine();
                //Determine if user has won or lost the game.
                if (!lettersInUserGuesses.Contains('_'))
                {
                    Console.WriteLine("You won! The word was: {0}", word);
                    break;
                }
                if (guesses == 0)
                {
                    Console.WriteLine("You lose! The word was: {0}", word);
                    break;
                }
                Console.WriteLine();
            }
            while (true);
            //Clear all lists for next game.
            primaryWordList.Clear();
            usedLettersList.Clear();
            listOfLists.Clear();
            listOfPositionNumbers.Clear();
            lettersInUserGuesses.Clear();

            //Ask user if they want to play again.
            do
            {
                Console.WriteLine("Play again? (Y/N)");
                playAgain = Console.ReadKey().KeyChar;
            }
            while (playAgain != 'Y' && playAgain != 'y' && playAgain != 'N' && playAgain != 'n');

        }
        public static void ShowStats()
        {
            //Show number of guesses remaining.
            if (guesses > 1)
            {
                Console.WriteLine("You have {0} guesses remaining.", guesses);
            }
            else
            {
                Console.WriteLine("You have {0} guess remaining.", guesses);
            }
            //Show letters that user has guessed.
            if (!usedLettersList.Any())
            {
                Console.WriteLine("Used letters: ");
            }
            else
            {
                Console.Write("Used letters: ");

                usedLettersList.Sort();

                for (int i = 0; i < usedLettersList.Count; i++)
                {
                    Console.Write(usedLettersList[i] + " ");
                }
                Console.WriteLine();
            }
            //Display number of words in wordfamily if user answered Y.
            if (displayListToUser == 'Y' || displayListToUser == 'y')
            {
                if (primaryWordList.Count > 1)
                {
                    Console.WriteLine(primaryWordList.Count + " Words remaining.");
                }
                else
                {
                    Console.WriteLine(primaryWordList.Count + " Word remaining.");
                }

            }

        }

        public static void PromptUserWordLength()
        {
            do
            {
                if (ai.CheckWordLength(wordLength, wordArray) == 2)
                {
                    Console.WriteLine("Please try again");
                }
                Console.Write("Enter word length: ");
                try
                {
                    wordLength = (int)Convert.ToUInt32(Console.ReadLine());
                }
                catch (FormatException)
                {
                    Console.WriteLine("Only enter numbers please!");
                }
                catch (OverflowException)
                {
                    Console.WriteLine("There are no words that long!");
                }
            }
            while (ai.CheckWordLength(wordLength, wordArray) != 1);
            Console.WriteLine();
        }

        public static void PromptUserShowList()
        {
            do
            {
                Console.WriteLine();
                Console.Write("Would you like me to display the number of words remaining in the word list? (Y/N): ");
                displayListToUser = Console.ReadKey().KeyChar;
            }
            while (displayListToUser != 'Y' && displayListToUser != 'y' && displayListToUser != 'N' && displayListToUser != 'n');

            if (displayListToUser == 'Y' || displayListToUser == 'y')
            {
                Console.WriteLine("\nOk. I will display the number of words remaining in the word list.");
            }
            else if (displayListToUser == 'N' || displayListToUser == 'n')
            {
                Console.WriteLine();
                Console.WriteLine("Playing in normal mode");
            }
            Console.WriteLine();
        }

        public static void PromptUserGuesses()
        {
            do
            {
                try
                {
                    Console.Write("How many guesses would you like?: ");
                    guesses = (int)Convert.ToUInt32(Console.ReadLine());
                }
                catch (FormatException)
                {
                    Console.WriteLine("Only enter numbers please!");
                }
                Console.WriteLine();
            }
            while (guesses < 1);
        }
    }
}
