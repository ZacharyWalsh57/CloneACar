using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloneACar.J2534Consumer;
using CloneACar.LoggingHelpers;
using CloneACar.Models;

// V0404 API
using Minimal_J2534_0404;
using Newtonsoft.Json;

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
        public static string ConvertDataToString(byte DataByte, bool UseZeroX = false)
        {
            byte[] DataBytes = new byte[1] { DataByte };
            return ConvertDataToString(DataBytes);
        }
        /// <summary>
        /// Used to convert a byte array into a string object.
        /// </summary>
        /// <param name="DataToConvert">Converts these bytes to string.</param>
        /// <param name="UseZeroX">Use the 0x prefix for the string. (0x08 vs 08)</param>
        /// <returns></returns>
        public static string ConvertDataToString(byte[] DataToConvert, bool UseZeroX = false)
        {
            if (DataToConvert == null)
            {
                if (UseZeroX) return "0x00 0x00";
                return "00 00";
            }
            for (int Count = 0; Count < 5; Count++)
            {
                try
                {
                    StringBuilder HexString = new StringBuilder(DataToConvert.Length * 2);
                    foreach (byte ByteItem in DataToConvert)
                    {
                        if (UseZeroX) { HexString.Append("0x"); }
                        HexString.AppendFormat("{0:x2} ".ToUpper(), ByteItem);
                    }
                    return HexString.ToString().Trim();
                }
                catch { }
            }

            if (UseZeroX) return "0x00 0x00";
            return "00 00";
        }
        /// <summary>
        /// Converts a hex string item to a byte array.
        /// </summary>
        /// <param name="InputString">String to send in for the conversion WITHOUT 0x</param>
        /// <returns>byte converted version of the input string.</returns>
        public static byte[] ConvertDataToByte(string InputString)
        {
            string NoSpacesPerm = InputString.Replace(" ", String.Empty).Replace("0x", String.Empty);
            byte[] NextMessage = Enumerable.Range(0, NoSpacesPerm.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(NoSpacesPerm.Substring(x, 2), 16))
                .ToArray();

            return NextMessage;
        }


        /// <summary>
        /// Generates and saves a whole set of address commands for a preset range of values.
        /// </summary>
        /// <param name="DiagBusBytes">Address to make commands for</param>
        /// <param name="MessageWriter">Logger object to write logs with. Can be null</param>
        /// <returns></returns>
        public static PassThruMessageSet GenerateMessagesForAddress(ProtocolId Protocol, int MaxMsgLen, byte[] DiagBusBytes, uint MessageFlags, Logger MessageWriter = null)
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

            #region File Exists

            // Check for file exist.
            string ProtocolString = (Protocol + "").Trim();
            if (MessageFlags == 0x100 && Protocol == ProtocolId.CAN) { ProtocolString += "_29BIT"; }
            if (MessageFlags == 0x40 && Protocol == ProtocolId.ISO15765) { ProtocolString += "_11BIT"; }

            // Get Value for Path to JSON and make address string.
            var Paths = AppConfigHelper.ReturnDebugPaths();
            string Address = ConvertDataToString(DiagBusBytes).Replace(" ", String.Empty).Trim();
            string FileName = $"Address_{Address}_{ProtocolString}.json";

            // Only do this if the file we need doesn't exist.
            string FilePath = $"{Paths[1].Item2}{ProtocolString}\\{FileName}";
            if (File.Exists(FilePath))
            {
                var SizeOfFile = new FileInfo(FilePath).Length;
                if (SizeOfFile == 0) { File.Delete(FilePath); }
            }

            if (File.Exists(FilePath))
            {
                // Stopwatch for All threads.
                Stopwatch ImportGenTimer = new Stopwatch();
                ImportGenTimer.Start();

                // Log File was found ok.
                // AppLogger.WriteLog($"ADDRESS FILE FOUND FOR {Address}. JSON PARSING IT NOW...");
                MessageWriter?.WriteMessageLog($"ADDRESS FILE FOUND FOR {Address}. JSON PARSING IT NOW...");
                
                // Convert the JSON to string messages
                PassThruMessageSet JSONMsgSet;
                using (StreamReader FileReader = File.OpenText(FilePath))
                {
                    JsonSerializer DeserializerObject = new JsonSerializer();
                    JSONMsgSet = (PassThruMessageSet)DeserializerObject.Deserialize(FileReader, typeof(PassThruMessageSet));
                }

                // Log time stats to the debug logs.
                // AppLogger.WriteLog($"COMPLETED JSON PARSE OF ADDRESS SET {ConvertDataToString(DiagBusBytes)} --> {ImportGenTimer.Elapsed.ToString("g")} --> {JSONMsgSet.Messages.Count} MESSAGES");
                MessageWriter?.WriteMessageLog($"COMPLETED JSON PARSE OF ADDRESS SET {ConvertDataToString(DiagBusBytes)} --> {ImportGenTimer.Elapsed.ToString("g")} --> {JSONMsgSet.Messages.Count} MESSAGES");

                // Feed back the message list.
                return JSONMsgSet;
            }
            #endregion

            #region File Does Not Exist

            // List of all PTMessages. This gets converted into an array on return.
            List<PassThruMsg> AllMessagesToSend = new List<PassThruMsg>();

            // Make a list of all usable PIDs for this Protocol, and a list to hold all PTMessages made.
            var ListOfPids = new List<byte> { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A };

            // Stopwatch for All threads.
            Stopwatch GenerationTimer = new Stopwatch();
            GenerationTimer.Start();

            // Write Address info here.
            // AppLogger.WriteLog($"GETTING MESSAGES AT ADDRESS \"{ConvertDataToString(DiagBusBytes, true)}\" FOR ALL STANDARD PIDS...");
            // AppLogger?.WriteMessageLog($"GETTING MESSAGES AT ADDRESS \"{ConvertDataToString(DiagBusBytes, true)}\" FOR ALL STANDARD PIDS...");

            // Loop all PID objects and make address commands.
            var Options = new ParallelOptions() { MaxDegreeOfParallelism = 5 };
            Parallel.ForEach(ListOfPids, Options, (PidItem) =>
            {
                // Make First MsgBytes
                byte[] FirstBytes = new byte[0];
                if (Protocol == ProtocolId.ISO15765) { FirstBytes = new byte[5] { 00, 00, DiagBusBytes[0], DiagBusBytes[1], PidItem }; }

                // Make a byte array here containing the 0x00 0x00, Address bytes, and PID value and store as string.
                string PidAndAddress = ConvertDataToString(FirstBytes);
                var NewMessages = GetNextSequence(MaxMsgLen, PidAndAddress, MessageWriter)
                    .Select(Msg => Msg)
                    .Distinct()
                    .ToList();

                // Loop all the strings and make messages based on the string values.
                foreach (var Message in NewMessages)
                {
                    // Make a message from the string returned out.
                    var NextPTMsg = J2534Device.CreatePTMsgFromString(Protocol, MessageFlags, Message);

                    // Format based on command type.
                    var FormattedPTMessage = new PassThruMsg(0);
                    bool CanAdd = FormatMessage(Protocol, MessageFlags, NextPTMsg, out FormattedPTMessage);

                    // Add if valid message type.
                    if (CanAdd) { AllMessagesToSend.Add(FormattedPTMessage); }
                }
            });

            // Remove dupes from messages.
            AllMessagesToSend = AllMessagesToSend.Select(Msg => Msg).Distinct().ToList();

            // Write messages out to a JSON file now.
            SavePassThruMessages Saver = new SavePassThruMessages(ProtocolString, DiagBusBytes);
            MessageWriter?.WriteMessageLog($"SAVING TO JSON FILE NOW. FILE NAME: {Saver.FileName}");
            Saver.SaveGeneratedMessages(AllMessagesToSend);

            // Log Time Taken.
            // AppLogger.WriteLog($"COMPLETED NEW JSON GENERATION OF ADDRESS SET {ConvertDataToString(DiagBusBytes)} --> {GenerationTimer.Elapsed.ToString("g")} --> {AllMessagesToSend.Count} MESSAGES");
            MessageWriter?.WriteMessageLog($"COMPLETED NEW JSON GENERATION OF ADDRESS SET {ConvertDataToString(DiagBusBytes)} --> {GenerationTimer.Elapsed.ToString("g")} --> {AllMessagesToSend.Count} MESSAGES");
            GenerationTimer.Stop();

            // Convert the list to an array and return out.
            PassThruMessageSet MsgSet = new PassThruMessageSet(Protocol, DiagBusBytes, AllMessagesToSend);
            return MsgSet;

            #endregion
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
                string ByteString = ConvertDataToString(BytesOfMessage);

                // Add to list of strings and get our perms from the split string item.
                // Store perm 0 to be added to again and used for new perm setup.
                try
                {
                    // Get the perms of the next message
                    var PermHelper = new PermutationHelper(ByteString, MessageStart);
                    BytesOfMessage = PermHelper.NextBaseMessage;

                    // Add all the found perms into the list of all values and remove dupes.
                    AllValuesConverted.AddRange(PermHelper.FinalMessages);
                    AllValuesConverted.Add(MessageStart + " " + MaxString.Replace("FF", "00"));

                    // Check for MaxSize
                    KeepSearching = ByteString != MaxString;
                    if (!KeepSearching) { break; }

                    // Increase Counter Value.
                    BytesOfMessage[Array.IndexOf(BytesOfMessage, BytesOfMessage.Min())] += 1;
                }
                catch { }
            }

            // Return the list of strings.
            return AllValuesConverted;
        }
        
        
        /// <summary>
        /// Converts a permuated message into a message used for a specific protocol.
        /// </summary>
        /// <param name="Protocol">Protocol in use.</param>
        /// <param name="MsgFlags">Flags of message.</param>
        /// <param name="Message">Message In</param>
        /// <param name="NewMessage">Message Modified.</param>
        /// <returns>True if message can be added, false if not.</returns>
        private static bool FormatMessage(ProtocolId Protocol, uint MsgFlags, PassThruMsg Message, out PassThruMsg NewMessage)
        {
            NewMessage = Message;
            if (Protocol == ProtocolId.ISO15765)
            {
                var TempData = Message.data;
                int LastIndex = Array.FindLastIndex(TempData, MatchByte => MatchByte != 0);
                Array.Resize(ref TempData, LastIndex + 2);

                NewMessage.txFlags = 0x00;
                if (TempData.Length <= 6)
                {
                    NewMessage.data = TempData;
                    NewMessage.dataLength = (uint)TempData.Length;
                    NewMessage.txFlags = 0x40;
                }
                return true;
            }

            return false;
        }
    }
}
