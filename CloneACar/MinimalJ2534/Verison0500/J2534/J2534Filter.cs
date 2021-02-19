using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minimal_J2534_0500
{
    class J2534Filter
    {
        public string mask;
        public string pattern;
        public uint filterId;
        public FilterDef filterType;

        public J2534Filter(string mask, string pattern, uint filterId, FilterDef filterType)
        {
            this.mask = mask;
            this.pattern = pattern;
            this.filterId = filterId;
            this.filterType = filterType;
        }

        public string AsString()
        {
            return "type: " + filterType + "mask: " + mask + ", pattern: " + pattern + "ID: " + filterId; 
        }

        public string AsStringNoFilterId()
        {
            return "type: " + filterType + "mask: " + mask + ", pattern: " + pattern;
        }
    }
}
