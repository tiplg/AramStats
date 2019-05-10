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
            int ClientNumber = -1;
            try
            {
                ClientNumber = Convert.ToInt32(args[0]);
                Console.Title = "SummonerFinderV4 Client #" + ClientNumber.ToString();
                Console.WriteLine("Client Number: " + ClientNumber.ToString());
            }
            catch (Exception)
            {
                Console.WriteLine("Enter client number argument");
                Console.ReadKey();
                return;
            }

            MySqlConnection link;
            Stopwatch stopWatch = new Stopwatch();
            link = new MySqlConnection(ConfigurationManager.AppSettings["MySqlConnectionString"]);
            int timeoutTimetime = 1;

            int batch = 50;
            int sample = 500;
            

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

            stopWatch.Start();

            while (true)
            {
                if (gameBase.GamesScrapable(1) < 1)
                {
                    tryagain:
                    summonerBase.NewSummonersToDatabase(link);
                    //load new or break
                    if (gameBase.LoadFromDatabase(link, ClientNumber*5000, sample))
                    {
                        timeoutTimetime = 1;
                        Console.WriteLine("Loaded " + sample + " new games from database, First Game: " + gameBase.gameList[0].gameId);
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
                Match matchData = null;
                Game game = gameBase.gameList.Find(g => g.scrapeIndex < 1);

                
                try
                {
                    matchData = api.Match.GetMatchAsync(Region.euw, game.gameId).Result;
                    game.scrapeIndex = 1;
                }
                catch (AggregateException e)
                {
                    Console.WriteLine("Error with game: " + game.gameId);
                    //Console.WriteLine(e.ToString());

                    string em = e.InnerException.Message;

                    if (e.InnerException.Message.Contains("A task"))
                    {
                        Console.WriteLine(em);
                        api = RiotApi.GetInstance(ConfigurationManager.AppSettings["RiotApiKey"], 495, 29500);
                    }
                    else if(em.StartsWith("500") || em.StartsWith("503") || em.StartsWith("504")  || em.StartsWith("429"))
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
                

                if (matchData != null)
                {
                    foreach (ParticipantIdentity pId in matchData.ParticipantIdentities)
                    {
                        var summoner = pId.Player;
                        //Console.WriteLine(match.PlatformID.GetHashCode() + " " + match.Region.GetHashCode());
                        summonerBase.AddUniqueSummoner(new Summoner(0, summoner.SummonerName, null, summoner.SummonerId, summoner.CurrentAccountId, summoner.CurrentPlatformId.GetHashCode(), summoner.ProfileIcon, 0, 0, 0, DateTime.FromBinary(0)));
                    }
                }

                if ((gameBase.gameList.Count - gameBase.GamesScrapable(1))%batch == 0)
                {
                    stopWatch.Stop();
                    Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + "\t" + (gameBase.gameList.Count - gameBase.GamesScrapable(1)) + "/" + gameBase.gameList.Count + " Dt: " + stopWatch.ElapsedMilliseconds / batch);
                    stopWatch.Reset();
                    stopWatch.Start();
                }


            /*
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


                //AddSummonersFromMatchAsync(api,gameBase.CurrentGame(),summonerBase,gameBase).Wait();

                stopWatch.Start();
                //var results = GetMatchesFromApiAsync(api,summonerBase,gameBase,30);
                //var results = api.Match.GetMatchAsync(Region.euw,)
                var results = GetMatchesFromApiParallel(api, gameBase, 30);
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


                Console.WriteLine(gameBase.gameList.Count-gameBase.GamesScrapable(1) +"/" + gameBase.gameList.Count + " Dt: " + stopWatch.ElapsedMilliseconds/batch);
                stopWatch.Reset();
                //System.Threading.Thread.Sleep(3000);
            }
            catch (Exception ex)
            {
                // Handle the exception however you want.
                Console.WriteLine(ex.ToString());

                //System.Threading.Thread.Sleep(10000); 
            }

            //gameBase.CurrentGame().scrapeIndex = 1;
            //gameBase.NextGame();
            */
        }
        
            End:
            Console.WriteLine("Hello World!");
            Console.ReadKey();

        }

        public static List<Match> GetMatchesFromApiParallel(RiotApi api, GameBase gameBase, int limit)
        {
            List<Match> output = new List<Match>();

            Parallel.ForEach<Game>(gameBase.gameList.FindAll(g => g.scrapeIndex < 1).GetRange(0,limit), async (game) =>
            {
                Match result = DownloadMatch(api, Region.euw, game.gameId).Result;

                try
                {

                }
                catch (Exception e)
                {
                    throw;
                }

                if (result != null)
                {
                    output.Add(result);
                    game.scrapeIndex = 1;
                }
                else
                {
                    Console.WriteLine(game.gameId);
                }
                
            });

            return output;
        }

        private static async Task<Match> DownloadMatch(RiotApi api, Region region, long gameId)
        {
            Match matchData;
            try
            {
                matchData = await api.Match.GetMatchAsync(region, gameId);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                matchData = null;
            }

            return matchData;

        }

        private static  List<Match> GetMatchesFromApiAsync(RiotApi api, SummonerBase summonerBase, GameBase gameBase, int limit)
        {
            List<Task<Match>> tasks = new List<Task<Match>>();
            List<Match> matchList = new List<Match>();

            foreach (var game in gameBase.gameList)
            {
                if (limit < 1) break;

                if (game.scrapeIndex == 0)
                {
                    tasks.Add(api.Match.GetMatchAsync(Region.euw, game.gameId));    
                    //tasks.Add(DownloadMatch(api, Region.euw, game.gameId));
                    game.scrapeIndex = 1; //TODO check if actually scraped

                    limit--;
                }
            }

            try
            {
                Task.WhenAll(tasks);
            }
            catch (RiotSharpException ex)
            {
                Console.WriteLine(ex.HttpStatusCode);
                
                //tasks.Clear();
                
                foreach (var task in tasks)
                {
                    task.Dispose();
                    //Console.WriteLine("Task - IsFaulted: " + task.IsFaulted + "     IsCanceled: " + task.IsCanceled + "     IsCompleted: " + task.IsCompleted);

                   
                }
                
                //System.Threading.Thread.Sleep(10000);
                
            }

            foreach (var match in tasks)
            {
                if(match.Result != null)
                {
                    matchList.Add(match.Result);
                }
            }

            return matchList;
            

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
