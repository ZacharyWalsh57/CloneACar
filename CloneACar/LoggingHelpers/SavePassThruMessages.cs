using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloneACar.Models;

// JSON 
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

// V0404 API
using Minimal_J2534_0404;

// Data Helpers
using static CloneACar.LogicalHelpers.DataByteHelpers;
using static CloneACar.GlobalObjects;

namespace CloneACar.LoggingHelpers
{
    public class SavePassThruMessages
    {
        public string PathToFileBase = @"C:\Drewtech\CloneACar\CloneACar_JSON_Messages";
        public string PathToSavedComms = @"C:\Drewtech\CloneACar\CloneACar_SavedComms";

        public string FullFileName;
        public string FileName;
        public string Address;

        public PassThruMsg[] MessagesToSave;
        public StringMessagePairs MessagePairsToSave;

        public SavePassThruMessages(byte[] AddressBytes, string Protocol)
        {
            Address = ConvertDataToString(AddressBytes).Substring(1).Replace(" ", String.Empty).Trim();
            FileName = $"CloneACar_{Address}_{Protocol}.json";
            FullFileName = PathToFileBase + $"\\{Protocol}\\{FileName}";

            string JSONDir = PathToFileBase + $"\\{Protocol}";
            if (!Directory.Exists(JSONDir))
            {
                Directory.CreateDirectory(PathToFileBase + $"\\{Protocol}");
                AppLogger.WriteLog($"DIRECTORY FOR JSON MESSAGES MADE OK");
            }

            if (!File.Exists(FullFileName)) { File.Create(FullFileName); }
            AppLogger.WriteLog($"SET UP A NEW MESSAGE WRITER. FILE {FileName}");
        }
        public SavePassThruMessages(StringMessagePairs MessagesToSave, string Protocol)
        {
            MessagePairsToSave = MessagesToSave;

            Address = MessagesToSave.SendAddress.Substring(1).Replace(" ", String.Empty).Trim() +
                      MessagesToSave.ReceiveAddress.Substring(1).Replace(" ", String.Empty).Trim(); ;

            FileName = $"CloneACar_SavedComms_{Address}_{Protocol}.json";
            FullFileName = PathToSavedComms + $"\\{Protocol}\\{FileName}";

            string SavedCommsDir = PathToSavedComms + $"\\{Protocol}";
            if (!Directory.Exists(SavedCommsDir))
            {
                Directory.CreateDirectory(PathToSavedComms + $"\\{Protocol}");
                AppLogger.WriteLog($"DIRECTORY FOR SAVED COMMS MADE OK");
            }

            if (!File.Exists(FullFileName)) { File.Create(FullFileName); }
            AppLogger.WriteLog($"SET UP A NEW COMMS SAVER. FILE {FileName}");
        }

        public void SaveGeneratedMessages(List<PassThruMsg> AllMessagesToSave)
        {
            // Set the messages to save as an array.
            MessagesToSave = AllMessagesToSave.ToArray();

            // Generate string JSON versions here.
            var AllStringMessages = new List<StrippedPTMessage>();
            foreach (var PTMessage in MessagesToSave)
            {
                if (PTMessage == null) { continue; }

                var StringifiedMessage = new StrippedPTMessage(PTMessage);
                AllStringMessages.Add(StringifiedMessage);
            }

            // Make JSON String now.
            string FullJSONString = JsonConvert.SerializeObject(AllStringMessages);

            // Write the file contents out now.
            try
            {
                var FileToWrite = new FileStream(FullFileName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                var FileStreamWriter = new StreamWriter((Stream)FileToWrite);

                FileStreamWriter.WriteLine(FullJSONString);

                FileStreamWriter.Close();
                FileToWrite.Close();
            }
            catch { }

        }
    }
}
