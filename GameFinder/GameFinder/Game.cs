using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RiotSharp.GameEndpoint;

namespace GameFinder
{
    public class Game
    {
        public Game(long gameId)
        {
            GameId = gameId;
        }

        public Game() { }

        public Game(long gameId, List<RiotSharp.GameEndpoint.Player> fellowPlayers)
        {
            GameId = gameId;
            FellowPlayers = fellowPlayers;
        }

        public long GameId { get; set; }
        public List<RiotSharp.GameEndpoint.Player> FellowPlayers;

        public string GetValuesString(Region region)
        {
            return string.Format("(NULL, '{0}', '{1}', '', '', '', ''),", GameId, region.regionId);
        }
    }
}
