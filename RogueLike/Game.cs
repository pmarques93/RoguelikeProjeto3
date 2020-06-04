using System; // SO PARA TESTES, APAGAR <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
using System.Threading;

namespace RogueLike
{
    sealed public class Game
    {
        bool gameOver;
        // Enemy[] Enemies;
        // PowerUp[] powerUps;
        Map[,] map;
    
        /// <summary>
        /// Controls main game loop
        /// </summary>
        /// <param name="rows">Number of Rows</param>
        /// <param name="columns">Number of Columns</param>
        public Game(int rows, int columns, long seed)
        {
            // Instances / Variable
            Level level     = new Level(rows, columns, seed);
            Renderer print = new Renderer();
            Input input     = new Input(); 
            map             = new Map[rows, columns];
            string playerInput;
            string turn;

            ////////////////////////////////////////////////////////////////////
            // Runs Menu Loop
            do
            {
                print.Introduction();
                print.BlankLine();
                // Prints Initial Menu
                print.PrintMenu();
                // Gets user Input
                playerInput = input.MenuOptions();
                if(playerInput == "1") break;
                if(playerInput == "5") break;
            } while (playerInput != "5" || playerInput != "1");
            ////////////////////////////////////////////////////////////////////
            
            ////////////////////////////////////////////////////////////////////
            // Run Game
            if (playerInput == "1")
            {
                CreateMap(rows, columns);
                /* CreatePlayer(0, rows, columns); */ ///////////////////////// < METER O NUMERO RANDOM EM VEZ DE 0
                // CreatePowerUp(1); ///////////////////////// < METER O NUMERO RANDOM EM VEZ DE 0
                // CreateEnemy(1); ///////////////////////// < METER O NUMERO RANDOM EM VEZ DE 0
                
                print.PrintGameActions(); // Prints First Action

                gameOver = false;
                // Generates the map and its elements
                level.CreateLevel(map);
                // CreateEnemy(level);                
                while (gameOver == false)
                {
                    // Resets Movement
                    level.player.MovementReset(); 

                    ////////////////////////////////////////////////////////////
                    // Player Movement and Map print ///////////////////////////
                    if (NoRemainingMoves(level)){
                        print.NoMoves();
                        level.player.Die();
                    }
                    while (level.player.Movement > 0 &&    // Player has 2 Movements
                            level.player.IsAlive)          // Player is alive
                    {
                        turn = "Player";
                        print.Map(map, rows, columns, level.PowerUps, level.Enemies, 
                                level.player, turn, level.LevelNum);
                        map = input.GetPosition(level, map, print);
                        // Checks if the level.player got any powerUp
                        foreach (PowerUp powerUp in level.PowerUps)
                            if (PowerUpPosition(level.player, powerUp))
                                if (!powerUp.Picked)
                                {
                                    level.player.PickPowerUp(map, powerUp);
                                    print.GetGameActions(powerUp);
                                }
                        // Prints actions list
                        print.PrintGameActions();
                    }
                    ////////////////////////////////////////////////////////////

                    // Enemy Turn //////////////////////////////////////////////
                    if (level.player.IsAlive)
                    {   // Prints the map, moves enemy, prints the map
                        foreach (Enemy enemy in level.Enemies)
                        {
                            turn = "Enemy";
                            print.Map(map, rows, columns, level.PowerUps, level.Enemies, 
                                    level.player, turn,  level.LevelNum);
                            if (map[enemy.Position.Row, enemy.Position.Column].Position.HasPowerUp)
                            {
                                map[enemy.Position.Row, enemy.Position.Column].
                                    Position.EnemyFree(false);
                            }
                            else
                            {   // If the enemy moves to an empty position
                                map[enemy.Position.Row, enemy.Position.Column].
                                    Position.EnemyFree(); 
                            }
                            Thread.Sleep(1000);
                            // Moves the enemy, occupies its space and prints it
                            enemy.Move(level.player, 1, map);
                            map[enemy.Position.Row, enemy.Position.Column].
                                Position.EnemyOccupy();
                            print.Map(map, rows, columns, level.PowerUps, level.Enemies, 
                                    level.player, turn, level.LevelNum);
                        }
                        // Player gets damage if the he's 1 square distance
                        foreach (Enemy enemy in level.Enemies)
                            if (DamagePosition(level.player, enemy))
                            {
                                level.player.TakeDamage(enemy);
                                print.GetGameActions(enemy);
                            }
                        // Prints actions list
                        print.PrintGameActions();
                    }
                    ////////////////////////////////////////////////////////////


                    // Prints level.player HP or ends the game
                    if (!level.player.IsAlive)
                    {
                        print.Map(map, rows, columns, level.PowerUps, level.Enemies, 
                                level.player, "Enemy", level.LevelNum);
                        print.GoodBye();
                        Quit();
                    }
                }
            }
        
        }
        ////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Compares character position with another character position
        /// </summary>
        /// <param name="p1">Character1 Position</param>
        /// <param name="en">Character2 Position</param>
        /// <returns>True if the distance is 1 square around
        ///  otherwise false</returns>
        private bool DamagePosition(Character p1, Character en)
        {
            bool occupied = false;
                if (p1.Position.Row == en.Position.Row -1 &&
                    p1.Position.Column == en.Position.Column ||
                    p1.Position.Row == en.Position.Row +1 &&
                    p1.Position.Column == en.Position.Column ||
                    p1.Position.Column == en.Position.Column -1 &&
                    p1.Position.Row == en.Position.Row ||
                    p1.Position.Column == en.Position.Column +1 &&
                    p1.Position.Row == en.Position.Row)
                    occupied = true;
            return occupied;
        }

