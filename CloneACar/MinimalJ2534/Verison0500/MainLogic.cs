
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Minimal_J2534_0500
{
    internal class MainLogic
    {
        private J2534Device device1;
        private J2534Device device2;
        //private ObserverTextboxMain TBM;
        //private ObserverTextboxDetail TBD;

        /*public void InitTextboxLogMain(TextBox t)
        {
            TBM = new ObserverTextboxMain(t);
            LogMain.RegisterObserver(TBM);
        }

        public void InitTextboxLogDetail(TextBox t)
        {
            TBD = new ObserverTextboxDetail(t);
            LogDetail.RegisterObserver(TBD);
        }
        */

        public List<SDevice> GetPresentDevices(J2534Dll dll)
        {
            return J2534InstalledDlls.GetAvailableDevicesForProductType(dll);
        }
         
        public void PTOpen(J2534Dll dll, string name, int device)
        {
            LogMain.WriteEntry("Device" + device + " PassThruOpen");
            try
            {
                J2534Device jdevice = null;
                if (device == 1)
                {
                    device1 = new J2534Device(dll);
                    jdevice = device1;
                }
                else if (device == 2)
                {
                    device2 = new J2534Device(dll);
                    jdevice = device2;
                }
                
                jdevice.PTOpen(name);
                
                LogMain.WriteEntry("OK");
            }
            catch (J2534Exception ex)
            {
                LogMain.WriteEntry("Error: " + ex.SimpleDescription());
            }
            catch (Exception ex)
            {
                LogMain.WriteEntry("Error: " + ex.Message);
            }
        }

        public void PTClose(int device)
        {
            LogMain.WriteEntry("Device" + device + " PassThruClose");
            try
            {
                J2534Device jdevice = null;
                if (device == 1)
                {
                    jdevice = device1;
                }
                else if (device == 2)
                {
                    jdevice = device2;
                }

                jdevice.JapiMarshal.PassThruClose(jdevice.deviceId);
                
                LogMain.WriteEntry("OK");
            }
            catch (J2534Exception ex)
            {
                LogMain.WriteEntry("Error: " + ex.SimpleDescription());
            }
            catch (Exception ex)
            {
                LogMain.WriteEntry("Error: " + ex.Message);
            }
        }

        public void PTStartMsgFilterAllPass(int device)
        {
            LogMain.WriteEntry("Device" + device + " PassThruStartMsgFilter (AllPass)");

            try
            {
                J2534Device jdevice = null;
                if (device == 1)
                {
                    jdevice = device1;
                }
                else if (device == 2)
                {
                    jdevice = device2;
                }

                jdevice.channels[0].SetFilter(0, FilterDef.PASS_FILTER, "00 00 00 00", "00 00 00 00");
                
                LogMain.WriteEntry("OK");
            }
            catch (J2534Exception ex)
            {
                LogMain.WriteEntry("Error: " + ex.SimpleDescription());
            }
            catch (Exception ex)
            {
                LogMain.WriteEntry("Error: " + ex.Message);
            }
        }

        public void PTStopMsgFilterAllPass(int device)
        {
            LogMain.WriteEntry("Device" + device + " PassThruStopMsgFilter (AllPass)");
            try
            {
                J2534Device jdevice = null;
                if (device == 1)
                {
                    jdevice = device1;
                }
                else if (device == 2)
                {
                    jdevice = device2;
                }
                
                jdevice.channels[0].RemoveFilter(0);
                
                LogMain.WriteEntry("OK");
            }
            catch (J2534Exception ex)
            {
                LogMain.WriteEntry("Error: " + ex.SimpleDescription());
            }
            catch (Exception ex)
            {
                LogMain.WriteEntry("Error: " + ex.Message);
            }
        }

        public void PTConnectCAN(int device)
        {
            LogMain.WriteEntry("Device" + device + " PassThruConnect (CAN, 500k)");
            try
            {
                J2534Device jdevice = null;
                if (device == 1)
                {
                    jdevice = device1;
                }
                else if (device == 2)
                {
                    jdevice = device2;
                }
                jdevice.PTConnect(0, ProtocolId.CAN, 0, 500000);
                
                LogMain.WriteEntry("OK");
            }
            catch (J2534Exception ex)
            {
                LogMain.WriteEntry("Error: " + ex.SimpleDescription());
            }
            catch (Exception ex)
            {
                LogMain.WriteEntry("Error: " + ex.Message);
            }
        }

        public void PTDisconnectCAN(int device)
        {
            LogMain.WriteEntry("Device" + device + " PassThruDisconnect");
            try
            {
                J2534Device jdevice = null;
                if (device == 1)
                {
                    jdevice = device1;
                }
                else if (device == 2)
                {
                    jdevice = device2;
                }
                
                jdevice.PTDisconnect(0);
               
                LogMain.WriteEntry("OK");
            }
            catch (J2534Exception ex)
            {
                LogMain.WriteEntry("Error: " + ex.SimpleDescription());
            }
            catch (Exception ex)
            {
                LogMain.WriteEntry("Error: " + ex.Message);
            }
        }

        public void PTLogicalConnect(int device)
        {
            LogMain.WriteEntry("Device" + device + " PassThruLogicalConnect (15765)");
            try
            {
                Iso15765ChannelDescriptor cD = new Iso15765ChannelDescriptor();

                J2534Device jdevice = null;
                if (device == 1)
                {
                    jdevice = device1;
                    cD.localAddress = new byte[5] { 0, 0, 0x07, 0xE0, 0 };
                    cD.remoteAddress = new byte[5] { 0, 0, 0x07, 0xE8, 0 };
                }
                else if (device == 2)
                {
                    jdevice = device2;
                    cD.localAddress = new byte[5] { 0, 0, 0x07, 0xE8, 0 };
                    cD.remoteAddress = new byte[5] { 0, 0, 0x07, 0xE0, 0 };
                }

                jdevice.channels[0].PTLogicalConnect(0, cD);

                LogMain.WriteEntry("OK");
            }
            catch (J2534Exception ex)
            {
                LogMain.WriteEntry("Error: " + ex.SimpleDescription());
            }
            catch (Exception ex)
            {
                LogMain.WriteEntry("Error: " + ex.Message);
            }
        }

        public void PTLogicalDisconnect(int device)
        {
            LogMain.WriteEntry("Device" + device + " PassThruLogicalDisconnect (15765)");
            try
            {
                J2534Device jdevice = null;
                if (device == 1)
                {
                    jdevice = device1;
                }
                else if (device == 2)
                {
                    jdevice = device2;
                }

                jdevice.channels[0].PTLogicalDisconnect(0);

                LogMain.WriteEntry("OK");
            }
            catch (J2534Exception ex)
            {
                LogMain.WriteEntry("Error: " + ex.SimpleDescription());
            }
            catch (Exception ex)
            {
                LogMain.WriteEntry("Error: " + ex.Message);
            }
        }

        public void ReadVoltage(int device, int pin, bool allPins = false)
        {
            if (!allPins)
            {
                LogMain.WriteEntry("Device" + device + " PassThruIoctl - READ_PIN_VOLTAGE (pin " + pin + ")");
                try
                {
                    J2534Device jdevice = null;
                    if (device == 1)
                    {
                        jdevice = device1;
                    }
                    else if (device == 2)
                    {
                        jdevice = device2;
                    }
                    int val = jdevice.ReadVBatt(pin);
                    LogMain.WriteEntry("Voltage = " + val);
                    LogMain.WriteEntry("OK");
                }
                catch (J2534Exception ex)
                {
                    LogMain.WriteEntry("Error: " + ex.SimpleDescription());
                }
                catch (Exception ex)
                {
                    LogMain.WriteEntry("Error: " + ex.Message);
                }
            }
            if (allPins)
            {
                LogMain.WriteEntry("Pulling voltage from all pins on the device.");
                List<string> pinsVoltage = new List<string>();

                int counter = 1;
                while (counter <= 16)
                {
                    try
                    {
                        J2534Device jdevice = null;
                        if (device == 1)
                        {
                            jdevice = device1;
                        }
                        else if (device == 2)
                        {
                            jdevice = device2;
                        }
                        int val = jdevice.ReadVBatt(counter);

                        pinsVoltage.Add(val.ToString());
                        
                    }
                    catch (J2534Exception ex)
                    {
                        pinsVoltage.Add("GND");
                    }
                    catch (Exception ex)
                    {
                       pinsVoltage.Add("GND");
                    }
                    counter++;
                }

                int indexer = 1;
  
                foreach (string voltage in pinsVoltage)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("[J2534DLL] ::: ");
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write("|");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write("Pin ");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write(indexer);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(" :: ");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(voltage);
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine("|");
                    indexer++;
                }
            }
        }

        public void ReadProgVoltage(int device)
        {
            LogMain.WriteEntry("Device" + device + " PassThruIoctl - READ_PROG_VOLTAGE");
            try
            {
                J2534Device jdevice = null;
                if (device == 1)
                {
                    jdevice = device1;
                }
                else if (device == 2)
                {
                    jdevice = device2;
                }
                int val = jdevice.ReadVProg();
                LogMain.WriteEntry("ProgVoltage = " + val);
                LogMain.WriteEntry("OK");
            }
            catch (J2534Exception ex)
            {
                LogMain.WriteEntry("Error: " + ex.SimpleDescription());
            }
            catch (Exception ex)
            {
                LogMain.WriteEntry("Error: " + ex.Message);
            }
        }

        public void PTReadVersion(int device)
        {
            LogMain.WriteEntry("Device" + device + " PassThruReadVersion");
            try
            {
                J2534Device jdevice = null;
                if (device == 1)
                {
                    jdevice = device1;
                }
                else if (device == 2)
                {
                    jdevice = device2;
                }

                string firmwareVersion;
                string dllVersion;
                string apiVersion;

                jdevice.JapiMarshal.PassThruReadVersion(jdevice.deviceId, out firmwareVersion, out dllVersion, out apiVersion);

                LogMain.WriteEntry("Firmware Version = " + firmwareVersion);
                LogMain.WriteEntry("Dll Version = " + dllVersion);
                LogMain.WriteEntry("Api Version = " + apiVersion);
                LogMain.WriteEntry("OK");
            }
            catch (J2534Exception ex)
            {
                LogMain.WriteEntry("Error: " + ex.SimpleDescription());
            }
            catch (Exception ex)
            {
                LogMain.WriteEntry("Error: " + ex.Message);
            }
        }

        public void PTStartPeriodicMsg(int device)
        {
            LogMain.WriteEntry("Device" + device + " PassThruStartPeriodicMsg (1sec)");

            try
            {
                byte address = 0;
                J2534Device jdevice = null;
                if (device == 1)
                {
                    address = 0xE8;
                    jdevice = device1;
                }
                else if (device == 2)
                {
                    address = 0xE0;
                    jdevice = device2;
                }

                PassThruMsg msg = new PassThruMsg(8);
                msg.protocolId = ProtocolId.CAN;
                msg.txFlags = 0;
                msg.msgHandle = 0;
                msg.dataLength = 8;
                msg.data[0] = 0;
                msg.data[1] = 0;
                msg.data[2] = 0x7;
                msg.data[3] = address;
                msg.data[4] = 0;
                for (int i = 5; i < 8; i++)
                {
                    msg.data[i] = Convert.ToByte(5 * i + 1);
                }

                jdevice.channels[0].PTStartPeriodicMsg(0, msg, 1000);

                LogMain.WriteEntry("OK");
            }
            catch (J2534Exception ex)
            {
                LogMain.WriteEntry("Error: " + ex.SimpleDescription());
            }
            catch (Exception ex)
            {
                LogMain.WriteEntry("Error: " + ex.Message);
            }
        }

        public void PTStopPeriodicMsg(int device)
        {
            LogMain.WriteEntry("Device" + device + " PassThruStopPeriodicMsg");
            try
            {
                J2534Device jdevice = null;
                if (device == 1)
                {
                    jdevice = device1;
                }
                else if (device == 2)
                {
                    jdevice = device2;
                }

                jdevice.channels[0].PTStopPeriodicMsg(0);

                LogMain.WriteEntry("OK");
            }
            catch (J2534Exception ex)
            {
                LogMain.WriteEntry("Error: " + ex.SimpleDescription());
            }
            catch (Exception ex)
            {
                LogMain.WriteEntry("Error: " + ex.Message);
            }
        }

        public void PTReadMsgs(int device)
        {
            LogMain.WriteEntry("Device" + device + " PassThruReadMsgs (1 msg, 0 timeout)");
            try
            {
                J2534Device jdevice = null;
                if (device == 1)
                {
                    jdevice = device1;
                }
                else if (device == 2)
                {
                    jdevice = device2;
                }

                uint numMsgs = 1;
                PassThruMsg[] msg = jdevice.channels[0].PTReadMsgs(ref numMsgs, 0);

                LogMain.WriteEntry("Messages Read = " + numMsgs);

                if (numMsgs > 0)
                {
                    LogMain.WriteEntry("ProtocolId: " + msg[0].protocolId);
                    LogMain.WriteEntry("RxStatus: " + msg[0].rxStatus);
                    LogMain.WriteEntry("TxFlags: " + msg[0].txFlags);
                    LogMain.WriteEntry("Timestamp: " + msg[0].timestamp);
                    LogMain.WriteEntry("DataLength: " + msg[0].dataLength);
                    LogMain.WriteEntry("ExtraDataIndex: " + msg[0].extraDataIndex);
                    string data_print = "";
                    for (int i = 0; i < msg[0].data.Length; i++)
                    {
                        data_print += msg[0].data[i] + " ";
                    }
                    LogMain.WriteEntry("Data: " + data_print);
                }
                LogMain.WriteEntry("OK");
            }
            catch (J2534Exception ex)
            {
                LogMain.WriteEntry("Error: " + ex.SimpleDescription());
            }
            catch (Exception ex)
            {
                LogMain.WriteEntry("Error: " + ex.Message);
            }
        }

        public void PTQueueMsgs(int device)
        {
            LogMain.WriteEntry("Device" + device + " PassThruQueueMsgs (2 messages)");
            try
            {
                J2534Device jdevice = null;
                if (device == 1)
                {
                    jdevice = device1;
                }
                else if (device == 2)
                {
                    jdevice = device2;
                }

                PassThruMsg[] msg = new PassThruMsg[2];
                msg[0] = new PassThruMsg(8);
                msg[0].protocolId = ProtocolId.CAN;
                msg[0].txFlags = 0;
                msg[0].dataLength = 8;
                msg[0].data[0] = 0;
                msg[0].data[1] = 0;
                msg[0].data[2] = 0;
                msg[0].data[3] = 2;
                for (int i = 4; i < 8; i++)
                {
                    msg[0].data[i] = Convert.ToByte(3 * i + 1);
                }
                
                msg[1] = new PassThruMsg(8);
                msg[1].protocolId = ProtocolId.CAN;
                msg[1].txFlags = 0;
                msg[1].dataLength = 8;
                msg[1].data[0] = 0;
                msg[1].data[1] = 0;
                msg[1].data[2] = 0;
                msg[1].data[3] = 2;
                for (int i = 4; i < 8; i++)
                {
                    msg[1].data[i] = Convert.ToByte(4 * i + 3);
                }

                uint numMsgs = 2;
                jdevice.channels[0].PTQueueMsgs(msg);

                LogMain.WriteEntry("Messages Written = " + numMsgs);

                LogMain.WriteEntry("OK");
            }
            catch (J2534Exception ex)
            {
                LogMain.WriteEntry("Error: " + ex.SimpleDescription());
            }
            catch (Exception ex)
            {
                LogMain.WriteEntry("Error: " + ex.Message);
            }
        }

        public void GetConfig(int device)
        {
            LogMain.WriteEntry("Device" + device + " PassThruIoctl - GET_CONFIG");
            try
            {
                J2534Device jdevice = null;
                if (device == 1)
                {
                    jdevice = device1;
                }
                else if (device == 2)
                {
                    jdevice = device2;
                }

                SConfigList sc = new SConfigList();
                sc.configList.Add(new SConfig(ConfigParamId.DATA_RATE));
                sc.configList.Add(new SConfig(ConfigParamId.DT_PULLUP_VALUE));
                sc.numOfParams = (uint)sc.configList.Count;

                jdevice.JapiMarshal.PassThruIoctl(jdevice.channels[0].channelId, IoctlId.GET_CONFIG, ref sc);

                foreach (SConfig s in sc.configList)
                {
                    LogMain.WriteEntry(s.parameter + " = " + s.value);
                }

                LogMain.WriteEntry("OK");
            }
            catch (J2534Exception ex)
            {
                LogMain.WriteEntry("Error: " + ex.SimpleDescription());
            }
            catch (Exception ex)
            {
                LogMain.WriteEntry("Error: " + ex.Message);
            }
        }

        public void PTQueueMsgsLogical(int device)
        {
            LogMain.WriteEntry("Device" + device + " PassThruQueueMsgs Logical (2 messages)");
            try
            {
                byte address = 0;
                J2534Device jdevice = null;
                if (device == 1)
                {
                    address = 0xE8;
                    jdevice = device1;
                }
                else if (device == 2)
                {
                    address = 0xE0;
                    jdevice = device2;
                }

                /*
                PassThruMsg[] msg = new PassThruMsg[2];
                msg[0] = new PassThruMsg(12);
                msg[0].protocolId = ProtocolId.ISO15765_LOGICAL;
                msg[0].txFlags = 0;
                msg[0].msgHandle = 0;
                msg[0].dataLength = 16;
                msg[0].data[0] = 0;
                msg[0].data[1] = 0;
                msg[0].data[2] = 0x7;
                msg[0].data[3] = 0xE0;
                for (int i = 4; i < 12; i++)
                {
                    msg[0].data[i] = Convert.ToByte(5 * i + 1);
                }

                msg[1] = new PassThruMsg(12);
                msg[1].protocolId = ProtocolId.ISO15765_LOGICAL;
                msg[1].txFlags = (uint)TxFlags.ISO15765_FRAME_PAD;
                msg[0].msgHandle = 1;
                msg[1].dataLength = 18;
                msg[1].data[0] = 0;
                msg[1].data[1] = 0;
                msg[1].data[2] = 0x7;
                msg[1].data[3] = 0xE0;
                for (int i = 4; i < 12; i++)
                {
                    msg[1].data[i] = Convert.ToByte(6 * i + 3);
                }
                */
                PassThruMsg[] msg = new PassThruMsg[1];
                msg[0] = new PassThruMsg(12);
                msg[0].protocolId = ProtocolId.ISO15765_LOGICAL;
                msg[0].txFlags = 0;
                msg[0].msgHandle = 0;
                msg[0].dataLength = 12;
                msg[0].data[0] = 0;
                msg[0].data[1] = 0;
                msg[0].data[2] = 0x7;
                msg[0].data[3] = address;
                for (int i = 4; i < 12; i++)
                {
                    msg[0].data[i] = Convert.ToByte(5 * i + 1);
                }

                uint numMsgs = 1;
                jdevice.channels[0].logicalChannels[0].PTQueueMsgs(msg);

                LogMain.WriteEntry("Messages Written = " + numMsgs);

                LogMain.WriteEntry("OK");
            }
            catch (J2534Exception ex)
            {
                LogMain.WriteEntry("Error: " + ex.SimpleDescription());
            }
            catch (Exception ex)
            {
                LogMain.WriteEntry("Error: " + ex.Message);
            }
        }

        public void PTReadMsgsLogical(int device)
        {
            LogMain.WriteEntry("Device" + device + " PassThruReadMsgs Logical (1 msg, 0 timeout)");
            try
            {
                J2534Device jdevice = null;
                if (device == 1)
                {
                    jdevice = device1;
                }
                else if (device == 2)
                {
                    jdevice = device2;
                }

                uint numMsgs = 1;
                PassThruMsg[] msg = jdevice.channels[0].logicalChannels[0].PTReadMsgs(ref numMsgs, 0);

                LogMain.WriteEntry("Messages Read = " + numMsgs);

                if (numMsgs > 0)
                {
                    LogMain.WriteEntry("ProtocolId: " + msg[0].protocolId);
                    LogMain.WriteEntry("RxStatus: " + msg[0].rxStatus);
                    LogMain.WriteEntry("TxFlags: " + msg[0].txFlags);
                    LogMain.WriteEntry("Timestamp: " + msg[0].timestamp);
                    LogMain.WriteEntry("DataLength: " + msg[0].dataLength);
                    LogMain.WriteEntry("ExtraDataIndex: " + msg[0].extraDataIndex);
                    string data_print = "";
                    for (int i = 0; i < msg[0].data.Length; i++)
                    {
                        data_print += msg[0].data[i] + " ";
                    }
                    LogMain.WriteEntry("Data: " + data_print);
                }
                LogMain.WriteEntry("OK");
            }
            catch (J2534Exception ex)
            {
                LogMain.WriteEntry("Error: " + ex.SimpleDescription());
            }
            catch (Exception ex)
            {
                LogMain.WriteEntry("Error: " + ex.Message);
            }
        }

        public void PTStartMsgFilterAllPassLogical(int device)
        {
            LogMain.WriteEntry("Device" + device + " PassThruStartMsgFilter Logical (AllPass)");

            try
            {
                J2534Device jdevice = null;
                if (device == 1)
                {
                    jdevice = device1;
                }
                else if (device == 2)
                {
                    jdevice = device2;
                }

                jdevice.channels[0].logicalChannels[0].SetFilter(0, FilterDef.PASS_FILTER, "00 00 00 00", "00 00 00 00");

                LogMain.WriteEntry("OK");
            }
            catch (J2534Exception ex)
            {
                LogMain.WriteEntry("Error: " + ex.SimpleDescription());
            }
            catch (Exception ex)
            {
                LogMain.WriteEntry("Error: " + ex.Message);
            }
        }

        public void PTStopMsgFilterAllPassLogical(int device)
        {
            LogMain.WriteEntry("Device" + device + " PassThruStopMsgFilter Logical (AllPass)");
            try
            {
                J2534Device jdevice = null;
                if (device == 1)
                {
                    jdevice = device1;
                }
                else if (device == 2)
                {
                    jdevice = device2;
                }

                jdevice.channels[0].logicalChannels[0].RemoveFilter(0);

                LogMain.WriteEntry("OK");
            }
            catch (J2534Exception ex)
            {
                LogMain.WriteEntry("Error: " + ex.SimpleDescription());
            }
            catch (Exception ex)
            {
                LogMain.WriteEntry("Error: " + ex.Message);
            }
        }

        public void PTStartPeriodicMsgLogical(int device)
        {
            LogMain.WriteEntry("Device" + device + " PassThruStartPeriodicMsg Logical (1sec)");

            try
            {
                byte address = 0;
                J2534Device jdevice = null;
                if (device == 1)
                {
                    address = 0xE8;
                    jdevice = device1;
                }
                else if (device == 2)
                {
                    address = 0xE0;
                    jdevice = device2;
                }

                PassThruMsg msg = new PassThruMsg(12);
                msg.protocolId = ProtocolId.ISO15765_LOGICAL;
                msg.txFlags = 0;
                msg.msgHandle = 0;
                msg.dataLength = 12;
                msg.data[0] = 0;
                msg.data[1] = 0;
                msg.data[2] = 0x7;
                msg.data[3] = address;
                for (int i = 4; i < 12; i++)
                {
                    msg.data[i] = Convert.ToByte(5 * i + 1);
                }

                jdevice.channels[0].logicalChannels[0].PTStartPeriodicMsg(0, msg, 1000);

                LogMain.WriteEntry("OK");
            }
            catch (J2534Exception ex)
            {
                LogMain.WriteEntry("Error: " + ex.SimpleDescription());
            }
            catch (Exception ex)
            {
                LogMain.WriteEntry("Error: " + ex.Message);
            }
        }

        public void PTStopPeriodicMsgLogical(int device)
        {
            LogMain.WriteEntry("Device" + device + " PassThruStopPeriodicMsg Logical");
            try
            {
                J2534Device jdevice = null;
                if (device == 1)
                {
                    jdevice = device1;
                }
                else if (device == 2)
                {
                    jdevice = device2;
                }

                jdevice.channels[0].logicalChannels[0].PTStopPeriodicMsg(0);

                LogMain.WriteEntry("OK");
            }
            catch (J2534Exception ex)
            {
                LogMain.WriteEntry("Error: " + ex.SimpleDescription());
            }
            catch (Exception ex)
            {
                LogMain.WriteEntry("Error: " + ex.Message);
            }
        }

        public void PTSelect(int device)
        {
            LogMain.WriteEntry("Device" + device + " PassThruSelect");

            try
            {
                J2534Device jdevice = null;
                if (device == 1)
                {
                    jdevice = device1;
                }
                else if (device == 2)
                {
                    jdevice = device2;
                }

                SChannelSet scs = jdevice.PTSelect();

                LogMain.WriteEntry("Channel Count: " + scs.channelCount);
                LogMain.WriteEntry("Channel List: " + String.Join(",", scs.channelList.ToArray()));

                LogMain.WriteEntry("OK");
            }
            catch (J2534Exception ex)
            {
                LogMain.WriteEntry("Error: " + ex.SimpleDescription());
            }
            catch (Exception ex)
            {
                LogMain.WriteEntry("Error: " + ex.Message);
            }
        }

        public void PTSelectLogical(int device)
        {
            LogMain.WriteEntry("Device" + device + " PassThruSelect Logical");

            try
            {
                J2534Device jdevice = null;
                if (device == 1)
                {
                    jdevice = device1;
                }
                else if (device == 2)
                {
                    jdevice = device2;
                }

                SChannelSet scs = jdevice.channels[0].logicalChannels[0].PTSelect();

                LogMain.WriteEntry("Channel Count: " + scs.channelCount);
                LogMain.WriteEntry("Channel List: " + String.Join(",", scs.channelList.ToArray()));

                LogMain.WriteEntry("OK");
            }
            catch (J2534Exception ex)
            {
                LogMain.WriteEntry("Error: " + ex.SimpleDescription());
            }
            catch (Exception ex)
            {
                LogMain.WriteEntry("Error: " + ex.Message);
            }
        }

        public void CloseAllDevices()
        {
            if ((device1 != null) && device1.isOpen)
            {
                device1.PTClose();
            }

            if ((device2 != null) && device2.isOpen)
            {
                device2.PTClose();
            }
        }
    }
}