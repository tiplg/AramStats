using System;
using System.Collections.Generic;
using System.Text;

namespace GameFinderV4
{
    class SummonerBase
    {
        public SummonerBase()
        {
            summonerList = new List<Summoner>();
            summonerToUpdate = new List<Summoner>();
        }

        public List<Summoner> summonerList { get; set; }        //new summoners to check
        public List<Summoner> summonerToUpdate { get; set; }    //checked summoners ready to be updated in database     

        public Summoner CurrentSummoner()
        {
            return summonerList[0];
        }

        public void AddSummoner(Summoner summoner)
        {
            summonerList.Add(summoner);
        }

        internal bool SummonersAvailable()
        {
            return summonerList.Count > 0;
        }

        public void NextSummoner()
        {
            summonerToUpdate.Add(summonerList[0]);
            summonerList.RemoveAt(0);
        }
    }
}
