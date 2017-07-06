using RiotSharp;
using RiotSharp.Misc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace GameFinder // EU
{
    class Program
    {
        static void Main(string[] args)
        {

            var apikey = ConfigurationManager.AppSettings["apikey"];

            Console.WriteLine("waddup boois");
            Console.WriteLine(apikey);

            var api = RiotApi.GetInstance(apikey,8,500); // , 7, 500

            Region region;
            List<Region> regions = new List<Region>();
            regions.Add(new Region("euw", RiotSharp.Misc.Region.euw, "DataFiles/EUW/"));
            regions.Add(new Region("eune", RiotSharp.Misc.Region.eune, "DataFiles/EUNE/"));
            regions.Add(new Region("na", RiotSharp.Misc.Region.na, "DataFiles/NA/"));
            regions.Add(new Region("kr", RiotSharp.Misc.Region.kr, "DataFiles/KR/"));

            do
            {
                Console.Write("Enter Region(euw/eune/na/kr): ");
                string result = Console.ReadLine().ToLower();
                region = regions.Find(r => r.name == result);


            } while (region == null);




            //List<long> gamesList = new List<long>();
            //List<long> idList = new List<long>();



            PlayerBase playerbase = PlayerBase.LoadFromFile(region.folder+"playerbase.xml"); //  new PlayerBase(); //
            GameBase gamebase = GameBase.LoadFromFile(region.folder + "gamebase.xml"); //new GameBase();

            /*
            try
            {
                var summoner = api.GetSummonerByName(Region.euw, "sipsclar"); //api.GetSummoner(Region.euw, "sipsclar");
                Console.WriteLine(summoner.Id);
                idList.Add(summoner.Id);

                playerbase.AddPlayer(summoner.Id);
            }
            catch (RiotSharpException ex)
            {
                // Handle the exception however you want.
            }
            */

            var timer = new System.Threading.Timer((e) =>
            {
                playerbase.SaveToFile(region.folder + "playerbase.xml");
                gamebase.SaveToFile(region.folder + "gamebase.xml");

                Console.WriteLine("SAVING FILES");
            }, null, TimeSpan.Zero,  TimeSpan.FromMinutes(1));


            while (playerbase.PlayersAvailable())
            {

                try
                {
                    List<RiotSharp.GameEndpoint.Game> games = new List<RiotSharp.GameEndpoint.Game>();

                    Console.WriteLine("Checking games from " + playerbase.CurrentPlayer().SummonerID);
                    try
                    {
                        games = api.GetRecentGames(region.region, playerbase.CurrentPlayer().SummonerID); //get 10 games
                    }
                    catch (Exception e)
                    {
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
                                playerbase.IncrementIndex();
                                continue;
                            }
                        }
                        catch (Exception)
                        {
                            
                        }

                        playerbase.IncrementIndex();
                        continue;
                    }
                    
                    playerbase.CurrentPlayer().AddGamesFound(games.Count);

                   // Console.Write(">");
                    foreach (var game in games)// for all games
                    {
                        if (game.GameMode == GameMode.Aram) // if this game is aram game
                        {
                            if (game.CreateDate > playerbase.CurrentPlayer().CheckedUntill)
                            {
                                playerbase.CurrentPlayer().AddAramsFound(1);
                                if (gamebase.AddGame(game.GameId)) // if this game has not yet been found 
                                {
                                    //Console.Write(":");

                                    if (!(game.FellowPlayers == null))
                                    {
                                        foreach (var player in game.FellowPlayers) // for all players in this aram game
                                        {
                                            playerbase.AddPlayer(player.SummonerId);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (games.Count > 0)
                    {
                        playerbase.CurrentPlayer().CheckedUntill = games[0].CreateDate;
                    }
                }
                catch (RiotSharpException ex)
                {
                    // Handle the exception however you want.
                }

                

                playerbase.IncrementIndex();

                Console.WriteLine("checking player " + playerbase.index + " out of " + playerbase.players.Count +" |" + ((float)playerbase.index/playerbase.players.Count*100 ).ToString("0.00") + "%| Games: " + gamebase.games.Count + " Est Total Games:" + (((float)gamebase.games.Count/ playerbase.index)* playerbase.players.Count).ToString("0"));
                Console.WriteLine();
                //playerbase.SaveToFile("DataFiles/playerbase.xml");
                //gamebase.SaveToFile("DataFiles/gamebase.xml");

                //Console.ReadKey();
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
