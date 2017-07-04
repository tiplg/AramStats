using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GameFinder
{
    public class PlayerBase
    {
        public int index { get; set; }
        public List<Player> players { get; set; }
        
        public PlayerBase()
        {
            index = 0;
            players = new List<Player>();
        }

        public void AddPlayer(long summonerId)
        {
            if (players.Any(p => p.SummonerID == summonerId))
            {
                //Console.Write(".");
            }
            else
            {
                //Console.Write("+");
                players.Add(new Player(summonerId));
            }
        }

        public void IncrementIndex()
        {
            index++;
        }

        public bool PlayersAvailable()
        {
            return (index < players.Count);
        }

        public Player CurrentPlayer()
        {
            return players[index];
        }

        public void SaveToFile(string fileName)
        {
            using (var stream = new FileStream(fileName, FileMode.Create))
            {
                var XML = new XmlSerializer(typeof(PlayerBase));
                XML.Serialize(stream, this);
            }
        }

        public static PlayerBase LoadFromFile(string fileName)
        {
            using (var stream = new FileStream(fileName, FileMode.Open))
            {
                var XML = new XmlSerializer(typeof(PlayerBase));
                return (PlayerBase)XML.Deserialize(stream);
            }
        }
    }
}
