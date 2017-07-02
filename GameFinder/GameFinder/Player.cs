using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameFinder
{
    class Player
    {
        public Player(long summonerID)
        {
            SummonerID = summonerID;
        }

        public long SummonerID { get; set; }
        public long AccountID { get; set; }
        public long TotalMatchesChecked { get; set; }
        public long AramsFound { get; set; }
        public long CheckedUntill { get; set; }


    }
}
