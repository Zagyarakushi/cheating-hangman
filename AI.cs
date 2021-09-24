using System.Collections.Generic;
using System;
using System.Linq;
using System.Text.RegularExpressions;


namespace WordFamilies
{
    class AI
    {
        // If word if available then true if not then false.
        static int wordAvailable { get; set; }
        //Store the letter that user has guessed.
        static char letter { get; set; }
        //Store the number of guessed letter that matches with the position of the word in wordfamily.
        static int primaryWordPosition { get; set; }
        //Store the number of guessed letter that matches in primaryword.
        static int primaryWordCount { get; set; }
        //Store counters for listOfList and number of word count in a wordfamily etc.
        static int counter { get; set; }
        //Store number of words that were added to a wordfamily.
        static int countNumberPrimaryList { get; set; }
        //Store the position of listOfList.
        static int position { get; set; }
        //Store expression.
        static string regexPattern { get; set; }
        //store temporary expression.
        static string tempPattern { get; set; }
        static bool firstTime { get; set; }
        static Random random { get; set; } = new Random();
        static Regex regex { get; set; }


        public char PromptUserLetter(List<char> usedLettersList)
        {
            do
            {
                Console.WriteLine();
                Console.Write("Enter Guess: ");
                letter = Console.ReadKey().KeyChar;
                Console.WriteLine();
                if (!usedLettersList.Contains(letter) && char.IsLetter(letter))
                {
                    usedLettersList.Add(letter);
                    break;
                }
                else if (!char.IsLetter(letter))
                {
                    Console.WriteLine("Only enter alphabets please!");
                }
                else if (usedLettersList.Contains(letter) && char.IsLetter(letter))
                {
                    Console.WriteLine("You have already used this letter!");
                }
            }
            while (true);

            return letter;
        }

        //check word length in the array and if the word exist then return 1 if not return 2.
        public int CheckWordLength(int wordLength, string[] wordArray)
        {
            for (int i = 0; i < wordArray.Length; i++)
            {
                if (wordArray[i].Length == wordLength)
                {
                    wordAvailable = 1;
                    //Reset stuff here every game/round.
                    firstTime = true;
                    break;
                }
                else if (wordArray[i].Length != wordLength && wordLength > 0)
                {
                    wordAvailable = 2;
                }
            }
            return wordAvailable;
        }

        //Add all words that match wordlength to primarywordlist.
        public void CreateWordList(int wordLength, string[] wordArray, List<string> primaryWordList)
        {
            for (int i = 0; i < wordArray.Length; i++)
            {
                if (wordArray[i].Length == wordLength)
                {
                    primaryWordList.Add(wordArray[i]);
                }
            }
        }

        //Create a wordfamily that doesnt contain the guessed letter.
        public List<string> CreateWordListWithoutLetter(List<string> primaryWordList, List<List<string>> listOfLists)
        {
            //Add a new list to listOfList
            listOfLists.Add(new List<string>());
            //Reset every round.
            countNumberPrimaryList = 0;

            //Go through all words and add words that doesn't contain guessed letter to wordfamily.
            for (int i = 0; i < primaryWordList.Count; i++)
            {
                if (!primaryWordList[i].Contains(letter))
                {
                    listOfLists[0].Add(primaryWordList[i]);
                    countNumberPrimaryList++;
                }
            }
            if (countNumberPrimaryList != 0)
            {
                //Delete all words that were added to new wordfamily and keep all the other words.
                primaryWordList = primaryWordList.Except(listOfLists[0]).ToList();
            }
            return primaryWordList;
        }

        public List<string> CreateWordListWithLetter(List<string> primaryWordList, List<int> listOfPositionNumbers,
            List<List<string>> listOfLists, List<char> lettersInUserGuesses, List<char> usedLettersList)
        {
            CreateRegexPattern(lettersInUserGuesses);

            //Check if the first wordfamily contain words without guessed letter.
            if (countNumberPrimaryList != 0)
            {
                counter = 1;
            }
            else
            {
                counter = 0;
            }
            do
            {
                //Add a anew list to create new wordfamily.
                listOfLists.Add(new List<string>());

                for (int i = 0; i < primaryWordList.Count; i++)
                {
                    //If the wordfamily is empty then add first word.
                    if (listOfLists[counter].Count == 0)
                    {
                        listOfLists[counter].Add(primaryWordList[i]);

                        //Add position of guessed letter in the new wordfamily to listOfPositionNumbers.
                        for (int j = 0; j < listOfLists[counter][0].Length; j++)
                        {
                            if (listOfLists[counter][0][j] == letter)
                            {
                                listOfPositionNumbers.Add(j);
                            }
                        }
                    }
                    else
                    {
                        //Reset for every word.
                        primaryWordCount = 0;
                        primaryWordPosition = 0;

                        //Count the guessed lettes in word which matches the position of wordfamily.
                        for (int j = 0; j < listOfPositionNumbers.Count; j++)
                        {
                            if (primaryWordList[i][listOfPositionNumbers[j]] == letter)
                            {
                                primaryWordPosition++;
                            }
                        }
                        //Count the number of guessed letter in word.
                        for (int k = 0; k < primaryWordList[i].Length; k++)
                        {
                            if (primaryWordList[i][k] == letter)
                            {
                                primaryWordCount++;
                            }
                        }
                        /*If this is true then it means that the word matches with the wordfamily and the word matches with 
                          gessed letter in hidden word (if any). Then add to wordfamily.*/
                        if (primaryWordPosition == primaryWordCount)
                        {
                            listOfLists[counter].Add(primaryWordList[i]);
                        }
                    }
                }
                //Delete all words that were added to new wordfamily and keep all the other words.
                primaryWordList = primaryWordList.Except(listOfLists[counter]).ToList();
                //Increment counter for listOfList.
                counter++;
                //Clear the position every wordfamily.
                listOfPositionNumbers.Clear();
            }
            while (primaryWordList.Count != 0);

            //Remove all words that doesnt match the hiddenword.
            RegexCleanWordfamily(usedLettersList, listOfLists);

            return primaryWordList;
        }

