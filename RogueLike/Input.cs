using System;
namespace RogueLike
{
    /// <summary>
    /// Responsible for every user input on the console
    /// </summary>
    sealed public class Input
    {
        private Renderer print;
        
        /// <summary>
        /// Class Constructor
        /// </summary>
        public Input()
        {
            print = new Renderer();
        }

        /// <summary>
        /// Creates options menu
        /// </summary>
        /// <returns>Returns player's option</returns>
        public string MenuOptions()
        {
            HighScoreManager highScore = new HighScoreManager();
            string playerInput;

            //Keeps running until players starts new game
            playerInput = Console.ReadLine();
            switch(playerInput)
            {
                //Starts new game
                case "1":
                    return playerInput;
                
                //Prints the Highscore Screen
                case "2":
                    highScore.PrintScore();
                    break;

                //Prints the game's Instructions
                case "3":
                    print.PrintInstructions();
                    break;

                //Prints the game's developers
                case "4":
                    print.PrintCredits();
                    break;
                
                //Prints Exit message and closes the game
                case "5":
                    print.PrintExitMsg();
                    return playerInput;
                //Returns the input here so the players goes back to main menu
                default:
                    print.PrintInputError();
                    break;
            }
            //Asks the user for an input to leave the option screen
            Console.ReadLine();
            return playerInput;
        }
        
        /// <summary>
        /// Gets a map with all positions updated
        /// </summary>
        /// <param name="player">Player's position</param>
        /// <param name="map">All map Positions</param>
        /// <returns>Returns a map position for the player</returns>
        public void GetPosition(Level level , Map[,] map, Renderer print)
        {
            // players input
            ConsoleKeyInfo playerInput;
            // Frees the player position
            map[level.player.Position.Row, level.player.Position.Column].
                Position.PlayerFree();

            // Gets player input
            playerInput = Console.ReadKey();
            
            // Checks if the player pressed the ESC key and quits the game
            if (playerInput.Key == ConsoleKey.Escape)
            {
                level.player.Die();
                level.player.LeaveGame();
                return;
            }
 

            // Moves player to new free position    
            if(level.player.Move(map, playerInput, print))
                map[level.player.Position.Row, level.player.Position.Column].
                Position.PlayerFree();
                // Occupies inserted position with player
                map[level.player.Position.Row,level.player.Position.Column].
                Position.PlayerOccupy();
                
        }

        /// <summary>
        /// Asks for user name for high score
        /// </summary>
        /// <returns>User name</returns>
        public String InsertName()
        {
            string trim = "";
            bool leave = false;
            while (leave == false)
            {
                string name = Console.ReadLine();
                trim = name.Trim();
                trim = trim.Replace( " ", "_");
                if (trim.Length < 12 && trim.Length > 0) leave = true;
                else print.InsertShorterName();
            }
            return trim;
        }
    }
}