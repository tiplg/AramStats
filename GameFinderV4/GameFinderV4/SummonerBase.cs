using MySql.Data.MySqlClient;
using RiotSharp.Misc;
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

        public bool LoadFromDatabase(MySqlConnection link, int limit)
        {
            UpdateSummonersToDatabase(link);

            var count = 0;
            MySqlCommand cmd = link.CreateCommand();
            cmd.CommandText = string.Format("SELECT `ID`, `name`,`platformId`,`accountId`,`timesChecked`,`aramsFound`,`checkedUntil` FROM `summoners` WHERE `platformId` = {0} AND `timesChecked` = 0 LIMIT 0,{1};", 2, limit);
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                this.summonerList.Add(new Summoner(Convert.ToInt32(reader["ID"]), (string)reader["name"], null, null, (string)reader["accountId"], Convert.ToInt32(reader["platformId"]), 0, 0, Convert.ToInt32(reader["timesChecked"]), Convert.ToInt32(reader["aramsFound"]), Convert.ToDateTime(reader["checkedUntil"])));
                count++;

            }
            reader.Close();

            return (count > 0);
        }

        public bool UpdateSummonersToDatabase(MySqlConnection link)
        {
            if (summonerToUpdate.Count < 1)
            {
                return false;
            }

            MySqlCommand cmd = link.CreateCommand();
            string q = "INSERT INTO summoners(`ID`,`timesChecked`,`aramsFound`,`checkedUntil`) VALUES";

            foreach (var summoner in summonerToUpdate)
            {
                //Console.WriteLine(string.Format("UPDATE players SET `totalMatchesChecked`='{0}', `aramsFound`='{1}', `checkedUntil` = '{2}'  WHERE `summonerId` = {3} AND `regionId` = {4};", CurrentPlayer().TotalMatchesChecked, CurrentPlayer().AramsFound, CurrentPlayer().CheckedUntill.ToString("yyyy-MM-dd HH:mm:ss"), CurrentPlayer().SummonerID, region.regionId));
                q += string.Format("({0},{1},{2},'{3}'),", summoner.id, summoner.timesChecked, summoner.aramsFound, summoner.checkedUntil.ToString("yyyy-MM-dd HH:mm:ss"));
            }

            q = q.Remove(q.Length - 1);
            q += " ON DUPLICATE KEY UPDATE `aramsFound`= VALUES(`aramsFound`),`timesChecked`= VALUES(`timesChecked`),`checkedUntil`= VALUES(`checkedUntil`);";

            cmd.CommandText = q;
           // Console.WriteLine(q);

            cmd.ExecuteNonQuery();

            summonerToUpdate.Clear();

            return true;
        }
    }
}
