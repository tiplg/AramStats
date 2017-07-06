using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace StatScraper
{
    public class ChampStatBase
    {
        public List<ChampStat> champStats { get; set; }

        public ChampStatBase()
        {
            champStats = new List<ChampStat>();
        }

        public void SaveToFile(string fileName)
        {
            using (var stream = new FileStream(fileName, FileMode.Create))
            {
                var XML = new XmlSerializer(typeof(ChampStatBase));
                XML.Serialize(stream, this);
            }
        }

        public static ChampStatBase LoadFromFile(string fileName)
        {
            using (var stream = new FileStream(fileName, FileMode.Open))
            {
                var XML = new XmlSerializer(typeof(ChampStatBase));
                return (ChampStatBase)XML.Deserialize(stream);
            }
        }
    }
}
