using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RiotSharp;
using RiotSharp.Misc;

namespace GameFinder
{
    class Region
    {
        public Region(string name, RiotSharp.Misc.Region region, string folder)
        {
            this.name = name;
            this.region = region;
            this.folder = folder;
        }

        public string name;
        public RiotSharp.Misc.Region region;
        public string folder;
    }
}
