using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloneACar.LoggingHelpers;

// V0404 API
using Minimal_J2534_0404;

// Global Objects.
using static CloneACar.GlobalObjects;

namespace CloneACar.LogicalHelpers
{
    public class DataByteHelpers
    {
        /// <summary>
        /// Used to convert a byte array into a string object.
        /// </summary>
        /// <param name="DataToConvert">Converts these bytes to string.</param>
        /// <param name="UseZeroX">Use the 0x prefix for the string. (0x08 vs 08)</param>
        /// <returns></returns>
        public static string ConvertDataToString(byte[] DataToConvert, bool UseZeroX = false)
        {
            StringBuilder HexString = new StringBuilder(DataToConvert.Length * 2);
            foreach (byte ByteItem in DataToConvert)
            {
                if (UseZeroX) { HexString.Append("0x"); }
                HexString.AppendFormat("{0:x2} ".ToUpper(), ByteItem);
            }

            return HexString.ToString().Trim();
        }


        /// <summary>
        /// Generates and saves a whole set of address commands for a preset range of values.
        /// </summary>
        /// <param name="DiagBusBytes">Address to make commands for</param>
        /// <param name="MessageWriter">Logger object to write logs with. Can be null</param>
        /// <returns></returns>
        public static PassThruMsg[] GenerateMessagesForAddress(string Protocol, int MaxMsgLen, byte[] DiagBusBytes, Logger MessageWriter = null)
        {
            #region How This Works

            // Message Setup and Creation Rules.
            #region Message Structure
            // Find how many messages can be made from the start to the end.
            // CAN 11 Bit can only be 11 Bits long.
            // Example Message:
            //   1    2    3    4    5    6    7    8    9    10   11
            // (0x00 0x00 0x07 0xE0 0x?? 0x?? 0x?? 0x?? 0x?? 0x?? 0x??)
            #endregion

            // PID Values to cycle through.
            #region OBD2 Pids
            // Loop all the basic OBD2 Pid values first.
            // 01 - Current Data
            // 02 - Freeze Frame
            // 03 - DTC
            // 04 - Clear DTC
            // 05 - Test Results (NON CAN ONLY SO SKIP)
            // 06 - Test Results comp monitors (CAN ONLY)
            // 07 - Pending DTCs
            // 08 - Control Operation of systems/components
            // 09 - VIN
            // 0A - Permanent DTCs.
            #endregion

            #endregion

            // If we already have a JSON, dont remake it.
            string PathOfJSON = 
                $@"C:\Drewtech\CloneACar\CloneACar_JSON_Messages" +
                $@"\{Protocol}" +
                $@"\CloneACar_{ConvertDataToString(DiagBusBytes).Substring(1).
                    Replace(" ", String.Empty).Trim()}" +
                $@"_{Protocol}.json";

            if (File.Exists(PathOfJSON))
            {
                // This needs to be changed so it knows how to read in the JSON File.
                return new PassThruMsg[0];
            }

            // List of all PTMessages. This gets converted into an array on return.
            List<PassThruMsg> AllMessagesToSend = new List<PassThruMsg>();

            // Make a list of all usable PIDs for this Protocol, and a list to hold all PTMessages made.
            var ListOfPids = new List<byte> { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A };

            // Stopwatch for All threads.
            Stopwatch GenerationTimer = new Stopwatch();
            GenerationTimer.Start();

            // Write Address info here.
            // AppLogger.WriteLog($"GETTING MESSAGES AT ADDRESS \"{ConvertDataToString(DiagBusBytes, true)}\" FOR ALL STANDARD PIDS...");
            MessageWriter?.WriteMessageLog($"GETTING MESSAGES AT ADDRESS \"{ConvertDataToString(DiagBusBytes, true)}\" FOR ALL STANDARD PIDS...");

