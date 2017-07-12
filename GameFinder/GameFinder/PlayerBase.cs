using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using MySql.Data.MySqlClient;
using MySql.Data.Types;

namespace GameFinder
{
    public class PlayerBase
    {
        public int index { get; set; }
        public List<Player> players { get; set; }
        public List<Player> playersToUpdate { get; set; }
        public Region region { get; set; }

        public PlayerBase()
        {
            index = 0;
            players = new List<Player>();
            playersToUpdate = new List<Player>();
        }

        public PlayerBase(Region region, MySqlConnection link)
        {
            index = 0;
            players = new List<Player>();
            playersToUpdate = new List<Player>();
            this.region = region;
        }

        public void AddPlayer(long summonerId)
        {
            if (players.Any(p => p.SummonerID == summonerId))
            {
                //Console.Write(".");
            }
            else
            {
                //Console.Write("+");
                players.Add(new Player(summonerId));
            }
        }

        public void IncrementIndex()
        {
            index++;
        }

        public bool PlayersAvailable()
        {
            return (players.Count > 0);
        }

        public Player CurrentPlayer()
        {
            return players[0];
        }

        public void SaveToFile(string fileName)
        {
            using (var stream = new FileStream(fileName, FileMode.Create))
            {
                var XML = new XmlSerializer(typeof(PlayerBase));
                XML.Serialize(stream, this);
            }
        }

        public static PlayerBase LoadFromFile(string fileName)
        {
            using (var stream = new FileStream(fileName, FileMode.Open))
            {
                var XML = new XmlSerializer(typeof(PlayerBase));
                return (PlayerBase)XML.Deserialize(stream);
            }
        }

        public void LoadFromDatabase(MySqlConnection link, int limit)
        {
            MySqlCommand cmd = link.CreateCommand();
            cmd.CommandText = string.Format("SELECT `id`, `summonerId`,`totalMatchesChecked`,`aramsFound`,`checkedUntil` FROM `players` WHERE `regionId` = {0} AND `totalMatchesChecked` = 0 LIMIT 0,{1};", region.regionId, limit);
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                //Console.WriteLine(reader["summonerId"].GetType());
                //Console.WriteLine(reader["totalMatchesChecked"].GetType());
                //Console.WriteLine(reader["aramsFound"].GetType());
                //Console.WriteLine(reader["checkedUntil"].GetType());
                this.players.Add(new Player((Int32)reader["id"], (Int64)reader["summonerId"], (Int16)reader["totalMatchesChecked"], (Int16)reader["aramsFound"], (DateTime)reader["checkedUntil"]));
            }
            reader.Close();
        }

        public void NextPlayer()
        {
            playersToUpdate.Add(players[0]);
            players.RemoveAt(0);
        }

        public void UpdatePlayers(MySqlConnection link)
        {
            MySqlCommand cmd = link.CreateCommand();
            string q = "INSERT INTO players(`id`,`totalMatchesChecked`,`aramsFound`,`checkedUntil`) VALUES";

            foreach (var player in playersToUpdate)
            {
                if (player.TotalMatchesChecked == 0)
                {
                    player.TotalMatchesChecked = 1;
                }
                //Console.WriteLine(string.Format("UPDATE players SET `totalMatchesChecked`='{0}', `aramsFound`='{1}', `checkedUntil` = '{2}'  WHERE `summonerId` = {3} AND `regionId` = {4};", CurrentPlayer().TotalMatchesChecked, CurrentPlayer().AramsFound, CurrentPlayer().CheckedUntill.ToString("yyyy-MM-dd HH:mm:ss"), CurrentPlayer().SummonerID, region.regionId));
                q += string.Format("({0},{1},{2},'{3}'),",player.Key, player.TotalMatchesChecked, player.AramsFound, player.CheckedUntill.ToString("yyyy-MM-dd HH:mm:ss"));
                
            }

            q = q.Remove(q.Length - 1);
            q += " ON DUPLICATE KEY UPDATE `aramsFound`= VALUES(`aramsFound`),`totalMatchesChecked`= VALUES(`totalMatchesChecked`),`checkedUntil`= VALUES(`checkedUntil`);"; 

            cmd.CommandText = q;
            cmd.ExecuteNonQuery();

            playersToUpdate.Clear();
        }
    }
}
