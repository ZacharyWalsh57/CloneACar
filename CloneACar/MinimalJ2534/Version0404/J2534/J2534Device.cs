
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;


namespace Minimal_J2534_0404
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
        public bool isConnected = false;

        public J2534Channel[] channels;
        public const uint maxChannels = 2;

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

            channels = new J2534Channel[maxChannels];
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
                serialNumber = - 1;
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
            var SplitString = msgString.Split(' ');
            byte[] MsgBytes = new byte[SplitString.Length];
            for (int index = 0; index < MsgBytes.Length; index++)
            {
                MsgBytes[index] = Byte.Parse(SplitString[index], NumberStyles.HexNumber);
            }

            PassThruMsg OutputMsg = new PassThruMsg((uint)MsgBytes.Length);
            OutputMsg.protocolId = protocol;
            OutputMsg.txFlags = flags;
            OutputMsg.data = MsgBytes;
            OutputMsg.dataLength = (uint)MsgBytes.Length;

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
            for (int i = 0; i < sb.numOfBytes; i++)
            {
                final[i] = sb.data[i];
            }
            return final;
        }

        public string GetRandomNumber()
        {
            return JapiMarshal.GetRandom();
        }

        public bool GetActivationState()
        {
            int res = Japi.DTEraseDevice((uint)IoctlId.DT_ISACTIVATED_STATUS);

            if (res == 0) { return true; }
            else if (res == 7) { return false; }
            else { throw new AppException("unable to read activation state"); }
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

        public void PTConnect(int index, ProtocolId protocol, uint flags, uint baud)
        {
            uint channelId;
            JapiMarshal.PassThruConnect(deviceId, protocol, flags, baud, out channelId);
            channels[index] = new J2534Channel(this, channelId, protocol, flags, baud);
        }

        public void PTDisconnect(int index)
        {
            JapiMarshal.PassThruDisconnect(channels[index].channelId);
            channels[index] = null;
        }

        /// <summary>
        /// wrapper for PassThruSetProgrammingVoltage
        /// </summary>
        public void SetPinVoltage(int pinNumber, int milliVolts)
        {
            JapiMarshal.PassThruSetProgrammingVoltage(deviceId, (uint)pinNumber, (uint)milliVolts);
        }

        /// <summary>
        /// wrapper for PassThruSetProgrammingVoltage, remove voltage generator
        /// </summary>
        public void RemovePinVoltage(int pinNumber)
        {
            JapiMarshal.PassThruSetProgrammingVoltage(deviceId, (uint)pinNumber, (uint)VoltageValue.VOLTAGE_OFF);
        }

        /// <summary>
        /// wrapper for PassThruIoctl with READ_VBATT
        /// </summary>
        public int ReadVPin(int pin)
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
        /// Reads voltage from pin 16
        /// </summary>
        /// <returns>int value of the voltage used from the DLC.</returns>
        public int ReadVBatt()
        {
            uint voltage;
            JapiMarshal.PassThruIoctl(deviceId, IoctlId.READ_PIN_VOLTAGE, out voltage);

            return (int)voltage;
        }

        /// <summary>
        /// wrapper for PassThruIoctl with READ_PROG_VOLTAGE
        /// </summary>
        public int ReadVProg()
        {
            uint voltage;

            JapiMarshal.PassThruIoctl(deviceId, IoctlId.READ_PROG_VOLTAGE, out voltage);

            return (int)voltage;
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

        public void PTReadVersion()
        {
            JapiMarshal.PassThruReadVersion(deviceId, out firmwareVersion, out dllVersion, out apiVersion);
        }
    }
}