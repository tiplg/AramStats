using RiotSharp.Misc;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameFinderV4
{
    class Game
    {
        public Game(Int64 gameId, Platform platform, DateTime gameCreation)
        {
            this.gameId = gameId;
            //this.platformId = platformId; TODO
            this.gameCreation = gameCreation;
        }

        public Int64 gameId { get; set; }
        public Region platformId { get; set; }
        public Int16 season { get; set; }
        public Int16 scrapeIndex { get; set; }
        public Int16 gameDuration { get; set; }
        public DateTime gameCreation { get; set; }
    }
}
