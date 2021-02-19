using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloneACar.LoggingHelpers;

// Using call for the Minimal 0404 DLL
using Minimal_J2534_0404;
using CloneACar.Models;

// Globals
using static CloneACar.GlobalObjects;
using static CloneACar.GlobalObjects.Version0404Objects;

namespace CloneACar.J2534Consumer
{
    public class WrappedCommands
    {
        private static J2534Device Device
        {
            get { return SelectedHW.GetDeviceValues<J2534Device>(); }
            set { SelectedHW.SetDeviceValues(Device); }
        }

        public static PassThruMsg[] WriteAndRead(uint ChannelID, ProtocolId Protocol, string MessageToSend, uint MessageFlags, uint MessagesToRead)
        {
            WriteMessage(ChannelID, Protocol, MessageToSend, MessageFlags);
            return ReadMessages(ChannelID, MessagesToRead);
        }
        public static PassThruMsg[] WriteAndRead(uint ChannelID, ProtocolId Protocol, List<string> MessagesToSend, uint MessageFlags, uint MessagesToRead)
        {
            foreach (var Message in MessagesToSend) { WriteMessage(ChannelID, Protocol, Message, MessageFlags); }
            return ReadMessages(ChannelID, MessagesToRead);
        }


        public static void WriteMessage(uint ChannelID, ProtocolId Protocol, string MessageToSend, uint MsgFlags)
        {
            var MessageItem = J2534Device.CreatePTMsgFromString(Protocol, MsgFlags, MessageToSend);
            uint MsgCount = 1;

            AppLogger.WriteLog($"WRITING MESSAGE TO CHANNEL {ChannelID} --> {MessageToSend}");
            try { Device.JapiMarshal.PassThruWriteMsgs(ChannelID, MessageItem, ref MsgCount, 250); }
            catch (J2534Exception ex)
            {
                AppLogger.WriteLog($"ERROR WHILE SENDING MESSAGE --> {ex.Message}", LogTypes.LogItemType.ERROR);
                AppLogger.WriteErrorLog(ex);
            }
        }
        public static PassThruMsg[] ReadMessages(uint ChannelID, uint MessagesToRead)
        {
            var MessagesRead = new PassThruMsg[MessagesToRead];
            try { Device.JapiMarshal.PassThruReadMsgs(ChannelID, out MessagesRead, ref MessagesToRead, 500); }
            catch (J2534Exception ex)
            {
                AppLogger.WriteLog($"ERROR WHILE READING MESSAGES --> {ex.Message}", LogTypes.LogItemType.ERROR);
                AppLogger.WriteErrorLog(ex);

                ClearTXAndRX(ChannelID);
                return MessagesRead;
            }

            int Count = 0;
            AppLogger.WriteLog($"READ BACK MESSAGES OK! FOUND {MessagesRead.Length} MESSAGES IN TOTAL. FEEDING THEM OUT NOW");
            foreach (var PTMessage in MessagesRead)
            {
                // NEED TO PUT SOMETHING IN HERE SO THIS KNOWS TO WRITE TO THE WINDOW THAT SHOWS UP TO THE USER.
                AppLogger.WriteLog($"MESSAGE[{Count}] --> {ConvertDataToString(PTMessage.data)}");
                Count++;
            }

            ClearTXAndRX(ChannelID);
            return MessagesRead;
        }


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

            SelectedHW.SetDeviceValues(Device);
        }
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

            SelectedHW.SetDeviceValues(Device);
        }


        public static void ClearTXAndRX(uint ChannelID)
        {
            Device.JapiMarshal.PassThruIoctl(ChannelID, IoctlId.CLEAR_RX_BUFFER);
            Device.JapiMarshal.PassThruIoctl(ChannelID, IoctlId.CLEAR_TX_BUFFER);
            AppLogger.WriteLog($"TX AND RX BUFFERS CLEARED OUT OK");
        }
    }
}
