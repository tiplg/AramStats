using AramData;
using MySql.Data.MySqlClient;
using RiotSharp;
using RiotSharp.Endpoints.MatchEndpoint;
using RiotSharp.Misc;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameFinderV4
{
    class Program
    {
        static void Main(string[] args)
        {
            MySqlConnection link;
            link = new MySqlConnection(ConfigurationManager.AppSettings["MySqlConnectionString"]);
            int timeoutTimetime = 1;

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

                System.Threading.Thread.Sleep(1000);
            }

            Console.WriteLine("Conncection to Database: " + link.Ping());
            var api = RiotApi.GetDevelopmentInstance(ConfigurationManager.AppSettings["RiotApiKey"]);

            try
            {
                var test = api.Champion.GetChampionRotationAsync(Region.euw);
                var result = test.Result.FreeChampionIds;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Connection to Api: " + ex.InnerException.Message);
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
                if (!summonerBase.SummonersAvailable())
                {
                tryagain:
                    gameBase.NewGamesToDatabase(link);
                    //load new or break
                    if (summonerBase.LoadFromDatabase(link, 100))
                    {
                        Console.WriteLine("Loaded new players from database");
                        timeoutTimetime = 1;
                    }
                    else
                    {
                        if (timeoutTimetime > 5)
                        {
                            Console.WriteLine("Could not load new players from database: Breaking");
                            break;
                        }
                        Console.WriteLine("Could not load new players from database: Timeout " + timeoutTimetime + " minute");
                        System.Threading.Thread.Sleep(timeoutTimetime * 60000);
                        timeoutTimetime++;
                        goto tryagain;
                    }
                }

                try
                {

                    var gameListResult = api.Match.GetMatchListAsync(Region.euw, summonerBase.CurrentSummoner().accountId, null, new List<int>(new int[] { 450 }), null, summonerBase.CurrentSummoner().checkedUntil, null, null, null);

                    foreach (MatchReference match in gameListResult.Result.Matches)
                    {
                        //Console.WriteLine(match.PlatformID.GetHashCode() + " " + match.Region.GetHashCode());

                        gameBase.AddNewGame(match.GameId, Region.euw.GetHashCode(), match.Season.GetHashCode(), match.Timestamp);
                    }

                    summonerBase.CurrentSummoner().AddGamesFound(gameListResult.Result);

                    Console.WriteLine(gameListResult.Result.Matches.Count.ToString() + " games added from summoner: " + summonerBase.CurrentSummoner().name);
                }
                catch (Exception ex)
                {
                    // Handle the exception however you want.

                    if (ex.InnerException.Message == "404, Resource not found")
                    {
                        Console.WriteLine("No new games for summoner: " + summonerBase.CurrentSummoner().name); // TODO 
                        summonerBase.CurrentSummoner().NoGamesFound();
                    }
                    else
                        Console.WriteLine(ex.ToString());
                }

                summonerBase.NextSummoner();
            }


        End:
            Console.WriteLine("Hello World!");
            Console.ReadKey();

        }
    }
}
