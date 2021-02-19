using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minimal_J2534_0500
{
    class J2534ChannelPhysical : J2534Channel
    {
        public uint baudrate;

        public J2534ChannelLogical[] logicalChannels;

        public const uint maxLogicalChannels = 10;

        public J2534ChannelPhysical(J2534Device j2534Device, uint channelId, ProtocolId protocol, uint flags, uint baudrate) : base(j2534Device, channelId, protocol, flags)
        {
            this.baudrate = baudrate;

            logicalChannels = new J2534ChannelLogical[maxLogicalChannels];
        }

        public void PTLogicalConnect(int index, Iso15765ChannelDescriptor cD)
        {
            if (logicalChannels[index] != null)
            {
                throw new AppException("logical channel slot " + index + " already contains a logical channel");
            }

            uint ch;
            j2534Device.JapiMarshal.PassThruLogicalConnect(channelId, ProtocolId.ISO15765_LOGICAL, 0, cD, out ch);

            logicalChannels[index] = new J2534ChannelLogical(j2534Device, this, ch, ProtocolId.ISO15765_LOGICAL, 0);
        }

        public void PTLogicalDisconnect(int index)
        {
            j2534Device.JapiMarshal.PassThruLogicalDisconnect(logicalChannels[index].channelId);
            logicalChannels[index] = null;
        }

        // does a PTSelect on this channel, plus all logical channels on this physical channel
        public SChannelSet PTSelect()
        {
            SChannelSet sc = new SChannelSet();
            sc.channelThreshold = 0;

            sc.channelList.Add((int)channelId);

            foreach (J2534ChannelLogical ch in logicalChannels)
            {
                sc.channelList.Add((int)ch.channelId);
            }

            sc.channelCount = (uint)sc.channelList.Count;

            j2534Device.JapiMarshal.PassThruSelect(ref sc, 0);

            return sc;
        }
    }
}
