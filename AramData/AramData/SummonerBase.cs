using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AramData
{
    public class SummonerBase
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

        public bool AddUniqueSummoner(Summoner summoner)
        { 
            if (summonerList.Any(s => s.accountId == summoner.accountId)) // if not a new summoner
            {
                return false;
            }
            else
            {
                summonerList.Add(summoner);
                return true;
            }
        }

        public bool SummonersAvailable()
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
            cmd.CommandText = string.Format("SELECT `ID`, `name`,`platformId`,`accountId`,`timesChecked`,`aramsFound`,`checkedUntil` FROM `summoners` WHERE `platformId` = {0} AND `timesChecked` = 0 LIMIT 0,{1};", 8, limit);
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                this.summonerList.Add(new Summoner(Convert.ToInt32(reader["ID"]), (string)reader["name"], null, null, (string)reader["accountId"], Convert.ToInt32(reader["platformId"]), 0, 0, Convert.ToInt32(reader["timesChecked"]), Convert.ToInt32(reader["aramsFound"]), Convert.ToDateTime(reader["checkedUntil"])));
                count++;

            }
            reader.Close();

            return (count > 0);
        }

        public bool NewSummonersToDatabase(MySqlConnection link)
        {
            if (summonerList.Count < 1)
            {
                //Console.WriteLine("0/0 Games added to database");
                return false;
            }

            int numberOfsummoners = summonerList.Count;

            MySqlDataReader reader;
            MySqlCommand cmd = link.CreateCommand();
            string q;


            // remove known summoners
            q = "SELECT `accountId` FROM `summoners` WHERE `platformId` = 8 AND `accountId` in (\""; //TODO region selection

            foreach (var summoner in summonerList)
            {
                q += (summoner.accountId + "\",\"");
            }

            q = q.Remove(q.Length - 2);
            q += ");";

             cmd.CommandText = q;
            reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                summonerList.RemoveAll(s => s.accountId == (string)reader["accountId"]);
            }
            reader.Close();


            //no new summoners found
            if (summonerList.Count < 1)
            {
                Console.WriteLine("0/" + numberOfsummoners.ToString() + " summoners added to database");

                return false;
            }

            // if still new summoners add them

            q = "INSERT INTO summoners(`name`,`platformId`,`accountId`,`summonerId`,`profileIconId`) VALUES";

            foreach (var summoner in summonerList)
            {
                //Console.WriteLine(string.Format("UPDATE players SET `totalMatchesChecked`='{0}', `aramsFound`='{1}', `checkedUntil` = '{2}'  WHERE `summonerId` = {3} AND `regionId` = {4};", CurrentPlayer().TotalMatchesChecked, CurrentPlayer().AramsFound, CurrentPlayer().CheckedUntill.ToString("yyyy-MM-dd HH:mm:ss"), CurrentPlayer().SummonerID, region.regionId));
                q += string.Format("(\"{0}\",{1},\"{2}\",\"{3}\",{4}),", summoner.name, summoner.platformId.GetHashCode(), summoner.accountId, summoner.summonerId, summoner.profileIconId);
            }

            q = q.Remove(q.Length - 1);
            q += ";";

            cmd.CommandText = q;
            // Console.WriteLine(q);

            cmd.ExecuteNonQuery();

            Console.WriteLine(summonerList.Count.ToString() + "/" + numberOfsummoners.ToString() + " summoners added to database");

            summonerList.Clear();

            return true;
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
