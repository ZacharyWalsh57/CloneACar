using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using CloneACar.LoggingHelpers;

// Using call for the Minimal 0404 DLL
using Minimal_J2534_0404;
using Newtonsoft.Json;

// Globals
using static CloneACar.GlobalObjects;
using static CloneACar.LogicalHelpers.DataByteHelpers;
using CloneACar.Models;

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

        public List<PassThruMessageSet> DiagCommandMessages = new List<PassThruMessageSet>();
        public List<ModuleCommunicationResults> ModuleCommResults = new List<ModuleCommunicationResults>();

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


        /// <summary>
        /// Generates message sets and stores them in DiagCommandMessages.
        /// </summary>
        /// <returns>A List of PTMessageSets for messages to be SENT out to the vehicle.</returns>
        public List<PassThruMessageSet> GenerateAllMessages()
        {
            // List of all byte addresses with 7DF manually added.
            var BusAddresses = new List<byte[]>();
            BusAddresses.Add(new byte[2] { 0x07, 0xDF });

            // Index of counted addresses.
            int Added = 0;
            List<string> AddressList = new List<string>();
            string TempList = "";

            // Add all the addresses in here.
            for (int Value = 0; Value < 0xF7; Value++)
            {
                var NextAddress = new byte[2] { 0x07, (byte)Value };
                if (NextAddress[1] == 0xDF) { continue; }

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
            var DiagMessages = new List<PassThruMessageSet>();
            DiagCommandMessages = new List<PassThruMessageSet>();

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
            var Options = new ParallelOptions() { MaxDegreeOfParallelism = 10 };
            Parallel.ForEach(SplitBusAddresses, Options, AddressSet =>
                Parallel.ForEach(AddressSet, Options, Addresses =>
                    DiagMessages.Add(GenerateMessagesForAddress(ProtocolId.ISO15765, 6, Addresses, 0x40, MsgLogger))
                )
            );

            // Log status output here.
            AppLogger.WriteLog($"FINALLY FUCKIN DONE. MADE A SHITLOAD OF MESSAGES AT A STUNNING {DiagMessages.Count} COMMANDS GENERATED");
            AppLogger.WriteLog($"TOOK APPROXIMATELY {GenerationTimer.Elapsed.ToString("g")}");
            GenerationTimer.Stop();

            // Return if we made any good commands. Usually yes.
            return DiagMessages;
        }
        /// <summary>
        /// Clones all modules on bus. Starting from 0x07 0x00 to 0x07 0xFF
        /// Auto turns on flow control when needed for the desired messages.
        /// </summary>
        /// <returns>Bool if any comms are found.</returns>
        public bool CloneAllModules()
        {
            // Open Device if needed and setup basic flow ctl.
            WrappedCommands.OpenDevice(ProtocolId.ISO15765, 0x00, 500000);
            AppLogger.WriteLog("DEVICE OPENED AND 11 BIT FLOW CONTROL FILTERS SETUP OK");

            // Loop each address set here 
            foreach (var MessageSet in DiagCommandMessages)
            {
                // Make Message Sets and local vars.
                string ProtocolString = (ProtocolId.ISO15765 + "_11BIT").Trim();
                string SendAddr = MessageSet.AddressString;

                // Make an object to hold response messages  .Make an arg object for it.
                ModuleCommunicationResults ModuleRespSet;
                CloneMethodArgs Args = new CloneMethodArgs
                {
                    BaudRate = 500000,
                    ChannelFlags = 0x00,
                    MessageSet = MessageSet,
                    ProtocolString = ProtocolString
                };
                
                // Run cloner and pass out the result based on what happens.
                ExecuteModuleClone CloneInvoker = new ExecuteModuleClone(ProtocolId.ISO15765, ProtocolString, SendAddr);
                if (CloneInvoker.CloneModuleSet(Args, out ModuleRespSet))
                {
                    AppLogger.WriteLog($"FOUND COMMS AT ADDRESS SET {SendAddr} OK!");
                    AppLogger.WriteLog($"LOOKS LIKE A TOTAL OF {ModuleRespSet.MessagePairs.Count} MESSAGES WERE PAIRED OFF");
                    ModuleCommResults.Add(ModuleRespSet);
                }
                else { AppLogger.WriteLog("FAILED TO FIND COMMS AT ADDRESS SET 0x07 0xDF!"); }
            }

            // Clear filters and buffers.
            Device.PTDisconnect(0);
            AppLogger.WriteLog($"DEVICE DISCONNECTED. SHOULD BE OK TO USE IT ELSEWHERE IF WANTED.");

            // Return if we got any comms at all for any addresses.
            bool FoundComms = ModuleCommResults.Count > 0;
            AppLogger.WriteLog($"RETURNING: {FoundComms} --> CHANNEL AND CLEARED BUFFS/FILTERS OK");
            return FoundComms;
        }
    }
}