            // Loop all PID objects and make address commands.
            var Options = new ParallelOptions() { MaxDegreeOfParallelism = 5 };
            Parallel.ForEach(ListOfPids, Options, (PidItem) =>
            {            
                // Stopwatch for pid threads.
                Stopwatch PIDTimer = new Stopwatch();
                PIDTimer.Start();
                
                // Make a byte array here containing the 0x00 0x00, Address bytes, and PID value and store as string.
                string PidAndAddress = ConvertDataToString(new byte[5] { 00, 00, DiagBusBytes[0], DiagBusBytes[1], PidItem });
                string AddressOnly = ConvertDataToString(DiagBusBytes);
                string PidString = ConvertDataToString(new byte[1] {PidItem});

                // Get all new byte strings.
                var NewMessages = GetNextSequence(MaxMsgLen, PidAndAddress, MessageWriter);

                // Loop all the strings and make messages based on the string values.
                foreach (var Message in NewMessages)
                {
                    var NextPTMsg = J2534Device.CreatePTMsgFromString(ProtocolId.ISO15765, 0x40, Message);
                    AllMessagesToSend.Add(NextPTMsg);
                }

                // Log the messages to temp file and info for the run. THIS WRITE TAKES A BIT OF TIME.
                // AppLogger.WriteLog($"FOR CMD {PidAndAddress} MADE A TOTAL OF {NewMessages.Count} MESSAGES"); 
                // AppLogger.WriteLog($"NOW HAVE A TOTAL OF {AllMessagesToSend.Count} MESSAGES TO SEND OUT");
                // AppLogger.WriteLog($"COMPLETED ALL MESSAGES FOR {PidAndAddress} IN ABOUT {PIDTimer.Elapsed:g}");
                // PIDTimer.Stop();
            });

            // Write messages out to a JSON file now.
            SavePassThruMessages Saver = new SavePassThruMessages(DiagBusBytes, Protocol);
            AppLogger.WriteLog($"SAVING TO JSON FILE NOW. FILE NAME: {Saver.FileName}");
            MessageWriter?.WriteMessageLog($"SAVING TO JSON FILE NOW. FILE NAME: {Saver.FileName}");
            Saver.SaveGeneratedMessages(AllMessagesToSend);

            // Log Time Taken.
            AppLogger.WriteLog($"COMPLETED ADDRESS SET {ConvertDataToString(DiagBusBytes)} IN {GenerationTimer.Elapsed.ToString("g")}");
            AppLogger.WriteLog($"FOUND A TOTAL OF {AllMessagesToSend.Count} MESSAGES HAVE BEEN GENERATED");
            AppLogger.WriteLog("THIS SHOULD BE CONSISTENT ACROSS ALL ADDRESSES\n");

            MessageWriter?.WriteMessageLog($"COMPLETED ADDRESS SET {ConvertDataToString(DiagBusBytes)} IN {GenerationTimer.Elapsed.ToString("g")}");
            MessageWriter?.WriteMessageLog($"FOUND A TOTAL OF {AllMessagesToSend.Count} MESSAGES HAVE BEEN GENERATED");
            MessageWriter?.WriteMessageLog("THIS SHOULD BE CONSISTENT ACROSS ALL ADDRESSES");

            GenerationTimer.Stop();

            // Convert the list to an array and return out.
            return AllMessagesToSend.ToArray();
        }
        /// <summary>
        /// Make a list of string items for the next perm list of bytes passed in.
        /// </summary>
        /// <param name="NextBytes">byte object of the address, and pid to count from.</param>
        /// <returns>List string of all byte objects and orders.</returns>
        private static List<string> GetNextSequence(int MessageLength, string MessageStart, Logger MessageLogger = null)
        {
            // Make max size string based on the largest message possible for this protocol.
            string MaxString = "";
            for (int Count = 0; Count < MessageLength; Count++) { MaxString += "FF "; }
            MaxString = MaxString.Trim();

            // Make a new byte[] of 0s for the loop to count up in and a list to hold all results.
            byte[] BytesOfMessage = new byte[MessageLength];
            List<string> AllValuesConverted = new List<string>();

            bool KeepSearching = true;
            while (KeepSearching)
            {
                // Store it as an int array for comparison and pull the lowest value to increase it by one.
                BytesOfMessage[Array.IndexOf(BytesOfMessage, BytesOfMessage.Min())] += 1;
                string ByteString = ConvertDataToString(BytesOfMessage);

                // Add to list of strings and get our perms from the split string item.
                // Store perm 0 to be added to again and used for new perm setup.
                try
                {
                    var PermHelper = new PermutationHelper(ByteString, MessageStart);
                    BytesOfMessage = PermHelper.NextBaseMessage;

                    // Add all the found perms into the list of all values and remove dupes.
                    AllValuesConverted.AddRange(PermHelper.FinalMessages);

                    // Write msgs to log file. Moved to write log out when done combining messages.
                    // MessageLogger?.WriteMessageLog($"MADE {PermHelper.FinalMessages.Count} " +
                    //                                $"MESSAGES FROM --> {PermHelper.FinalMessages[0]}",
                    //                                   MessageLogTypes.MessageTypes.CLNE_MSG);


                    // Check for MaxSize
                    KeepSearching = ByteString != MaxString;
                    if (!KeepSearching) { break; }
                }
                catch { }
            }

            // Return the list of strings.
            return AllValuesConverted;
        }
    }
}
