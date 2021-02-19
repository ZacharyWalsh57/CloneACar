using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minimal_J2534_0404
{
    class J2534Filter
    {
        public string type;
        public uint flags;

        public string mask;
        public string pattern;
        public string flowCtl;
        public uint filterId;


        public J2534Filter(string mask, string pattern, uint filterId)
        {
            this.mask = mask;
            this.pattern = pattern;
            this.filterId = filterId;
        }

        public J2534Filter(string type, string mask, string pattern, string flowCtl, uint flags, uint filterId)
        {
            this.type = type;
            this.mask = mask;
            this.pattern = pattern;
            this.flowCtl = flowCtl;
            this.filterId = filterId;
            this.flags = flags;
        }

        public J2534Filter(string type, string mask, string pattern, uint flags, uint filterId)
        {
            this.type = type;
            this.flags = flags;
            this.mask = mask;
            this.pattern = pattern;
            this.filterId = filterId;
        }

        public J2534Filter(string mask, string pattern)
        {
            this.mask = mask;
            this.pattern = pattern;
            this.filterId = 0;
        }

        public string AsString()
        {
            return "mask: " + mask + ", pattern: " + pattern + "ID: " + filterId; 
        }

        public string AsStringNoFilterId()
        {
            return "mask: " + mask + ", pattern: " + pattern;
        }
    }
}
