using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AramData
{
    public class Game
    {
        public Game(Int64 gameId, Int16 platformId, Int16 season, DateTime gameCreation)
        {
            this.gameId = gameId;
            this.platformId = platformId;
            this.season = season;
            this.gameCreation = gameCreation;
        }

        public Game(int iD, long gameId, short platformId, short season, short scrapeIndex, short gameDuration, DateTime gameCreation)
        {
            this.ID = iD;
            this.gameId = gameId;
            this.platformId = platformId;
            this.season = season;
            this.scrapeIndex = scrapeIndex;
            this.gameDuration = gameDuration;
            this.gameCreation = gameCreation;
        }

        public int ID { get; set; }
        public Int64 gameId { get; set; }
        public Int16 platformId { get; set; }
        public Int16 season { get; set; }
        public Int16 scrapeIndex { get; set; }
        public Int16 gameDuration { get; set; }
        public DateTime gameCreation { get; set; }
    }
}
