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

namespace CloneACar.J2534Consumer
{
    public class WrappedCommands
    {
        /// <summary>
        /// This is the J2534 Device opened from our device discovery setup.
        /// Since it's kinda hard to pass this thing around, use this to snag it from globals.
        /// </summary>
        private static J2534Device Device
        {
            get { return HardwareConfig.SelectedHW.GetDeviceValues<J2534Device>(); }
            set { HardwareConfig.SelectedHW.SetDeviceValues(Device); }
        }


        /// <summary>
        /// Open a new channel on the set device object.
        /// </summary>
        /// <param name="Protocol">Protocol to use </param>
        /// <param name="Flags">Flags for the channel</param>
        /// <param name="BaudRate">BAUD to connect at.</param>
        public static void OpenDevice(ProtocolId Protocol, uint Flags, uint BaudRate)
        {
            if (!Device.isOpen) { Device.PTOpen(Device.name); }
            if (Device.channels[0] != null)
                if (Device.channels[0].protocol != Protocol)
                    Device.PTDisconnect(0);
            if (Device.channels[0] == null)
                Device.PTConnect(0, Protocol, Flags, BaudRate);
        }


        /// <summary>
        /// Writes and reads back messages on a specified PTChannel.
        /// </summary>
        /// <param name="ChannelID">Channel ID</param>
        /// <param name="Protocol">ProtocolID</param>
        /// <param name="MessageToSend">Messages to send as string values.</param>
        /// <param name="MessageFlags">Flags for messages.</param>
        /// <param name="MessagesToRead">Message Count to read back</param>
        /// <param name="MsgLogger">Logger object to pass in for message logging.</param>
        /// <returns>A PTMessage Array that contains response values.</returns>
        public static PassThruMsg[] WriteAndRead(uint ChannelID, ProtocolId Protocol, string MessageToSend, uint MessageFlags, uint MessagesToRead, Logger MsgLogger = null)
        {
            return  WriteAndRead(ChannelID, Protocol, new List<string> { MessageToSend }, MessageFlags, MessagesToRead, MsgLogger);
        }
        /// <summary>
        /// Writes and reads back messages on a specified PTChannel.
        /// </summary>
        /// <param name="ChannelID">Channel ID</param>
        /// <param name="Protocol">ProtocolID</param>
        /// <param name="MessagesToSend">Messages to send as string values.</param>
        /// <param name="MessageFlags">Flags for messages.</param>
        /// <param name="MessagesToRead">Message Count to read back</param>
        /// <param name="MsgLogger">Logger object to pass in for message logging.</param>
        /// <returns>A PTMessage Array that contains response values.</returns>
        public static PassThruMsg[] WriteAndRead(uint ChannelID, ProtocolId Protocol, List<string> MessagesToSend, uint MessageFlags, uint MessagesToRead, Logger MsgLogger = null)
        {
            WriteMessage(ChannelID, Protocol, MessagesToSend, MessageFlags, MsgLogger);
            return ReadMessages(ChannelID, MessagesToRead, MsgLogger);
        }


