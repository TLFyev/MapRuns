using Dalamud.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapRuns
{
    public class MapRunLoot
    {
        public int gil { get; set; } = 0;
        public int chests { get; set; } = 0;
        public int portals { get; set; } = 0;
        public int poetics { get; set; } = 0;
        public int uncapped { get; set; } = 0;
        public int capped { get; set; } = 0;
        public List<string>? tempItems { get; set; } = null;
        public Dictionary<string, string>? goodItems { get; set; } = null;
        public Dictionary<string, string>? commonItems { get; set; } = null;

        public MapRunLoot()
        {
            this.gil = 0;
            this.chests = 0;
            this.portals = 0;
            this.poetics = 0;
            this.uncapped = 0;
            this.capped = 0;

            this.tempItems = new List<string>();
            this.goodItems = new Dictionary<string, string>();
            this.commonItems = new Dictionary<string, string>();
        }

        public void ClearAll()
        {
            this.gil = 0;
            this.chests = 0;
            this.portals = 0;
            this.poetics = 0;
            this.uncapped = 0;
            this.capped = 0;

            this.tempItems = new List<string>();
            this.goodItems = new Dictionary<string, string>();
            this.commonItems = new Dictionary<string, string>();
        }

        public string GetPrettyPrintGil()
        {
            if (this.gil >0)
            {
                return (this.gil + (this.gil % 1000)).ToString("N0");
            }
            else
            {
                return "0";
            }
        }
    }
}
