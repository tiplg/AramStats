using RiotSharp.Misc;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameFinderV4
{
    class Game
    {
        public Game(Int64 gameId, int platformId, int season, DateTime gameCreation)
        {
            this.gameId = gameId;
            this.platformId = platformId;
            this.season = season;
            this.gameCreation = gameCreation;
        }

        public Int64 gameId { get; set; }
        public int platformId { get; set; }
        public int season { get; set; }
        public Int16 scrapeIndex { get; set; }
        public Int16 gameDuration { get; set; }
        public DateTime gameCreation { get; set; }
    }
}
