using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minimal_J2534_0500
{
    class J2534PeriodicMsg
    {
        public PassThruMsg msg;
        public uint timeInterval;
        public uint msgId;

        public J2534PeriodicMsg(PassThruMsg msg, uint timeInterval, uint msgId)
        {
            this.msg = msg;
            this.timeInterval = timeInterval;
            this.msgId = msgId;
        }
    }
}
