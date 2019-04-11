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

namespace SummonerFinderV4
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
                if (!gameBase.GamesAvailable())
                {
                    tryagain:
                    summonerBase.NewSummonersToDatabase(link);
                    //load new or break
                    if (gameBase.LoadFromDatabase(link, 100))
                    {
                        Console.WriteLine("Loaded new games from database");
                        timeoutTimetime = 1;
                    }
                    else
                    {
                        if (timeoutTimetime > 5)
                        {
                            Console.WriteLine("Could not load new games from database: Breaking");
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

                    var matchResult = api.Match.GetMatchAsync(Region.euw, gameBase.CurrentGame().gameId).Result;

                    foreach (ParticipantIdentity pId in matchResult.ParticipantIdentities)
                    {
                        var summoner = pId.Player;
                        //Console.WriteLine(match.PlatformID.GetHashCode() + " " + match.Region.GetHashCode());
                        summonerBase.AddUniqueSummoner(new Summoner(0, summoner.SummonerName, null, summoner.SummonerId, summoner.CurrentAccountId, summoner.CurrentPlatformId.GetHashCode(), summoner.ProfileIcon, 0, 0, 0, DateTime.FromBinary(0)));
                    }

                    Console.WriteLine("added summoners from game: " + gameBase.CurrentGame().gameId);
                }
                catch (Exception ex)
                {
                    // Handle the exception however you want.

                    if (ex.InnerException.Message == "404, Resource not found")
                    {
                        Console.WriteLine("Error 404: " + gameBase.CurrentGame().gameId); // TODO 
                    }
                    else
                        Console.WriteLine(ex.ToString());

                    System.Threading.Thread.Sleep(10000); 
                }

                gameBase.CurrentGame().scrapeIndex = 1;
                gameBase.NextGame();
            }
            
            End:
            Console.WriteLine("Hello World!");
            Console.ReadKey();

        }

    }
}
