using MySql.Data.MySqlClient;
using RiotSharp.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameFinderV4
{
    class GameBase
    {
        public GameBase()
        {
            gameList = new List<Game>();
        }

        public List<Game> gameList { get; set; }

        public bool AddNewGame(Int64 gameId, int platform, int season, DateTime timestamp)
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

        public bool UpdateGamesToDatabase(MySqlConnection link)
        {
            if (gameList.Count < 1)
            {
                return false;
            }

            MySqlCommand cmd = link.CreateCommand();
            string q = "INSERT INTO games(`gameId`,`platformId`,`season`,`gameCreation`) VALUES";

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

            Console.WriteLine(gameList.Count + " Games added to database");

            gameList.Clear();

            return true;
        }
    }
}
