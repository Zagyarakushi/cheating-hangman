using System;

namespace WordFamilies
{
    class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game();

            //Loop while player wants to play again.
            do
            {
                game.MainMenu();
                game.StartGame();
            }
            while (game.playAgain == 'Y' || game.playAgain == 'y');

            //Clear Screen and show message to player before exit.
            Console.Clear();
            Console.WriteLine("Thank you for playing!");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
