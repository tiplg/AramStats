using RiotSharp.MatchEndpoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatScraper
{
    public class ChampStat
    {
        public ChampStat(long gameId, Participant participant)
        {
            GameId = gameId;

            ChampionId = participant.ChampionId;
            highestAchievedSeasonTier = participant.HighestAchievedSeasonTier.ToString();
            spell1Id = participant.Spell1Id;
            spell2Id = participant.Spell2Id;

            winner = Convert.ToInt32(participant.Stats.Winner);
            minionsKilled = participant.Stats.MinionsKilled;
            kills = participant.Stats.Kills;
            assists = participant.Stats.Assists;
            deaths = participant.Stats.Deaths;
            goldEarned = participant.Stats.GoldEarned;

            item0 = participant.Stats.Item0;
            item1 = participant.Stats.Item1;
            item2 = participant.Stats.Item2;
            item3 = participant.Stats.Item3;
            item4 = participant.Stats.Item4;
            item5 = participant.Stats.Item5;
            item6 = participant.Stats.Item6;
        }

        public ChampStat() { }

        public long GameId { get; set; }
        public long ChampionId { get; set; } 
        public string highestAchievedSeasonTier { get; set; }
        public int spell1Id { get; set; }
        public int spell2Id { get; set; }

        public int winner { get; set; }
        public long minionsKilled { get; set; }
        public long kills { get; set; }
        public long assists { get; set; }
        public long deaths { get; set; }
        public long goldEarned { get; set; }

        public long item0 { get; set; }
        public long item1 { get; set; }
        public long item2 { get; set; }
        public long item3 { get; set; }
        public long item4 { get; set; }
        public long item5 { get; set; }
        public long item6 { get; set; }


    }
}
