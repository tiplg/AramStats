using RiotSharp.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatScraper
{
    public class Game
    {
        public long GameId { get; set; }
        public DateTime matchCreation { get; set; }
        public int scrapeIndex { get; set; }
        public int matchDuration { get; set; }
        public string region { get; set; }

        public Game(long gameId)
        {
            GameId = gameId;
        }

        public Game() { }

        public Game(long gameId, DateTime matchCreation, int scrapeIndex, int matchDuration, Region region)
        {
            GameId = gameId;
            this.matchCreation = matchCreation;
            this.scrapeIndex = scrapeIndex;
            this.matchDuration = matchDuration;
            this.region = region.ToString();

        }
    }
}
