using AramData;
using RiotSharp;
using MySql.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using RiotSharp.Misc;
using RiotSharp.Endpoints.MatchEndpoint;
using System.Configuration;
using System.Diagnostics;

namespace SummonerFinderV4
{
    class Program
    {
        static void Main(string[] args)
        {
            MySqlConnection link;
            Stopwatch stopWatch = new Stopwatch();
            link = new MySqlConnection(ConfigurationManager.AppSettings["MySqlConnectionString"]);
            int timeoutTimetime = 1;
            int sample = 30 * 30;

            while (!link.Ping())
            {
                try
                {
                    link.Open();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

                System.Threading.Thread.Sleep(10000);
            }

            Console.WriteLine("Conncection to Database: " + link.Ping());
            //var api = RiotApi.GetDevelopmentInstance(ConfigurationManager.AppSettings["RiotApiKey"]);
            var api = RiotApi.GetInstance(ConfigurationManager.AppSettings["RiotApiKey"],495,29500);


            try
            {
                var test = api.Champion.GetChampionRotationAsync(Region.euw);
                var result = test.Result.FreeChampionIds;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Connection to Api: " + ex.ToString());
                goto End;
            }

            Console.WriteLine("Connection to Api: True");

            var gameBase = new GameBase();
            var summonerBase = new SummonerBase();

            //var sips = new Summoner("SipsClar", null, null, "0fhpK-H2m0-tS_xeHpBRyXL9Lzu_uGTNts9cwCF36BJ-FnU", 0, 0, 0, 0, 0, new DateTime(0));

            //summonerBase.AddSummoner(sips);

            //summonerBase.LoadFromDatabase(link, 100);
            
            while (true)
            {
                if (gameBase.GamesScrapable(1) < 1)
                {
                    tryagain:
                    summonerBase.NewSummonersToDatabase(link);
                    //load new or break
                    if (gameBase.LoadFromDatabase(link, 30*30))
                    {
                        timeoutTimetime = 1;
                        Console.WriteLine("Loaded "+ sample + " new games from database");
                    }
                    else
                    {
                        if (timeoutTimetime > 5)
                        {
                            //Console.WriteLine("Could not load new games from database: Breaking");
                            break;
                        }
                        Console.WriteLine("Could not load new games from database: Timeout " + timeoutTimetime + " minute");
                        System.Threading.Thread.Sleep(timeoutTimetime * 60000);
                        timeoutTimetime++;
                        goto tryagain;
                    }
                }
                //here

                try
                {
                    /*
                    var matchResult = api.Match.GetMatchAsync(Region.euw, gameBase.CurrentGame().gameId).Result;

                    foreach (ParticipantIdentity pId in matchResult.ParticipantIdentities)
                    {
                        var summoner = pId.Player;
                        //Console.WriteLine(match.PlatformID.GetHashCode() + " " + match.Region.GetHashCode());
                        summonerBase.AddUniqueSummoner(new Summoner(0, summoner.SummonerName, null, summoner.SummonerId, summoner.CurrentAccountId, summoner.CurrentPlatformId.GetHashCode(), summoner.ProfileIcon, 0, 0, 0, DateTime.FromBinary(0)));
                    }

                    Console.WriteLine("added summoners from game: " + gameBase.CurrentGame().gameId);
                    */

                    //AddSummonersFromMatchAsync(api,gameBase.CurrentGame(),summonerBase,gameBase).Wait();
                    
                    stopWatch.Start();
                    var results = AddSummonersFromMatchAsync(api,summonerBase,gameBase,30).Result;
                    stopWatch.Stop();

                    foreach (var match in results)
                    {
                        foreach (ParticipantIdentity pId in match.ParticipantIdentities)
                        {
                            var summoner = pId.Player;
                            //Console.WriteLine(match.PlatformID.GetHashCode() + " " + match.Region.GetHashCode());
                            summonerBase.AddUniqueSummoner(new Summoner(0, summoner.SummonerName, null, summoner.SummonerId, summoner.CurrentAccountId, summoner.CurrentPlatformId.GetHashCode(), summoner.ProfileIcon, 0, 0, 0, DateTime.FromBinary(0)));
                        }
                    }

                    
                    Console.WriteLine(gameBase.gameList.Count-gameBase.GamesScrapable(1) +"/" + gameBase.gameList.Count + " Batch time: " + stopWatch.ElapsedMilliseconds);
                    stopWatch.Reset();
                    //System.Threading.Thread.Sleep(3000);
                }
                catch (Exception ex)
                {
                    // Handle the exception however you want.
                    Console.WriteLine(ex.ToString());

                    System.Threading.Thread.Sleep(10000); 
                }

                //gameBase.CurrentGame().scrapeIndex = 1;
                //gameBase.NextGame();
            }
            
            End:
            Console.WriteLine("Hello World!");
            Console.ReadKey();

        }

        private static async Task<Match[]> AddSummonersFromMatchAsync(RiotApi api, SummonerBase summonerBase, GameBase gameBase, int limit)
        {
            List<Task<Match>> tasks = new List<Task<Match>>();

            foreach (var game in gameBase.gameList)
            {
                if (limit < 1) break;

                if (game.scrapeIndex == 0)
                {
                    tasks.Add(api.Match.GetMatchAsync(Region.euw, game.gameId));     
                    game.scrapeIndex = 1;

                    limit--;
                }
            }

            try
            {
                return await Task.WhenAll(tasks);
            }
            catch (RiotSharpException ex)
            {
                Console.WriteLine(ex.HttpStatusCode);
                System.Threading.Thread.Sleep(10000);
                return null;
            }
           
            

            /*
            var matchResult = await api.Match.GetMatchAsync(Region.euw, game.gameId);

            foreach (ParticipantIdentity pId in matchResult.ParticipantIdentities)
            {
                var summoner = pId.Player;
                //Console.WriteLine(match.PlatformID.GetHashCode() + " " + match.Region.GetHashCode());
                summonerBase.AddUniqueSummoner(new Summoner(0, summoner.SummonerName, null, summoner.SummonerId, summoner.CurrentAccountId, summoner.CurrentPlatformId.GetHashCode(), summoner.ProfileIcon, 0, 0, 0, DateTime.FromBinary(0)));
            }

            gameBase.gameList.Remove(game);
            game.scrapeIndex = 1;
            gameBase.gamesToUpdate.Add(game);

            //Console.WriteLine("added summoners from game: " + game.gameId);
            */
        }



    }
}
