using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using CloneACar.LoggingHelpers;

// Using call for the Minimal 0404 DLL
using Minimal_J2534_0404;

// Globals
using static CloneACar.GlobalObjects;
using static CloneACar.LogicalHelpers.DataByteHelpers;

namespace CloneACar.J2534Consumer.VehicleCloning
{
    public class ISO15765_11BIT_Cloning
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

        private Logger MsgLogger;
        private List<PassThruMsg> DiagCommandMessages = new List<PassThruMsg>();

        public ISO15765_11BIT_Cloning()
        {
            // Make a clone logger.
            MsgLogger = new Logger(true, "ISO15765-11BIT", "CLONER");
            MsgLogger.WriteLog($"INIT OF 11BIT CAN LOGGER WAS OK! FILE AND DIR STRUCTURE SEEMS RIGHT");

            // Log whats going on.
            AppLogger.WriteLog("SETTING UP FOR 11 BIT CLONING SESSION. SETTING DEVICE OBJECT NOW");

            // Open the device and connect to an 11bit can channel then close it so we dont waste resources keeping it open.
            WrappedCommands.OpenDevice(ProtocolId.ISO15765, 0x00, 500000);
            Device.PTDisconnect(0);

            // AutoID Process can now be attempted.
            AppLogger.WriteLog("11 BIT CAN CHANNEL WAS OPENED AND CONNECTED OK. READY TO RUN CLONING PROCESS NOW");
        }


        public void StartClone()
        { 
            // Make all 11 Bit CAN Messages here.
            if (!GenerateAllMessages()) { return; }

            // Init the FlowCtl Filters for this channel.
            uint Channel = Device.channels[0].channelId;
            WrappedCommands.Setup11BitFlowCtl(Channel);
        }


        public bool GenerateAllMessages()
        {
            // List of all byte addresses with 7DF manually added.
            var BusAddresses = new List<byte[]>();
            BusAddresses.Add(new byte[2] { 0x07, 0xDF });

            // Index of counted addresses.
            int Added = 0;
            List<string> AddressList = new List<string>();
            string TempList = "";

            // Add all the addresses in here.
            for (int Value = 0; Value < 0xFF; Value++)
            {
                var NextAddress = new byte[2] { 0x07, (byte)Value };

                if (Added < 10)
                {
                    TempList += ConvertDataToString(NextAddress, true) + ", ";
                    Added++;
                }
                if (Added == 10)
                {
                    AddressList.Add(TempList);
                    TempList = "";
                    Added = 0;
                }

                BusAddresses.Add(NextAddress);
            }

            // Write addresses to the log file.
            AppLogger.WriteLog($"GENERATED ALL ADDRESS SETS OK. MADE A TOTAL OF {BusAddresses.Count} ADDRESS PAIRS");
            AppLogger.WriteLog($"ADDRESS LOCATIONS TO SEND TO:\n\n--- {string.Join("\n--- ", AddressList)}\n");

            // Init list of all diag messages.
            DiagCommandMessages = new List<PassThruMsg>();

            // Split Addresses into Chunks
            var SplitBusAddresses = BusAddresses
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / 10)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();

            // Log Counts here.
            AppLogger.WriteLog("STARTING GENERATION OF ALL MESSAGES FOR ALL ADDRESSES ON THE CAN BUS");
            AppLogger.WriteLog("WILL TAKE A LONG TIME. FRANKLY, I HAVE NO IDEA HOW LONG BUT LONG.");
            AppLogger.WriteLog($"ADDRESSES: {BusAddresses.Count} --- THREADS: {SplitBusAddresses.Count} --- KICKING OFF NOW...");

            // Make a timer to time this whole thing.
            Stopwatch GenerationTimer = new Stopwatch();
            GenerationTimer.Start();

            // Parallel loop all values found in here to make get command messages.
            // Doing 10 Lists, which split up makes 26 Lists or threads. This might be a little better.
            string ProtocolString = ProtocolId.ISO15765 + "_11BIT";
            var Options = new ParallelOptions() { MaxDegreeOfParallelism = 100 };

            Parallel.ForEach(SplitBusAddresses, Options, AddressSet =>
                Parallel.ForEach(AddressSet, Options, Addresses =>
                    DiagCommandMessages.AddRange(
                        GenerateMessagesForAddress(ProtocolString, 6, Addresses, MsgLogger).ToList())
                )
            );

            // Log status output here.
            AppLogger.WriteLog($"FINALLY FUCKIN DONE. MADE A SHITLOAD OF MESSAGES AT A STUNNING {DiagCommandMessages.Count} COMMANDS GENERATED");
            AppLogger.WriteLog($"TOOK APPROXIMATELY {GenerationTimer.Elapsed.ToString("g")}");
            GenerationTimer.Stop();

            // Return if we made any good commands. Usually yes.
            return DiagCommandMessages.Count > 0;
        }
    }
}
