
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;


namespace Minimal_J2534_0500
{
    /// <summary>
    /// parent of all J2534 devices
    /// </summary>
    internal class J2534Device
    {
        public J2534Dll Jdll;
        public J2534Api Japi;                   // wrapper for PassThru functions
        public J2534ApiMarshal JapiMarshal;     // wrapper for PassThru functions with all marshalling done
        public uint deviceId;
        public string name;

        public bool isOpen = false;

        public J2534ChannelPhysical[] channels;
        public const uint maxChannels = 6;

        public string firmwareVersion;
        public string dllVersion;
        public string apiVersion;

        public ProtocolId ConnectProtocol { get; set; }   // used by connect strategy
        public uint ConnectFlags { get; set; }                  // used by connect strategy
        public uint ConnectBaud { get; set; }                   // used by connect strategy

        /// <summary>
        /// constructor
        /// </summary>
        public J2534Device(J2534Dll jdll)
        {
            Jdll = jdll;
            Japi = new J2534Api(Jdll.FunctionLibrary);
            JapiMarshal = new J2534ApiMarshal(Japi);

            channels = new J2534ChannelPhysical[maxChannels];
        }

        /// <summary>
        /// wrapper for PassThruIoctl with GET_DEVICE_INFO, param=SERIAL_NUMBER
        /// </summary>
        public int ReadSerial()
        {
            int serialNumber;

            SPARAM_LIST paramList = new SPARAM_LIST();
            SPARAM param = new SPARAM();

            param.Parameter = (uint)SParamParameters.SERIAL_NUMBER;
            param.Value = 0;
            param.Supported = 0;

            IntPtr ptrParam = Marshal.AllocHGlobal(Marshal.SizeOf(param));
            Marshal.StructureToPtr(param, ptrParam, true);

            paramList.NumOfParameters = 1;
            paramList.SParamPtr = ptrParam;

            IntPtr ptrParamList = Marshal.AllocHGlobal(Marshal.SizeOf(paramList));
            Marshal.StructureToPtr(paramList, ptrParamList, true);

            Japi.PassThruIoctl(deviceId, IoctlId.GET_DEVICE_INFO, IntPtr.Zero, ptrParamList);

            paramList = (SPARAM_LIST)Marshal.PtrToStructure(ptrParamList, typeof(SPARAM_LIST));
            param = (SPARAM)Marshal.PtrToStructure(ptrParam, typeof(SPARAM));

            if (param.Supported == 0)
            {
                serialNumber = -1;
            }
            else
            {
                serialNumber = (int)param.Value;
            }

            Marshal.DestroyStructure(ptrParam, param.GetType());
            Marshal.FreeHGlobal(ptrParam);
            Marshal.DestroyStructure(ptrParamList, paramList.GetType());
            Marshal.FreeHGlobal(ptrParamList);

            return serialNumber;
        }

        public static PassThruMsg CreatePTMsgFromString(ProtocolId protocol, uint flags, string msgString)
        {
            int chunkSize = 2;
            int stringLength = msgString.Length;
            int byteIndex = 0;
            byte[] MsgBytes = new byte[msgString.Length / 2];

            for (int i = 0; i < stringLength; i += chunkSize)
            {
                if (i + chunkSize > stringLength) { chunkSize = stringLength - i; }
                MsgBytes[byteIndex] = Encoding.ASCII.GetBytes(msgString.Substring(i, chunkSize))[0];
                byteIndex++;
            }

            PassThruMsg OutputMsg = new PassThruMsg((uint)msgString.Length / 2);
            OutputMsg.protocolId = protocol;
            OutputMsg.txFlags = flags;
            OutputMsg.data = MsgBytes;

            return OutputMsg;

            /*SoapHexBinary shb = SoapHexBinary.Parse(msgString);
            uint dataSize = (uint)shb.Value.Length;
            PassThruMsg msg = new PassThruMsg(dataSize);
            msg.protocolId = protocol;
            msg.txFlags = flags;
            msg.dataLength = dataSize;
            for (int i = 0; i < shb.Value.Length; i++)
                msg.data[i] = shb.Value[i];

            return msg;*/
        }

        public static PassThruMsg CreatePTMsgFromDataBytes(ProtocolId protocol, uint flags, byte[] dataBytes)
        {
            PassThruMsg msg = new PassThruMsg((uint)dataBytes.Length);
            msg.protocolId = protocol;
            msg.txFlags = flags;
            msg.dataLength = (uint)dataBytes.Length;
            for (int i = 0; i < (uint)dataBytes.Length; i++)
                msg.data[i] = dataBytes[i];

            return msg;
        }

        public static byte[] CreateByteArrayFromSByteArray(SByteArray sb)
        {
            byte[] final = new byte[sb.numOfBytes];
            for (int i=0; i < sb.numOfBytes; i++)
            {
                final[i] = sb.data[i];
            }
            return final;
        }

