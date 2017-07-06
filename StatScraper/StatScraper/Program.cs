using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using RiotSharp;
using RiotSharp.Misc;
using RiotSharp.MatchEndpoint;

namespace StatScraper
{
    class Program
    {
        static void Main(string[] args)
        {
            var apikey = ConfigurationManager.AppSettings["apikey"];

            Console.WriteLine("waddup boois");
            Console.WriteLine(apikey);

            var api = RiotApi.GetInstance(apikey, 8, 500); // , 7, 500

            GameBase gamebase = GameBase.LoadFromFile("DataFiles/gamebaseEX.xml");
            GameBase gamebaseToAdd = GameBase.LoadFromFile("DataFiles/gamebase.xml");
            int gamesToAdd = 0; //gamebaseToAdd.games.Count(g => !gamebase.games.Any(p => g.GameId == p.GameId));
            int index = 0;

            Console.WriteLine("Games to Add: " + gamesToAdd);

            ChampStatBase champStatBase = ChampStatBase.LoadFromFile("DataFiles/ChampStatBase.xml"); // new ChampStatBase();//

            var timer = new System.Threading.Timer((e) =>
            {
                gamebase.SaveToFile("DataFiles/gamebaseEX.xml");
                champStatBase.SaveToFile("DataFiles/ChampStatBase.xml");

                Console.WriteLine("SAVING FILES");
            }, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));

            foreach (Game game in gamebaseToAdd.games)
            {
                if (!gamebase.games.Any(g => g.GameId == game.GameId)) // if not already in de gamebase
                {

                    MatchDetail apiGame = null;
                    try
                    {
                        apiGame = api.GetMatch(Region.euw, game.GameId);
                    }
                    catch (Exception)
                    {
                        continue;
                    }

                    gamebase.games.Add(new Game(game.GameId, apiGame.MatchCreation, 1, (int)apiGame.MatchDuration.TotalSeconds, apiGame.Region));

                    foreach (var participant in apiGame.Participants)
                    {
                        champStatBase.champStats.Add(new ChampStat(game.GameId, participant));
                    }

                    index++;

                    Console.WriteLine("GameId: " + game.GameId + " Game " + index + " out of " + gamesToAdd + " Total Games: " + gamebase.games.Count());
                }
            }


            Console.WriteLine("All Done! press any key to close.");
            Console.ReadKey();
        }
    }
}
