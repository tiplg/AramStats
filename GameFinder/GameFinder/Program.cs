using RiotSharp;
using RiotSharp.Misc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using MySql.Data.MySqlClient;
using MySql.Data.Types;

namespace GameFinder // EU
{
    class Program
    {
        public static MySqlConnection link;

        public static string apikey = ConfigurationManager.AppSettings["apikey"];

        static void Main(string[] args)
        {
            MySqlConnectionStringBuilder b = new MySqlConnectionStringBuilder();
            b.Server = ConfigurationManager.AppSettings["server"];
            b.UserID = ConfigurationManager.AppSettings["uid"];
            b.Password = ConfigurationManager.AppSettings["password"];
            b.Database = ConfigurationManager.AppSettings["database"];
            b.Port = 3306;
            b.ConvertZeroDateTime = true;
            b.AllowZeroDateTime = true;

            link = new MySqlConnection(b.ToString()); //b.ToString()
            MySqlCommand cmd = link.CreateCommand();

            Console.WriteLine("waddup boois");
            Console.WriteLine(apikey);

            var api = RiotApi.GetInstance(apikey,8,500); // , 7, 500

            
            List<Region> regions = new List<Region>();
            regions.Add(new Region("euw", RiotSharp.Misc.Region.euw, 1));
            regions.Add(new Region("eune", RiotSharp.Misc.Region.eune, 0));
            regions.Add(new Region("na", RiotSharp.Misc.Region.na, 3));
            regions.Add(new Region("kr", RiotSharp.Misc.Region.kr, 2));

            Region region = null;
            try
            {
                region = regions.Find(r => r.name == args[0]);
            }
            catch (Exception){ }
            

            while (region == null)
            {
                Console.Write("Enter Region(euw/eune/na/kr): ");
                string result = Console.ReadLine().ToLower();
                region = regions.Find(r => r.name == result);
            }

            Console.WriteLine("Region selected: " + region.region.ToString());
            Console.Title = "GameFinder " + region.region.ToString().ToUpper();

            regions.Clear();

            while (!link.Ping())
            {
                try
                {
                    link.Open();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            PlayerBase players = new PlayerBase(region,link); //  new PlayerBase(); //
            //PlayerBase newPlayers = new PlayerBase(region, link);
            GameBase gamebase = new GameBase(region); //new GameBase();

            players.LoadFromDatabase(link,100);

            while (players.PlayersAvailable())
            {

                try
                {
                    List<RiotSharp.GameEndpoint.Game> games = new List<RiotSharp.GameEndpoint.Game>();

                    Console.WriteLine("Checking games from " + players.CurrentPlayer().SummonerID);
                    try
                    {
                        games = api.GetRecentGames(region.region, players.CurrentPlayer().SummonerID); //get 10 games
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        try
                        {
                            RiotSharpException ex = (RiotSharpException)e;
                            Console.WriteLine("ERROR: " + ex.Message);
                            if (ex.Message == "429, Rate Limit Exceeded")
                            {
                                //Console.WriteLine("429, Rate Limit Exceeded");
                                System.Threading.Thread.Sleep(5000);
                                continue;
                            }
                            else
                            {
                                players.NextPlayer();
                                continue;
                            }
                        }
                        catch (Exception)
                        {
                            
                        }
                        //Console.WriteLine(e.Message);
                        players.NextPlayer();
                        continue;
                    }
                    
                    players.CurrentPlayer().AddGamesFound(games.Count);

                   // Console.Write(">");
                    foreach (var game in games)// for all games
                    {
                        if (game.GameMode == GameMode.Aram) // if this game is aram game
                        {
                            if (game.CreateDate > players.CurrentPlayer().CheckedUntill)
                            {
                                players.CurrentPlayer().AddAramsFound(1);

                                gamebase.AddGame(new Game(game.GameId, game.FellowPlayers));
                            }
                        }
                    }

                    if (games.Count > 0)
                    {
                        players.CurrentPlayer().CheckedUntill = games[0].CreateDate;
                    }
                }
                catch (RiotSharpException ex)
                {
                    // Handle the exception however you want.
                    throw;
                }

                players.NextPlayer();
                
                
                if (players.players.Count < 5)
                {
                    Console.WriteLine("UPLINK TO DATABASE...");
                    gamebase.AddPlayersAndGamesToDatabase(link);
                    players.UpdatePlayers(link);
                    players.LoadFromDatabase(link, 100);
                    Console.WriteLine("QUIT NOW !");
                    System.Threading.Thread.Sleep(5000);
                    Console.WriteLine("to late.");
                }
                

                //players.IncrementIndex();

                //Console.WriteLine("checking player " + players.index + " out of " + players.players.Count +" |" + ((float)players.index/players.players.Count*100 ).ToString("0.00") + "%| Games: " + gamebase.games.Count + " Est Total Games:" + (((float)gamebase.games.Count/ players.index)* players.players.Count).ToString("0"));
                //Console.WriteLine();
            }


            Console.ReadKey();
        }

        public void SaveFiles(PlayerBase pb, GameBase gb)
        {
            pb.SaveToFile("DataFiles/playerbase.xml");
            gb.SaveToFile("DataFiles/gamebase.xml");
        }
    }
}
