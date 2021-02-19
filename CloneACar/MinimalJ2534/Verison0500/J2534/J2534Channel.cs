using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minimal_J2534_0500
{
    class J2534Channel
    {
        protected J2534Device j2534Device;
        public ProtocolId protocol;
        public uint channelId;
        public uint flags;

        public J2534Filter[] filters;
        public J2534PeriodicMsg[] periodicMsgs;

        public const uint maxFilters = 10;
        public const uint maxPeriodicMsgs = 10;

        public J2534Channel(J2534Device j2534Device, uint channelId, ProtocolId protocol, uint flags)
        {
            this.j2534Device = j2534Device;
            this.protocol = protocol;
            this.channelId = channelId;
            this.flags = flags; 

            filters = new J2534Filter[maxFilters];
            periodicMsgs = new J2534PeriodicMsg[maxPeriodicMsgs];
        }

        public void SetFilter(int index, FilterDef filterType, string mask, string pattern, bool is29bit = false)
        {
            if (filters[index] != null)
            {
                throw new AppException("filter slot " + index + " already contains a filter");
            }

            uint flag = 0;
            if (is29bit)
            {
                flag = (uint)TxFlags.CAN_29BIT_ID;
            }

            PassThruMsg msgMask = J2534Device.CreatePTMsgFromString(protocol, flag, mask);
            PassThruMsg msgPattern = J2534Device.CreatePTMsgFromString(protocol, flag, pattern);

            uint filterId;
            j2534Device.JapiMarshal.PassThruStartMsgFilter(channelId, filterType, msgMask, msgPattern, out filterId);

            filters[index] = new J2534Filter(mask, pattern, filterId, filterType);
        }

        public void RemoveFilter(int index)
        {
            if (filters[index] != null)
            {
            j2534Device.JapiMarshal.PassThruStopMsgFilter(channelId, filters[index].filterId);
            filters[index] = null;
        }
        }

        private J2534Filter FindFilter(int filterId)
        {
            foreach (J2534Filter f in filters)
            {
                if (f.filterId == filterId)
                {
                    return f;
                }
            }
            return null;
        }

        public void RemoveAllFilters()
        {
            j2534Device.JapiMarshal.PassThruIoctl(channelId, IoctlId.CLEAR_MSG_FILTERS);
            for (int i = 0; i < maxFilters; i++)
            {
                filters[i] = null;
            }
        }

        private J2534PeriodicMsg FindPeriodicMsg(uint id)
        {
            foreach (J2534PeriodicMsg p in periodicMsgs)
            {
                if (p.msgId == id)
                {
                    return p;
                }
            }
            return null;
        }

        public PassThruMsg[] PTReadMsgs(ref uint numMsgs, uint timeout)
        {
            PassThruMsg[] msg; 
            j2534Device.JapiMarshal.PassThruReadMsgs(channelId, out msg, ref numMsgs, timeout, protocol);
            return msg;
        }

        /*
        public PassThruMsg[] PTReadMsgsEfficient(ref uint numMsgs, uint timeout)
        {
            PassThruMsg[] msg;
            j2534Device.JapiMarshal.PassThruReadMsgsEfficient(channelId, out msg, ref numMsgs, timeout);
            return msg;
        }
        */

        public uint PTQueueMsgs(PassThruMsg[] msgs)
        {
            uint numMsgs = (uint)msgs.Length;
            j2534Device.JapiMarshal.PassThruQueueMsgs(channelId, msgs, ref numMsgs);
            return numMsgs;
        }

        public uint PTQueueMsgs(PassThruMsg msg)
        {
            uint numMsgs = 1;
            j2534Device.JapiMarshal.PassThruQueueMsgs(channelId, msg, ref numMsgs);
            return numMsgs;
        }

        public uint PTWriteMsgs(PassThruMsg[] msg, uint timeout)
        {
            uint numMsgs = 1;
            j2534Device.JapiMarshal.PassThruWriteMsgs(channelId, msg, ref numMsgs, timeout);
            return numMsgs;
        }

        public void PTStartPeriodicMsg(int index, PassThruMsg msg, uint timeInterval)
        {
            if (periodicMsgs[index] != null)
            {
                throw new AppException("periodic message slot " + index + " already contains a periodic message");
            }

            uint msgId;
            j2534Device.JapiMarshal.PassThruStartPeriodicMsg(channelId, msg, out msgId, timeInterval);
            periodicMsgs[index] =  new J2534PeriodicMsg(msg, timeInterval, msgId);
        }

        public void PTStopPeriodicMsg(int index)
        {
            j2534Device.JapiMarshal.PassThruStopPeriodicMsg(channelId, periodicMsgs[index].msgId);
            periodicMsgs[index] = null;
        }

        public void ClearRxQueue()
        {
            j2534Device.JapiMarshal.PassThruIoctl(channelId, IoctlId.CLEAR_RX_QUEUE);
        }

        public void ClearTxQueue()
        {
            j2534Device.JapiMarshal.PassThruIoctl(channelId, IoctlId.CLEAR_TX_QUEUE);
        }

        public uint GetConfig(ConfigParamId configParam)
        {
            SConfigList scl = new SConfigList();
            scl.numOfParams = 1;

            SConfig sc = new SConfig(configParam);

            scl.configList.Add(sc);

            j2534Device.JapiMarshal.PassThruIoctl(channelId, IoctlId.GET_CONFIG, ref scl);

            return scl.configList[0].value;
        }

        public void SetConfig(ConfigParamId configParam, uint val)
        {
            SConfigList scl = new SConfigList();
            scl.numOfParams = 1;

            SConfig sc = new SConfig(configParam);
            sc.value = val;

            scl.configList.Add(sc);

            j2534Device.JapiMarshal.PassThruIoctl(channelId, IoctlId.SET_CONFIG, ref scl);
        }

        public byte[] FiveBaudInit(byte byteIn)
        {
            SByteArray sByteArrayIn = new SByteArray(1);
            sByteArrayIn.data[0] = byteIn;

            SByteArray sByteArrayOut = new SByteArray(64);

            j2534Device.JapiMarshal.PassThruIoctl(channelId, IoctlId.FIVE_BAUD_INIT, sByteArrayIn, ref sByteArrayOut);

            byte[] final = J2534Device.CreateByteArrayFromSByteArray(sByteArrayOut);

            return final;
        }

        public byte[] FastInit(byte[] bytesIn, bool responseRequired)
        {
            PassThruMsg msgIn = null;
            if (bytesIn != null)
            {
                msgIn = J2534Device.CreatePTMsgFromDataBytes(protocol, 0, bytesIn);
            }
            PassThruMsg msgOut = null;
            if (responseRequired)
            {
                msgOut = new PassThruMsg(64);
            }

            j2534Device.JapiMarshal.PassThruIoctl(channelId, IoctlId.FAST_INIT, msgIn, ref msgOut);

            if (responseRequired)
            {
                return msgOut.data;
            }
            else
            {
                return null;
            }
        }
    }
}
