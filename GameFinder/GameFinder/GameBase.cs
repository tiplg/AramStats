using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using MySql.Data.MySqlClient;

namespace GameFinder
{
    public class GameBase
    {
        private Region region;

        public List<Game> games { get; set; }

        public GameBase() {
            games = new List<Game>();
        }

        public GameBase(Region region)
        {
            games = new List<Game>();
            this.region = region; 
        }

        //public GameBase() { }

        public bool AddGame(Game game)
        {
            if (games.Any(g => g.GameId == game.GameId)) // if not a new game
            {
                return false;
            }
            else
            {
                games.Add(game);
                return true;
            }
        }


        public void SaveToFile(string fileName)
        {
            using (var stream = new FileStream(fileName, FileMode.Create))
            {
                var XML = new XmlSerializer(typeof(GameBase));
                XML.Serialize(stream, this);
            }
        }

        public static GameBase LoadFromFile(string fileName)
        {
            using (var stream = new FileStream(fileName, FileMode.Open))
            {
                var XML = new XmlSerializer(typeof(GameBase));
                return (GameBase)XML.Deserialize(stream);
            }
        }

        public void AddPlayersAndGamesToDatabase(MySqlConnection link)
        {
            MySqlCommand cmd = link.CreateCommand();

            List<long> knownPlayers = new List<long>();
            List<RiotSharp.GameEndpoint.Player> newPlayers = new List<RiotSharp.GameEndpoint.Player>();

            List<long> knownGames = new List<long>();
            int newGames = 0;

            string query;
            MySqlDataReader reader;

            if (games.Count > 0)
            {
                query = "SELECT `matchId` FROM `games` WHERE `matchId` in (";

                foreach (var game in games)
                {
                    query += (game.GameId + ",");
                }

                query = query.Remove(query.Length - 1);
                query += ");";

                cmd.CommandText = query;
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    knownGames.Add((Int64)reader["matchId"]);
                }
                reader.Close();

                foreach (var knownId in knownGames)
                {
                    games.RemoveAll(g => g.GameId == knownId);
                }

                if (games.Count > 0)
                {
                    query = "INSERT INTO `games` (`id`, `matchId`, `regionId`, `scrapeIndex`, `seasonId`, `gameDuration`, `gameCreation`) VALUES ";

                    foreach (var game in games)
                    {
                        query += game.GetValuesString(region);
                    }

                    query = query.Remove(query.Length - 1);
                    query += ";";

                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();
                }
            }

            newGames = games.Count;

            foreach (var game in games)
            {
                if (game.FellowPlayers != null)
                {
                    foreach (var player in game.FellowPlayers)
                    {
                        newPlayers.Add(player);
                    }
                }
                else
                {
                    Console.WriteLine("ERROR IN PLAYERS FROM GAME: " + game.GameId);
                }
            }

            if (newPlayers.Count > 0)
            {
                query = "SELECT `summonerId` FROM `players` WHERE `summonerId` in (";

                foreach (var player in newPlayers)
                {
                    query += (player.SummonerId.ToString() + ",");
                }

                query = query.Remove(query.Length - 1);
                query += ");";

                cmd.CommandText = query;
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    knownPlayers.Add((Int64)reader["summonerId"]);
                }

                foreach (var knownId in knownPlayers)
                {
                    newPlayers.RemoveAll(p => p.SummonerId == knownId);
                }

                reader.Close();
                if (newPlayers.Count > 0)
                {
                    query = "INSERT INTO `players` (`id`,`accountId`, `regionId`, `summonerId`, `totalMatchesChecked`, `aramsFound`, `checkedUntil`) VALUES ";

                    foreach (var player in newPlayers)
                    {
                        query += string.Format("(NULL, '{0}', '{1}', '{2}', '{3}', '{4}', '{5}'),", 0, region.regionId, player.SummonerId, 0, 0, 0);
                    }

                    query = query.Remove(query.Length - 1);
                    query += ";";

                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();
                }
            }

            Console.WriteLine("ADDED GAMES: " + newGames + " ADDED PLAYERS: " + newPlayers.Count);

            games.Clear();
        }
    }
}