        public void CreateRegexPattern(List<char> lettersInUserGuesses)
        {
            //This is going to be used to determine if word match or not.
            regexPattern = @"";

            //tempPattern will be set to this value just the first time then will be changed until round finishes.
            if (firstTime == true)
            {
                tempPattern = @"[abcdefghijklmnopqrstuvwxyz]";
                firstTime = false;
            }

            /*Create a regex pattern. Allow any letters in each position if user has not matched a letter
              else allow only the letter that user has matched in a particular position.*/
            for (int i = 0; i < lettersInUserGuesses.Count; i++)
            {
                if (lettersInUserGuesses[i] == '_')
                {
                    regexPattern += tempPattern;
                }
                else
                {
                    regexPattern += lettersInUserGuesses[i];
                }
            }
            //Create instance and store expression.
            regex = new Regex(regexPattern, RegexOptions.IgnoreCase);
        }

        public void RegexCleanWordfamily(List<char> usedLettersList, List<List<string>> listOfLists)
        {
            //Remove letters that user has already guessed from tempPattern.
            for (int i = 0; i < usedLettersList.Count; i++)
            {
                tempPattern = Regex.Replace(tempPattern, usedLettersList[i].ToString(), "");
            }
            /*Go through all wordfamilies and remove all words that dont match with regex.
              This will make sure that there are no illegal words.*/
            for (int i = 0; i < listOfLists.Count; i++)
            {
                for (int j = 0; j < listOfLists[i].Count; j++)
                {
                    if (!regex.IsMatch(listOfLists[i][j]))
                    {
                        listOfLists[i].Remove(listOfLists[i][j]);
                    }
                }
            }
        }

        public List<string> ChooseBestWordFamily(List<List<string>> listOfLists, List<string> primaryWordList, int guesses)
        {
            //Reset every round
            counter = 0;
            position = 0;

            /*Go through all wordfamily in listOfLists and count the number of items.
             If counter is less than the number of items in wordfamily then store it in counter 
            and store position of wordfamily.*/
            if (guesses < 4)
            {
                for (int i = 0; i < listOfLists.Count; i++)
                {
                    if (counter < listOfLists[i].Count)
                    {
                        if (!listOfLists[i][0].Contains(letter))
                        {
                            counter = listOfLists[i].Count;
                            position = i;
                        }
                    }
                }
            }
            else if (counter == 0)
            {
                for (int i = 0; i < listOfLists.Count; i++)
                {
                    if (counter < listOfLists[i].Count)
                    {
                        counter = listOfLists[i].Count;
                        position = i;
                    }
                }
            }

            else
            {
                for (int i = 0; i < listOfLists.Count; i++)
                {
                    if (counter < listOfLists[i].Count)
                    {
                        counter = listOfLists[i].Count;
                        position = i;
                    }
                }
            }

            //Copy the items from longet wordfamily into primarywordlist.
            primaryWordList = listOfLists[position].ToList();
            //Destroy all wordfamily.
            listOfLists.Clear();

            return primaryWordList;

        }

        //Choose random word.
        public string ChooseWord(List<string> primaryWordList, string word)
        {
            word = primaryWordList[random.Next(0, primaryWordList.Count)];

            return word;
        }

        public int DetermineGuesses(string word, int guesses, List<char> lettersInUserGuesses)
        {
            if (word.Contains(letter))
            {
                counter = 0;
                for (int i = 0; i < word.Length; i++)
                {
                    //Change the letter in hidden word list to display correctly guessed letter.
                    if (word[i] == letter)
                    {
                        lettersInUserGuesses[i] = word[i];
                        counter++;
                    }
                }
                if (counter == 1)
                {
                    Console.WriteLine("Yes, there is {0} copy of {1}.", counter, letter);
                }
                else
                {
                    Console.WriteLine("Yes, there are {0} copies of {1}.", counter, letter);
                }
            }
            else
            {
                Console.WriteLine("Sorry, there are no {0}'s.", letter);
                //Subtract 1 from guesses if user got it wrong.
                guesses--;
            }

            return guesses;
        }
        public void ShowHiddenWord(List<char> lettersInUserGuesses)
        {
            //Display blanked out word.
            for (int i = 0; i < lettersInUserGuesses.Count; i++)
            {
                Console.Write(lettersInUserGuesses[i]);
            }

        }
    }
}
