﻿using RiotSharp.Endpoints.MatchEndpoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameFinderV4
{
    class Summoner
    {
        public string name { get; set; }
        public string puuid { get; set; }
        public string summonerId { get; set; }
        public string accountId { get; set; }

        public int platformId { get; set; }
        public int profileIconId { get; set; }
        public int summonerlevel { get; set; }
        public int timesChecked { get; set; }
        public int aramsFound { get; set; }
        public DateTime checkedUntill { get; set; }

        public Summoner(string name, string puuid, string summonerId, string accountId, int platformId, int profileIconId, int summonerlevel, int timesChecked, int aramsFound, DateTime checkedUntill)
        {

            this.name = name;
            this.puuid = puuid;
            this.summonerId = summonerId;
            this.accountId = accountId;
            this.platformId = platformId;
            this.profileIconId = profileIconId;
            this.summonerlevel = summonerlevel;
            this.timesChecked = timesChecked;
            this.aramsFound = aramsFound;
            this.checkedUntill = checkedUntill;

        }

        public void AddGamesFound(MatchList matchList)
        {
            aramsFound += matchList.TotalGames;
            timesChecked++;
            checkedUntill = matchList.Matches.Max(t => t.Timestamp).AddSeconds(1);
        }

        internal void NoGamesFound()
        {
            timesChecked++;
        }
    }
}
