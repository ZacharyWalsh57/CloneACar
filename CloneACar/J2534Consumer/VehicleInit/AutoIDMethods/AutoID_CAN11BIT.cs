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

namespace CloneACar.J2534Consumer.VehicleInit.AutoIDMethods
{
    public class AutoID_CAN11BIT
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

        public string VIN_NUMBER;
        public PassThruMsg[] ModeOnePidZero;
        public PassThruMsg[] VinNumberMessages;

        private readonly Logger VinLogger11Bit;
 
        public AutoID_CAN11BIT()
        {
            // Log whats going on.
            VinLogger11Bit = new Logger(true, "ISO15765-11BIT", "AUTO_ID");
            AppLogger.WriteLog("SETTING UP FOR 11 BIT AUTO ID. SETTING DEVICE OBJECT NOW");

            // Open the device and connect to an 11bit can channel.
            WrappedCommands.OpenDevice(ProtocolId.ISO15765, 0x00, 500000);

            // AutoID Process can now be attempted.
            AppLogger.WriteLog("11 BIT CAN CHANNEL WAS OPENED AND CONNECTED OK. READY TO RUN AUTO ID PROCESS NOW");
        }

        public bool ReadVIN(out string VIN)
        {
            // Open the device and connect to an 11bit can channel.
            WrappedCommands.OpenDevice(ProtocolId.ISO15765, 0x00, 500000);

            // Get channel ID.
            uint ChannelID = Device.channels[0].channelId;
            AppLogger.WriteLog($"DEVICE CONNECTED AND OPEN. CHANNEL ID {ChannelID}");

            // Setup all flow control filters.
            WrappedCommands.Setup11BitFlowCtl(ChannelID, 0x40);
            AppLogger.WriteLog("ALL FLOW CONTROL FILTERS ARE NOW SETUP OK.");

            // Clear TX and RX buffers.
            WrappedCommands.ClearTXAndRX(ChannelID);

            // Run DF 01
            ModeOnePidZero = WrappedCommands.WriteAndRead(
                ChannelID, ProtocolId.ISO15765, "00 00 07 DF 01 00",
                0x40, 8, VinLogger11Bit);
            AppLogger.WriteLog("SENT AND RECEIVE DONE FOR MODE ONE PID 0");

            // Run 09 02
            VinNumberMessages = WrappedCommands.WriteAndRead(
                ChannelID, ProtocolId.ISO15765, "00 00 07 DF 09 02",
                0x40, 8, VinLogger11Bit);
            AppLogger.WriteLog("SENT AND RECEIVE DONE FOR VIN NUMBER");

            // Write messages to logs and pull vin out.
            if (VinNumberMessages != null)
            {
                foreach (var PTMessage in VinNumberMessages)
                {
                    if (PTMessage.dataLength <= 7) { continue; }

                    byte[] VIN_ONLY = PTMessage.data.Skip(7).ToArray();
                    VIN = Encoding.Default.GetString(VIN_ONLY);
                    VIN_NUMBER = VIN;

                    Device.PTDisconnect(0);
                    AppLogger.WriteLog($"FOUND A VIN OK! GOT A NEW VIN AS {VIN}", TextLogTypes.LogItemType.EXEOK);
                    AppLogger.WriteLog($"CLOSED DOWN THE CAN CHANNEL USED FOR AUTOID. MOVING ON", TextLogTypes.LogItemType.EXEOK);

                    // VinLogger11Bit.WriteLog("VIN NUMBER FOUND OK! --> " + VIN, TextLogTypes.LogItemType.EXEOK);
                    return true;
                }
            }

            // If no VIN Can be found return false and dump the messages.
            VIN = "UNKNOWN";
            VIN_NUMBER = VIN;

            Device.PTDisconnect(0);
            return false;
        }
    }
}