        /// <summary>
        /// Writes a PTMessage to the selected JBox interface.
        /// </summary>
        /// <param name="ChannelID">Channel ID.</param>
        /// <param name="Protocol">Protocol ID in use.</param>
        /// <param name="MessageToSend">Messages to write out.</param>
        /// <param name="MsgFlags">Message Flags.</param>
        /// <param name="MessageLogger">Logger to write messages to file.</param>
        public static void WriteMessage(uint ChannelID, ProtocolId Protocol, string MessageToSend, uint MsgFlags, Logger MessageLogger = null)
        {
            var MessageItem = J2534Device.CreatePTMsgFromString(Protocol, MsgFlags, MessageToSend);
            uint MsgCount = 1;

            AppLogger.WriteLog($"WRITING MESSAGE TO CHANNEL {ChannelID} --> {MessageToSend}");
            MessageLogger?.WriteMessageLog(MessageToSend, MessageLogTypes.MessageTypes.PT_WRITE);

            try { Device.JapiMarshal.PassThruWriteMsgs(ChannelID, MessageItem, ref MsgCount, 250); }
            catch (J2534Exception ex)
            {
                AppLogger.WriteLog($"ERROR WHILE SENDING MESSAGE --> {ex.Message}", TextLogTypes.LogItemType.ERROR);
                AppLogger.WriteErrorLog(ex);
            }
        }
        /// <summary>
        /// Writes a List of PTMessages to the selected JBox interface.
        /// </summary>
        /// <param name="ChannelID">Channel ID.</param>
        /// <param name="Protocol">Protocol ID in use.</param>
        /// <param name="MessagesToSend">Messages to write out.</param>
        /// <param name="MsgFlags">Message Flags.</param>
        /// <param name="MessageLogger">Logger to write messages to file.</param>
        public static void WriteMessage(uint ChannelID, ProtocolId Protocol, List<string> MessagesToSend, uint MsgFlags, Logger MessageLogger = null)
        {
            uint MsgCount = (uint)MessagesToSend.Count;
            var Messages = new PassThruMsg[MessagesToSend.Count];

            for (int Count = 0; Count < MessagesToSend.Count; Count++)
            {
                string NextString = MessagesToSend[Count];
                var NextMsg = J2534Device.CreatePTMsgFromString(Protocol, MsgFlags, NextString);
                AppLogger.WriteLog($"WRITING MESSAGE [{Count}] TO CHANNEL {ChannelID} --> {NextString}");

                Messages[Count] = NextMsg;
            }
            
            ClearTXAndRX(ChannelID);

            MessageLogger?.WriteMessageLog(Messages, MessageLogTypes.MessageTypes.PT_WRITE);
            try { Device.JapiMarshal.PassThruWriteMsgs(ChannelID, Messages, ref MsgCount, 250); }
            catch (J2534Exception ex)
            {
                AppLogger.WriteLog($"ERROR WHILE SENDING MESSAGE --> {ex.Message}", TextLogTypes.LogItemType.ERROR);
                AppLogger.WriteErrorLog(ex);
            }
        }
        /// <summary>
        /// Reads a set number of PTMessages back from the JDevice.
        /// </summary>
        /// <param name="ChannelID">Channel ID in use.</param>
        /// <param name="MessagesToRead">Number of messages to pull back</param>
        /// <param name="MessageLogger">Logger to write messages out to file.</param>
        /// <returns></returns>
        public static PassThruMsg[] ReadMessages(uint ChannelID, uint MessagesToRead, Logger MessageLogger = null)
        {
            ClearTXAndRX(ChannelID);

            var MessagesRead = new PassThruMsg[MessagesToRead];
            try { Device.JapiMarshal.PassThruReadMsgs(ChannelID, out MessagesRead, ref MessagesToRead, 500); }
            catch (J2534Exception ex)
            {
                AppLogger.WriteLog($"ERROR WHILE READING MESSAGES --> {ex.Message}", TextLogTypes.LogItemType.ERROR);
                AppLogger.WriteErrorLog(ex);

                ClearTXAndRX(ChannelID);
                return MessagesRead;
            }

            AppLogger.WriteLog($"READ BACK MESSAGES OK! FOUND {MessagesRead.Length} MESSAGES IN TOTAL. FEEDING THEM OUT NOW");
            MessageLogger?.WriteMessageLog(MessagesRead, MessageLogTypes.MessageTypes.PT_READS);

            for (int Count = 0; Count < MessagesRead.Length; Count++)
            {
                string ByteConverted = ConvertDataToString(MessagesRead[Count].data);
                string WriteThis = "MESSAGE [" + Count + "] --> " + ByteConverted;
                AppLogger.WriteLog(WriteThis);
            }
            
            return MessagesRead;
        }


