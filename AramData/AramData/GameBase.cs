using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using RiotSharp.Misc;

namespace AramData
{
    public class GameBase
    {
        public GameBase()
        {
            gameList = new List<Game>();
            gamesToUpdate = new List<Game>();
        }

        public List<Game> gameList { get; set; }
        public List<Game> gamesToUpdate { get; set; }

        public bool AddNewGame(Int64 gameId, Int16 platform, Int16 season, DateTime timestamp)
        {
            Game game = new Game(gameId, platform, season, timestamp);

            if (gameList.Any(g => g.gameId == game.gameId)) // if not a new game
            {
                return false;
            }
            else
            {
                gameList.Add(game);
                return true;
            }
        }

        public bool UpdateToDatabase(MySqlConnection link)
        {
            return true;
        }

        public bool GamesAvailable()
        {
            return gameList.Count > 0;
        }

        public bool LoadFromDatabase(MySqlConnection link, int limit)
        {
            UpdateGamesToDatabase(link);

            var count = 0;
            MySqlCommand cmd = link.CreateCommand();
            cmd.CommandText = string.Format("SELECT `ID`, `gameId`,`platformId`,`season`,`scrapeIndex`,`gameDuration`,`gameCreation` FROM `games` WHERE `platformId` = {0} AND `scrapeIndex` = 0 LIMIT 0,{1};", 2, limit);
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                this.gameList.Add(new Game(Convert.ToInt32(reader["ID"]), Convert.ToInt64(reader["gameId"]), Convert.ToInt16(reader["platformId"]), Convert.ToInt16(reader["season"]), Convert.ToInt16(reader["scrapeIndex"]), Convert.ToInt16(reader["gameDuration"]), Convert.ToDateTime(reader["gameCreation"])));
                count++;
            }
            reader.Close();

            return (count > 0);
        }

        private bool UpdateGamesToDatabase(MySqlConnection link)
        {
            if (gamesToUpdate.Count < 1)
            {
                return false;
            }

            MySqlCommand cmd = link.CreateCommand();
            string q = "INSERT INTO games(`ID`,`timesChecked`,`aramsFound`,`checkedUntil`) VALUES";

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

        public bool NewGamesToDatabase(MySqlConnection link)
        {
            if (gameList.Count < 1)
            {
                //Console.WriteLine("0/0 Games added to database");
                return false;
            }

            int numberOfgames = gameList.Count;

            MySqlDataReader reader;
            MySqlCommand cmd = link.CreateCommand();
            string q;


            // remove known games
            q = "SELECT `gameId` FROM `games` WHERE `platformId` = 2 AND `gameId` in ("; //TODO region selection

            foreach (var game in gameList)
            {
                q += (game.gameId + ",");
            }

            q = q.Remove(q.Length - 1);
            q += ");";

            cmd.CommandText = q;
            reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                gameList.RemoveAll(g => g.gameId == Convert.ToInt64(reader["gameId"]));
            }
            reader.Close();


            // if still new games add them
            if (gameList.Count < 1)
            {
                Console.WriteLine("0/" + numberOfgames.ToString() + " Games added to database");

                return false;
            }

            q = "INSERT INTO games(`gameId`,`platformId`,`season`,`gameCreation`) VALUES";

            foreach (var game in gameList)
            {
                //Console.WriteLine(string.Format("UPDATE players SET `totalMatchesChecked`='{0}', `aramsFound`='{1}', `checkedUntil` = '{2}'  WHERE `summonerId` = {3} AND `regionId` = {4};", CurrentPlayer().TotalMatchesChecked, CurrentPlayer().AramsFound, CurrentPlayer().CheckedUntill.ToString("yyyy-MM-dd HH:mm:ss"), CurrentPlayer().SummonerID, region.regionId));
                q += string.Format("({0},{1},{2},'{3}'),", game.gameId, game.platformId, game.season, game.gameCreation.ToString("yyyy-MM-dd HH:mm:ss"));
            }

            q = q.Remove(q.Length - 1);
            q += ";";

            cmd.CommandText = q;
            // Console.WriteLine(q);

            cmd.ExecuteNonQuery();

            Console.WriteLine(gameList.Count.ToString() + "/" + numberOfgames.ToString() + " Games added to database");

            gameList.Clear();

            return true;
        }
    }
}
