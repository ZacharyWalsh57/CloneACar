using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloneACar.LoggingHelpers;

// Using call for the Minimal 0404 DLL
using Minimal_J2534_0404;

// Globals
using static CloneACar.GlobalObjects;
using static CloneACar.LogicalHelpers.DataByteHelpers;
using CloneACar.Models;

namespace CloneACar.J2534Consumer
{
    public struct CloneMethodArgs
    {
        public PassThruMessageSet MessageSet;
        public uint ChannelFlags;
        public uint BaudRate;
        public string ProtocolString;
    }

    public class ExecuteModuleClone
    {       
        /// <summary>
        /// This is the J2534 Device opened from our device discovery setup.
        /// Since it's kinda hard to pass this thing around, use this to snag it from globals.
        /// </summary>
        private J2534Device Device
        {
            get { return HardwareConfig.SelectedHW.GetDeviceValues<J2534Device>(); }
            set { HardwareConfig.SelectedHW.SetDeviceValues(Device); }
        }

        private Logger CommsLogger;

        public ProtocolId Protocol;
        public string ProtocolString;
        public byte[] SendAddress;
        public byte[] ReadAddress;

        public ExecuteModuleClone(ProtocolId Proc, string ProcString, string AddressToSend)
        {
            Protocol = Proc;
            ProtocolString = ProcString;

            CommsLogger = new Logger(true, ProtocolString, "COMMS_LOGGING");
            CommsLogger.WriteMessageLog($"INIT OF COMMS LOGGER WAS OK! FILE AND DIR STRUCTURE SEEMS RIGHT");

            SendAddress = ConvertDataToByte(AddressToSend);
            ReadAddress = new byte[2] { SendAddress[0], (byte)(SendAddress[1] + 8) };

            CommsLogger.WriteMessageLog($"PROTOCOL SET --> {ProtocolString}");
            CommsLogger.WriteMessageLog($"SEND ADDRESS --> {ConvertDataToString(SendAddress)}");
            CommsLogger.WriteMessageLog($"READ ADDRESS --> {ConvertDataToString(ReadAddress)}");
        }
        public ExecuteModuleClone(ProtocolId Proc, string ProcString, byte[] AddressToSend)
        {
            Protocol = Proc;
            ProtocolString = ProcString;

            CommsLogger = new Logger(true, ProtocolString, "COMMS_LOGGING");
            CommsLogger.WriteMessageLog($"INIT OF COMMS LOGGER WAS OK! FILE AND DIR STRUCTURE SEEMS RIGHT");

            SendAddress = AddressToSend;
            ReadAddress = new byte[2] { SendAddress[0], (byte)(SendAddress[1] + 8) };

            CommsLogger.WriteMessageLog($"PROTOCOL SET --> {ProtocolString}");
            CommsLogger.WriteMessageLog($"SEND ADDRESS --> {ConvertDataToString(SendAddress)}");
            CommsLogger.WriteMessageLog($"READ ADDRESS --> {ConvertDataToString(ReadAddress)}");
        }


        /// <summary>
        /// Clone procedure. Checks into a supplied message set and runs each message.
        /// </summary>
        /// <returns>Bool saying if the requested module address comms ok or not.</returns>
        public bool CloneModuleSet(CloneMethodArgs ParamObject, out ModuleCommunicationResults ModuleCommResults)
        {
            // Open Device if needed and setup basic flow ctl.
            WrappedCommands.OpenDevice(Protocol, ParamObject.ChannelFlags, ParamObject.BaudRate);
            WrappedCommands.ClearFilters(Device.channels[0].channelId);
            WrappedCommands.ClearTXAndRX(Device.channels[0].channelId);
            AppLogger.WriteLog("DEVICE OPENED AND 11 BIT FLOW CONTROL FILTERS SETUP OK");

            // Setup new filter objects.
            SetupFilters(
                Device.channels[0].channelId,
                Protocol, ParamObject.ChannelFlags, 
                ConvertDataToString(SendAddress)
            );

            // Send our 7DF messages here. 
            PassThruMessageSet ModuleMessages = ParamObject.MessageSet;
            AppLogger.WriteLog($"SENDING MESSAGES AT {SendAddress} NOW");

            // Run Cloner and check the results of it.
            ModuleCommResults = SendMessageSet(ModuleMessages.Messages, out bool FoundComms);

            // Log result of query.
            if (FoundComms) { AppLogger.WriteLog($"FOUND COMMS AT ADDRESS SET {SendAddress} OK!"); }
            else { AppLogger.WriteLog($"FAILED TO FIND COMMS AT ADDRESS SET {SendAddress}!"); }

            // Clear filters and buffers.
            WrappedCommands.ClearFilters(Device.channels[0].channelId);
            WrappedCommands.ClearTXAndRX(Device.channels[0].channelId);
            Device.PTDisconnect(0);
            Device.PTClose();

            // Return if we got good comms or not.
            AppLogger.WriteLog($"RETURNING: {FoundComms} --> CHANNEL AND CLEARED BUFFS/FILTERS OK");
            return FoundComms;
        }


