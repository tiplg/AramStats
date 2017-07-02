using RiotSharp;
using RiotSharp.Misc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace GameFinder
{
    class Program
    {
        static void Main(string[] args)
        {
            var apikey = ConfigurationManager.AppSettings["apikey"];

            Console.WriteLine("waddup boois");
            Console.WriteLine(apikey);

            var api = RiotApi.GetInstance(apikey);

            List<long> gamesList = new List<long>();

            List<long> idList = new List<long>();
            int index = 0;

            try
            {
                var summoner = api.GetSummonerByName(Region.euw, "sipsclar"); //api.GetSummoner(Region.euw, "sipsclar");
                Console.WriteLine(summoner.Id);
                idList.Add(summoner.Id);
            }
            catch (RiotSharpException ex)
            {
                // Handle the exception however you want.
            }

            while (index < idList.Count)
            {

                try
                {
                    Console.WriteLine("Checking games from " + idList[index]);
                    var games = api.GetRecentGames(Region.euw, idList[index]);

                    Console.Write(">");
                    foreach (var game in games)
                    {

                        if (game.GameMode == GameMode.Aram)
                        {
                            Console.Write(":");
                            if (!gamesList.Contains(game.GameId))
                            {
                                gamesList.Add(game.GameId);
                                using (StreamWriter gamefile = new StreamWriter(@"games.txt", true))
                                {
                                    gamefile.WriteLine(game.GameId.ToString());
                                }
                            }

                            foreach (var player in game.FellowPlayers)
                            {
                                //Console.WriteLine(player.SummonerId);
                                if (idList.Contains(player.SummonerId))
                                { // not new
                                    Console.Write(".");
                                }
                                else
                                {   // new aram player
                                    if (!(player.SummonerId == 32341516L))
                                    {
                                        Console.Write("+");
                                        idList.Add(player.SummonerId);

                                        using (StreamWriter gamefile = new StreamWriter(@"sums.txt", true))
                                        {
                                            gamefile.WriteLine(player.SummonerId.ToString());
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (RiotSharpException ex)
                {
                    // Handle the exception however you want.
                }

                Console.WriteLine();

                index++;

                Console.WriteLine("index: " + index + " Accounts: " + idList.Count + " Games: " + gamesList.Count);
                //Console.ReadKey();
            }

            /*
            try //222822326
            {
                var list = api.GetMatchList(Region.euw, 76467553L);
                
                foreach (var match in list.Matches)
                {
                    Console.WriteLine(match.Timestamp.ToString("dd-MM-yyyy HH:mm") + " Queue: " + match.Queue.ToString() + " ID: " + match.MatchID.ToString());
                }
                
            }
            catch (RiotSharpException ex)
            {
                Console.WriteLine(ex.ToString());
            }
            */

            Console.ReadKey();
        }
    }
}
