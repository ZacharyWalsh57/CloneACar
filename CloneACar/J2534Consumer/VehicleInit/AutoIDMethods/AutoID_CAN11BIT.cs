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

namespace CloneACar.J2534Consumer.VehicleInit.AutoIDMethods
{
    public class AutoID_CAN11BIT
    {
        private J2534Device Device
        {
            get { return SelectedHW.GetDeviceValues<J2534Device>(); }
            set { SelectedHW.SetDeviceValues(Device); }
        }

        public string VIN_NUMBER;
        public PassThruMsg[] ModeOnePidZero;
        public PassThruMsg[] VinNumberMessages;

        public AutoID_CAN11BIT()
        {
            // Log whats going on.
            AppLogger.WriteLog("SETTING UP FOR 11 BIT AUTO ID. SETTING DEVICE OBJECT NOW");

            // Open the device and connect to an 11bit can channel.
            if (!Device.isOpen) { Device.PTOpen(Device.name); }
            if (Device.channels[0] != null)
                if (Device.channels[0].protocol != ProtocolId.ISO15765)
                    Device.PTDisconnect(0);
            if (Device.channels[0] == null)
                Device.PTConnect(0, ProtocolId.ISO15765, 0x00, 500000);


            // AutoID Process can now be attempted.
            AppLogger.WriteLog("11 BIT CAN CHANNEL WAS OPENED AND CONNECTED OK. READY TO RUN AUTO ID PROCESS NOW");
        }

        public bool ReadVIN(out string VIN)
        {
            // Open the device and connect to an 11bit can channel.
            if (!Device.isOpen) { Device.PTOpen(Device.name); }
            if (Device.channels[0] != null)
                if (Device.channels[0].protocol != ProtocolId.ISO15765)
                    Device.PTDisconnect(0);
            if (Device.channels[0] == null)
                Device.PTConnect(0, ProtocolId.ISO15765, 0x00, 500000); 

            // Get channel ID.
            uint ChannelID = Device.channels[0].channelId;
            AppLogger.WriteLog($"DEVICE CONNECTED AND OPEN. CHANNEL ID {ChannelID}");

            // Setup all flow control filters.
            SetupStandardFlowCtl(Device.channels[0].channelId);
            AppLogger.WriteLog("ALL FLOW CONTROL FILTERS ARE NOW SETUP OK.");

            // Clear TX and RX buffers.
            WrappedCommands.ClearTXAndRX(ChannelID);

            // Run DF 01
            ModeOnePidZero = WrappedCommands.WriteAndRead(
                ChannelID, ProtocolId.ISO15765, "00 00 07 DF 01 00",
                0x40, 8);
            AppLogger.WriteLog("SENT AND RECEIVE DONE FOR MODE ONE PID 0");

            // Run 09 02
            VinNumberMessages = WrappedCommands.WriteAndRead(
                ChannelID, ProtocolId.ISO15765, "00 00 07 DF 09 02",
                0x40, 8);
            AppLogger.WriteLog("SENT AND RECEIVE DONE FOR VIN NUMBER");

            // Write messages to logs and pull vin out.
            foreach (var PTMessage in VinNumberMessages)
            {
                if (PTMessage.dataLength <= 7) { continue; }

                byte[] VIN_ONLY = PTMessage.data.Skip(7).ToArray();
                VIN = Encoding.Default.GetString(VIN_ONLY);
                VIN_NUMBER = VIN;

                Device.PTDisconnect(0);
                AppLogger.WriteLog($"FOUND A VIN OK! GOT A NEW VIN AS {VIN}", LogTypes.LogItemType.EXEOK);
                AppLogger.WriteLog($"CLOSED DOWN THE CAN CHANNEL USED FOR AUTOID. MOVING ON", LogTypes.LogItemType.EXEOK);
                return true;
            }

            // If no VIN Can be found return false and dump the messages.
            VIN = "UNKNOWN";
            VIN_NUMBER = VIN;

            Device.PTDisconnect(0);
            return false;
        }


        public void SetupStandardFlowCtl(uint ChannelID, uint Flags = 0x40)
        {
            int StartPattern = (int)0xE8;
            int StartFlowCtl = (int)0xE0;

            for (int Value = 0; Value < 8; Value++)
            {
                string Mask = "FF FF FF FF";
                string Pattern = "00 00 07 " + String.Format("{0:X}", StartPattern + Value);
                string Flow = "00 00 07 " + String.Format("{0:X}", StartFlowCtl + Value);

                WrappedCommands.SetupFlowCtlFilter(ChannelID, ProtocolId.ISO15765, Flags, Mask, Pattern, Flow);
            }
        }
    }
}