        /// <summary>
        /// Compares character position with powerUp position
        /// </summary>
        /// <param name="p1">Character position</param>
        /// <param name="powerUp">PowerUp position</param>
        /// <returns>True if both positions are the same 
        /// otherwise false</returns>
        private bool PowerUpPosition(Character p1, PowerUp powerUp)
        {
            bool occupied = false;
                if (p1.Position.Row == powerUp.Position.Row &&
                    p1.Position.Column == powerUp.Position.Column)
                    occupied = true;
            return occupied;
        }

        /// <summary>
        /// Creates the game map
        /// </summary>
        /// <param name="rows">Number of rows in the game</param>
        /// <param name="columns">Number of columns in the game</param>
        private void CreateMap(int rows, int columns)
        {
            for (int i = 0; i < rows; i++) 
                for (int j = 0; j < columns; j++)
                    map[i,j] = new Map (new Position(i,j));
        }

        /// <summary>
        /// Creates level.player
        /// </summary>
        /// <param name="x">Random number to spawn level.player</param>
        /// <param name="rows">Number of rows in the game</param>
        /// <param name="columns">Number of columns in the game</param>
       /*  private void CreatePlayer(int x, int rows, int columns)
        {
            level.player = new Player(new Position(x, 0), rows, columns);
            map[x, 0].Position.PlayerOccupy();
        } */

        /// <summary>
        /// Stops the game loop and exits game
        /// </summary>

        /// <summary>
        /// Checks if the level.player can move
        /// </summary>
        /// <returns>True if they level.player can't move</returns>
        private bool NoRemainingMoves(Level level)
        {
            int  count = 0;
            bool lose  = false;

            try
            {   // Checks north
                if (map[level.player.Position.Row - 1, level.player.Position.Column].
                    Position.Walkable == false) count++;               
            } catch {count++;};
            try
            {   // Checks south
                if (map[level.player.Position.Row + 1, level.player.Position.Column].
                    Position.Walkable == false) count++;               
            } catch {count++;};
            try
            {   // Checks east
                if (map[level.player.Position.Row, level.player.Position.Column + 1].
                    Position.Walkable == false) count++;               
            } catch {count++;};
            try
            {   // Checks Column
                if (map[level.player.Position.Row, level.player.Position.Column - 1].
                    Position.Walkable == false) count++;               
            } catch {count++;};
            // If count == 4, it's gameover
            if (count == 4) lose = true;
            return lose;
        }

        /// <summary>
        /// Quits the game loop
        /// </summary>
        private void Quit() => gameOver = true;
    }
}