using AramData;
using MySql.Data.MySqlClient;
using RiotSharp;
using RiotSharp.Endpoints.MatchEndpoint;
using RiotSharp.Misc;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
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
            Stopwatch stopWatch1 = new Stopwatch();
            Stopwatch stopWatch2 = new Stopwatch();
            long countje = 0;

            int ClientNumber = -1;
            try
            {
                ClientNumber = Convert.ToInt32(args[0]);
                Console.Title = "GameFinderV4 Client #" + ClientNumber.ToString();
                Console.WriteLine("Client Number: " + ClientNumber.ToString());
            }
            catch (Exception)
            {
                Console.WriteLine("Enter client number argument");
                Console.ReadKey();
                return;
            }
            


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

            stopWatch2.Start();

            while (true)
            {
                if (!summonerBase.SummonersAvailable())
                {
                tryagain:
                    gameBase.NewGamesToDatabase(link);
                    //load new or break
                    if (summonerBase.LoadFromDatabase(link, ClientNumber*5000, 500))
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

                if(gameBase.gameList.Count > 10000)
                {
                    //stopWatch2.Stop();
                    Console.WriteLine("batchtime: " + stopWatch2.ElapsedMilliseconds / countje);
                    //stopWatch2.Reset();
                    //stopWatch2.Start();

                    gameBase.NewGamesToDatabase(link);
                    summonerBase.UpdateSummonersToDatabase(link);

                }


                int beginIndex = 0;
                MatchList gameListResult = new MatchList();
                DateTime checkedUntil = DateTime.FromBinary(0);
                stopWatch1.Start();
                do
                {
                    try
                    {
                        gameListResult = api.Match.GetMatchListAsync(Region.euw, summonerBase.CurrentSummoner().accountId, null, new List<int>(new int[] { 450 }), null, summonerBase.CurrentSummoner().checkedUntil, null, beginIndex, null).Result;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error with Summoner: " + summonerBase.CurrentSummoner().name);
                        //Console.WriteLine(e.ToString());

                        string em = e.InnerException.Message;

                        if (e.InnerException.Message.Contains("A task"))
                        {
                            Console.WriteLine(em);
                            api = RiotApi.GetInstance(ConfigurationManager.AppSettings["RiotApiKey"], 495, 29500);
                        }
                        else if (em.StartsWith("500") || em.StartsWith("503") || em.StartsWith("504") || em.StartsWith("429"))
                        {
                            Console.WriteLine(em);
                        }
                        else
                        {
                            Console.WriteLine("\nOther Error\n");
                            Console.WriteLine(e.ToString());
                        }

                        System.Threading.Thread.Sleep(5000);
                    }

                    if (gameListResult.Matches != null)
                    {
                        foreach (MatchReference match in gameListResult.Matches)
                        {
                            gameBase.AddNewGame(match.GameId, match.PlatformID.GetHashCode(), match.Season.GetHashCode(), match.Timestamp);
                        }

                        if (beginIndex == 0) //only run first loop
                        {
                            checkedUntil = gameListResult.Matches.Max(t => t.Timestamp).AddSeconds(1);
                        }

                        beginIndex += 100;
                        countje++;
                    }
                    else
                    {
                        continue;
                    }

                    
                } while (gameListResult.TotalGames > beginIndex);
                if (gameListResult.Matches != null)
                {
                    stopWatch1.Stop();

                    summonerBase.CurrentSummoner().AddGamesFound(gameListResult.TotalGames, checkedUntil);

                    Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + "  dt: " + stopWatch1.ElapsedMilliseconds / ((gameListResult.TotalGames + 99) / 100) + "\t" + gameListResult.TotalGames.ToString() + " games added from summoner: " + summonerBase.CurrentSummoner().name);
                    stopWatch1.Reset();

                    summonerBase.NextSummoner();
                }
            }


        End:
            Console.WriteLine("Hello World!");
            Console.ReadKey();

        }
    }
}
