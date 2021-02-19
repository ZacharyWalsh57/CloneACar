using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minimal_J2534_0500
{
    class J2534ChannelLogical : J2534Channel
    {
        public J2534ChannelPhysical channelPhysical;

        public J2534ChannelLogical(J2534Device j2534Device, J2534ChannelPhysical channelPhysical, uint channelId, ProtocolId protocol, uint flags) : base(j2534Device, channelId, protocol, flags)
        {
            this.channelPhysical = channelPhysical;
        }

        // does a PTSelect on this logical channel
        public SChannelSet PTSelect()
        {
            SChannelSet sc = new SChannelSet();
            sc.channelThreshold = 0;
            sc.channelList.Add((int)channelId);
            sc.channelCount = (uint)sc.channelList.Count;

            j2534Device.JapiMarshal.PassThruSelect(ref sc, 0);

            return sc;
        }
    }
}