        public void PTOpen(string name)
        {
            this.name = name;
            JapiMarshal.PassThruOpen(name, out deviceId);
            isOpen = true;
        }

        public void PTClose()
        {
            JapiMarshal.PassThruClose(deviceId);
            isOpen = false;
        }

        public void PTConnect(int index, ProtocolId protocol, uint flags, uint baud, int[] pins = null)
        {
            uint channelId;
            JapiMarshal.PassThruConnect(deviceId, protocol, flags, baud, out channelId, pins);
            channels[index] = new J2534ChannelPhysical(this, channelId, protocol, flags, baud);
        }

        public void PTDisconnect(int index)
        {
            JapiMarshal.PassThruDisconnect(channels[index].channelId);
            channels[index] = null;
        }

        /// <summary>
        /// wrapper for PassThruIoctl with READ_VBATT
        /// </summary>
        public int ReadVBatt(int pin)
        {
            uint voltage;
            ResourceStruct rs = new ResourceStruct();
            rs.connector = Connector.J1962_CONNECTOR;
            rs.numOfResources = 1;
            rs.resourceList.Add(pin);

            JapiMarshal.PassThruIoctl(deviceId, IoctlId.READ_PIN_VOLTAGE, rs, out voltage);

            return (int)voltage;
        }

        /// <summary>
        /// wrapper for PassThruIoctl with READ_PROG_VOLTAGE
        /// </summary>
        public int ReadVProg()
        {
            IntPtr pOutput = Marshal.AllocHGlobal(4);
            Japi.PassThruIoctl(deviceId, IoctlId.READ_PROG_VOLTAGE, IntPtr.Zero, pOutput);
            Int32 voltage = Marshal.ReadInt32(pOutput);

            Marshal.FreeHGlobal(pOutput);

            return voltage;
        }
        /// <summary>
        /// wrapper for PassThruSetProgrammingVoltage
        /// </summary>
        public void SetPinVoltage(int pinNumber, int milliVolts)
        {
            JapiMarshal.PassThruSetProgrammingVoltage(deviceId, (uint)pinNumber, (uint)milliVolts);
        }

        /// <summary>
        /// wrapper for PassThruSetProgrammingVoltage, short to ground
        /// </summary>
        public void ShortPinToGround(int pinNumber)
        {
            JapiMarshal.PassThruSetProgrammingVoltage(deviceId, (uint)pinNumber, (uint)VoltageValue.SHORT_TO_GROUND);
        }

        /// <summary>
        /// wrapper for PassThruSetProgrammingVoltage, remove voltage generator
        /// </summary>
        public void RemovePinVoltage(int pinNumber)
        {
            JapiMarshal.PassThruSetProgrammingVoltage(deviceId, (uint)pinNumber, (uint)VoltageValue.PIN_OFF);
        }

        // set resistor between VBatt and pin 8
        // allowed values, -1 = OFF, 0 = 0 ohm, 510=510 ohm, 4700=4.7Kohm
        public void SetPin8ActivationLine(int resistor)
        {
            JapiMarshal.PassThruIoctl(deviceId, IoctlId.ETH_ACTIVATION_PULLUP, resistor);
        }

        public J2534Channel GetChannelFromId(uint channelId)
        {
            foreach (J2534Channel c in channels)
            {
                if (c.channelId == channelId)
                {
                    return c;
                }
            }
            return null;
        }

        // does a PTSelect on all channels in the device (including logical channels)
        public SChannelSet PTSelect()
        {
            SChannelSet sc = new SChannelSet();
            sc.channelThreshold = 0;

            foreach (J2534ChannelPhysical ch in channels)
            {
                sc.channelList.Add((int)ch.channelId);
                foreach (J2534ChannelLogical chl in ch.logicalChannels)
                {
                    sc.channelList.Add((int)chl.channelId);
                }
            }

            sc.channelCount = (uint)sc.channelList.Count;

            JapiMarshal.PassThruSelect(ref sc, 0);

            return sc;
        }

        public void PTReadVersion()
        {
            JapiMarshal.PassThruReadVersion(deviceId, out firmwareVersion, out dllVersion, out apiVersion);
        }

        public string ReadCableSerial()
        {
            SByteArray cableSerialSb = new SByteArray(17);
            JapiMarshal.PassThruIoctl(deviceId, IoctlId.DT_READ_CABLE_SERIAL_NUMBER, ref cableSerialSb);

            string cableSerial = System.Text.Encoding.ASCII.GetString(cableSerialSb.data, 0, 16);

            return cableSerial;
        }

        public string ReadCableType()
        {
            return ReadCableSerial().Substring(0, 2);
        }

    }
}