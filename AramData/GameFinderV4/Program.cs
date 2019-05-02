using AramData;
using MySql.Data.MySqlClient;
using RiotSharp;
using RiotSharp.Endpoints.MatchEndpoint;
using RiotSharp.Misc;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GameFinderV4
{
    class Program
    {
        static void Main(string[] args)
        {
            int ClientNumber = -1;
            try
            {
                ClientNumber = Convert.ToInt32(args[0]);
            }
            catch (Exception)
            {
                Console.WriteLine("Enter client number argument");
                Console.ReadKey();
                return;
            }
            Console.WriteLine("Client Number: " + ClientNumber.ToString());


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
            //var api = RiotApi.(ConfigurationManager.AppSettings["RiotApiKey"], 500, 30000);
            var api = RiotApi.GetInstance(ConfigurationManager.AppSettings["RiotApiKey"], 495, 29500);

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
                    if (summonerBase.LoadFromDatabase(link, ClientNumber*5000, 100))
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

                if(gameBase.gameList.Count> 10000)
                {
                    gameBase.NewGamesToDatabase(link);
                    summonerBase.UpdateSummonersToDatabase(link);
                }

                try
                {
                    int beginIndex = 0;
                    MatchList gameListResult;
                    DateTime checkedUntil = DateTime.FromBinary(0);

                    do
                    {
                        gameListResult = api.Match.GetMatchListAsync(Region.euw, summonerBase.CurrentSummoner().accountId, null, new List<int>(new int[] { 450 }), null, summonerBase.CurrentSummoner().checkedUntil, null, beginIndex, null).Result;

                        foreach (MatchReference match in gameListResult.Matches)
                        {
                            gameBase.AddNewGame(match.GameId, match.PlatformID.GetHashCode(), match.Season.GetHashCode(), match.Timestamp);
                        }

                        

                        if (beginIndex == 0) //only run first loop
                        {
                            checkedUntil = gameListResult.Matches.Max(t => t.Timestamp).AddSeconds(1);
                        }

                        beginIndex += 100;
                    } while (gameListResult.TotalGames > beginIndex);

                    summonerBase.CurrentSummoner().AddGamesFound(gameListResult.TotalGames, checkedUntil);

                    Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + "\t" + gameListResult.TotalGames.ToString() + " games added from summoner: " + summonerBase.CurrentSummoner().name);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());

                    summonerBase.CurrentSummoner().AddGamesFound(0, DateTime.FromBinary(0));
                    System.Threading.Thread.Sleep(10000);

                }

                summonerBase.NextSummoner();
            }


        End:
            Console.WriteLine("Hello World!");
            Console.ReadKey();

        }
    }
}
