using MySql.Data.Types;
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

        public Player(Int32 key, Int64 summonerID, Int16 totalMatchesChecked, Int16 aramsFound, DateTime checkedUntill)
        {
            Key = key;
            SummonerID = summonerID;
            TotalMatchesChecked = totalMatchesChecked;
            AramsFound = aramsFound;
            CheckedUntill = (DateTime)checkedUntill;
        }

        public long Key { get; set; }
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

        public string GetValuesString(Region region)
        {
            CheckedUntill = CheckedUntill.AddMinutes(1);
            return string.Format("(NULL, '{0}', '{1}', '{2}', '{3}', '{4}', '{5}'),", AccountID, region.regionId, SummonerID, TotalMatchesChecked, AramsFound, CheckedUntill.ToString("yyyy-MM-dd HH:mm:ss"));
        }
    }
}