        /// <summary>
        /// Setup a flow Control filter. ONLY VALID FOR 11 BIT CAN
        /// </summary>
        /// <param name="ChannelID">Channel In use.</param>
        /// <param name="Protocol">ProtocolID - MUST BE 15765 11 BIT</param>
        /// <param name="Flags">Flags for filter (Usually 0x40)</param>
        /// <param name="MaskMsg">Mask Message (Usually FF FF FF FF)</param>
        /// <param name="PatternMsg">Pattern Message (Receive Address 07 E8)</param>
        /// <param name="FlowCtlMsg">Flow Ctl Message (Send Address 07 E0)</param>
        public static void SetupFlowCtlFilter(uint ChannelID, ProtocolId Protocol, uint Flags, string MaskMsg, string PatternMsg, string FlowCtlMsg)
        {
            var PTMaskMsg = J2534Device.CreatePTMsgFromString(Protocol, Flags, MaskMsg);
            var PTPatternMsg = J2534Device.CreatePTMsgFromString(Protocol, Flags, PatternMsg);
            var PTFlowCtlMsg = J2534Device.CreatePTMsgFromString(Protocol, Flags, FlowCtlMsg);

            try
            {
                Device.JapiMarshal.PassThruStartMsgFilter(
                    ChannelID, FilterDef.FLOW_CONTROL_FILTER,
                    PTMaskMsg, PTPatternMsg, PTFlowCtlMsg,
                    out uint FilterID);

                AppLogger.WriteLog($"SETUP A NEW FLOW CONTROL FILTER FOR CHANNEL {ChannelID} WITH AN ID OF {FilterID}");
                AppLogger.WriteLog($"FILTER DETAILS\n\\__ MASK:     {MaskMsg}\n\\__ PATTERN:  {PatternMsg}\n\\__ FLOW CTL: {FlowCtlMsg}\n");
            }
            catch (J2534Exception Ex)
            {
                AppLogger.WriteLog($"FAILED TO SETUP MESSAGE FILTER. --> {Ex.Message}");
                AppLogger.WriteErrorLog(Ex);
            }

            HardwareConfig.SelectedHW.SetDeviceValues(Device);
        }
        /// <summary>
        /// Sets up the basic flow ctl filters for an 11 bit can channel.
        /// </summary>
        /// <param name="ChannelID">Channel ID to check.</param>
        /// <param name="Flags">Flags for the filters (Should be 0x40)</param>
        public static void Setup11BitFlowCtl(uint ChannelID, uint Flags = 0x40)
        {
            int StartPattern = (int)0xE8;
            int StartFlowCtl = (int)0xE0;

            for (int Value = 0; Value < 8; Value++)
            {
                string Mask = "FF FF FF FF";
                string Pattern = "00 00 07 " + String.Format("{0:X}", StartPattern + Value);
                string Flow = "00 00 07 " + String.Format("{0:X}", StartFlowCtl + Value);

                SetupFlowCtlFilter(ChannelID, ProtocolId.ISO15765, Flags, Mask, Pattern, Flow);
            }
        }
        /// <summary>
        /// Setup a pass or block filter. Use this to filter out messages.
        /// </summary>
        /// <param name="ChannelID">Channel In use.</param>
        /// <param name="Protocol">ProtocolID - MUST BE 15765 11 BIT</param>
        /// <param name="Flags">Flags for filter</param>
        /// <param name="MaskMsg">Mask Message</param>
        /// <param name="PatternMsg">Pattern Message</param>
        public static void SetupPassOrBlockFilter(uint ChannelID, ProtocolId Protocol, FilterDef TypeOfFilter, uint Flags, string MaskMsg, string PatternMsg)
        {
            var PTMaskMsg = J2534Device.CreatePTMsgFromString(Protocol, Flags, MaskMsg);
            var PTPatternMsg = J2534Device.CreatePTMsgFromString(Protocol, Flags, PatternMsg);

            try
            {
                Device.JapiMarshal.PassThruStartMsgFilter(
                    ChannelID, TypeOfFilter,
                    PTMaskMsg, PTPatternMsg, null,
                    out uint FilterID);

                AppLogger.WriteLog($"SETUP A NEW PASS OR BLOCK FILTER FOR CHANNEL {ChannelID} WITH AN ID OF {FilterID}");
                AppLogger.WriteLog($"FILTER DETAILS\n\\__ MASK:     {MaskMsg}\n\\__ PATTERN:  {PatternMsg}\n");
            }
            catch (J2534Exception Ex)
            {
                AppLogger.WriteLog($"FAILED TO SETUP MESSAGE FILTER. --> {Ex.Message}");
                AppLogger.WriteErrorLog(Ex);
            }

            HardwareConfig.SelectedHW.SetDeviceValues(Device);
        }


        /// <summary>
        /// Clears out the TxAndRx Buffers on the JBox. This should be run when any messages are pulled in
        /// or any messages are sent out to prevent Overflows.
        /// </summary>
        /// <param name="ChannelID">Channel ID In use.</param>
        public static void ClearTXAndRX(uint ChannelID)
        {
            Device.JapiMarshal.PassThruIoctl(ChannelID, IoctlId.CLEAR_RX_BUFFER);
            Device.JapiMarshal.PassThruIoctl(ChannelID, IoctlId.CLEAR_TX_BUFFER);
            AppLogger.WriteLog($"TX AND RX BUFFERS CLEARED OUT OK");
        }
    }
}
