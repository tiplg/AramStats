using RiotSharp.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameFinderV4
{
    class GameBase
    {
        public GameBase()
        {
            gameList = new List<Game>();
        }

        public List<Game> gameList { get; set; }

        public bool AddNewGame(Int64 gameId, Platform platform, DateTime timestamp)
        {
            Game game = new Game(gameId, platform, timestamp);

            if (gameList.Any(g => g.gameId == game.gameId)) // if not a new game
            {
                return false;
            }
            else
            {
                gameList.Add(game);
                return true;
            }
        }
    }
}
