using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameFinder
{
    public class Game
    {
        public Game(long gameId)
        {
            GameId = gameId;
        }

        public Game() { }

        public long GameId { get; set; }
    }
}
