using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RiotSharp;
using RiotSharp.Misc;

namespace GameFinder
{
    public class Region
    {
        public Region(string name, RiotSharp.Misc.Region region, int regionId)
        {
            this.name = name;
            this.region = region;
            this.regionId = regionId;
        }

        public string name;
        public RiotSharp.Misc.Region region;
        public int regionId;
    }
}