        /// <summary>
        /// Sends and receives a list of message pair objects.
        /// </summary>
        /// <param name="MessageSet">MEssage set object. All Messages to send and other info..</param>
        /// <returns>A paired set of sent and read messages.</returns>
        private ModuleCommunicationResults SendMessageSet(PassThruMessageSet MessageSet, out bool FoundComms)
        {
            return SendMessageSet(MessageSet.Messages, out FoundComms);
        }
        /// <summary>
        /// Sends and receives a list of message pair objects.
        /// </summary>
        /// <param name="AddressBytes">Bytes of the address to send from.</param>
        /// <param name="MessageList">List of PTMessages to send</param>
        /// <returns>A paired set of sent and read messages.</returns>
        private ModuleCommunicationResults SendMessageSet(List<PassThruMsg> MessageList, out bool FoundComms)
        {
            // Channel ID for open channel on device.
            uint ChannelID = Device.channels[0].channelId;

            // Make response object and send out msgs then get responses.
            var MessageSendAndResponse = new ModuleCommunicationResults();
            for (int Index = 0; Index < MessageList.Count; Index++)
            {
                string MsgString = ConvertDataToString(MessageList[Index].data);
                var SendMsgs = new PassThruMsg[1] { MessageList[Index] };
                var ReadMsgs = new PassThruMsg[10];

                // Read messages in now.
                ReadMsgs = WrappedCommands.WriteAndRead(ChannelID, SendMsgs.ToList(), 8, CommsLogger);
                if (ReadMsgs != null)
                {
                    // Append it to the list of paired log comm lines.
                    if (!MessageSendAndResponse.AddMessageTuple(SendMsgs, ReadMsgs))
                    {
                        AppLogger.WriteMessageLog("FAILED TO ADD THIS ITEM SET TO THE TUPLE LIST!");
                        AppLogger.WriteMessageLog("THIS CAN BE DUE TO NULL MESSAGE COUNTS OR OTHER ISSUES");
                    }
                }
            }

            // Return the message values here. Use the bool to determine if something needs to be done with output.
            FoundComms = MessageSendAndResponse.MessagePairs.Count > 0;
            return MessageSendAndResponse;
        }
        /// <summary>
        /// Auto establish new filters as messages come in. This is used pre clone running.
        /// When a message comes in, another filter program may be setup. 
        /// </summary>
        /// <param name="ChannelID">Channel to work on.</param>
        /// <param name="Protocol">Protocol of the channel</param>
        /// <param name="ChannelOrMsgFlags">Message flags.</param>
        /// <param name="SendAddress">Address messages are being sent from.</param>
        private void SetupFilters(uint ChannelID, ProtocolId Protocol, uint ChannelOrMsgFlags, string SendAddress)
        {
            switch (Protocol)
            {
                case ProtocolId.ISO15765:
                    if (SendAddress == "0x07 0xDF") { WrappedCommands.Setup11BitFlowCtl(ChannelID); }
                    else if (ChannelOrMsgFlags == 0x40)
                    {
                        byte[] LocalAddress = ConvertDataToByte(SendAddress);
                        byte[] RemoteAddress = new byte[2] { LocalAddress[0], (byte)(LocalAddress[1] + 8) };

                        string MaskMsg = "FF FF FF FF";
                        string PattMsg = "00 00 " + ConvertDataToString(RemoteAddress);
                        string FlowCtl = "00 00 " + ConvertDataToString(LocalAddress);

                        // AppLogger.WriteLog($"SETTING FILTER: {PattMsg} --> {FlowCtl}");
                        WrappedCommands.SetupFlowCtlFilter(ChannelID, Protocol, 0x40, MaskMsg, PattMsg, FlowCtl);
                    }
                    break;

                case ProtocolId.CAN:
                    if (SendAddress == "0x18 0xDA")
                    {
                        string MaskMsg = "FF FF 00 00";
                        string Pattern = "18 DA 00 00";

                        // AppLogger.WriteLog($"SETTING FILTER: {MaskMsg} --> {Pattern}");
                        WrappedCommands.SetupPassOrBlockFilter(
                            ChannelID, Protocol, FilterDef.PASS_FILTER,
                            0x100, MaskMsg, Pattern
                        );
                    }
                    break;
            }

            WrappedCommands.ClearTXAndRX(ChannelID);
            return;
        }
    }
}
