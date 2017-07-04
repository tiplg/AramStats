using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GameFinder
{
    public class GameBase
    {
        public List<Game> games { get; set; }

        public GameBase() {
            games = new List<Game>();
        }

        //public GameBase() { }

        public bool AddGame(long gameId)
        {
            if (games.Any(g => g.GameId == gameId)) // if not a new game
            {
                return false;
            }
            else
            {
                games.Add(new Game(gameId));
                return true;
            }
        }


        public void SaveToFile(string fileName)
        {
            using (var stream = new FileStream(fileName, FileMode.Create))
            {
                var XML = new XmlSerializer(typeof(GameBase));
                XML.Serialize(stream, this);
            }
        }

        public static GameBase LoadFromFile(string fileName)
        {
            using (var stream = new FileStream(fileName, FileMode.Open))
            {
                var XML = new XmlSerializer(typeof(GameBase));
                return (GameBase)XML.Deserialize(stream);
            }
        }
    }
}
