using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameFinder
{
    public class Player
    {
        public Player(long summonerID)
        {
            SummonerID = summonerID;
            CheckedUntill = DateTime.MinValue;
        }

        public Player() { }

        public long SummonerID { get; set; }
        public long AccountID { get; set; }
        public long TotalMatchesChecked { get; set; }
        public long AramsFound { get; set; }
        public DateTime CheckedUntill { get; set; }

        public void AddGamesFound(int numGames)
        {
            TotalMatchesChecked += numGames;
        }

        public void AddAramsFound(int numGame)
        {
            AramsFound += numGame;
        }

    }
}
