using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloneACar.LogicalHelpers;
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
        private string PathToFileBase;
        private string PathToUserData;
        private string VIN_NUMBER;

        public string FullFileName;
        public string FileName;

        private string Address;
        private byte[] AddressByte;

        public SavePassThruMessages(string Protocol, byte[] AddressBytes)
        {
            var Paths = AppConfigHelper.ReturnDebugPaths();
            PathToFileBase = Paths[1].Item2;
            PathToUserData = Paths[2].Item2;

            string JSONDir = PathToFileBase + $"{Protocol}";

            AddressByte = AddressBytes;
            Address = ConvertDataToString(AddressBytes).Replace(" ", String.Empty).Trim();
            FileName = $"Address_{Address}_{Protocol}.json";
            FullFileName = PathToFileBase + $"\\{Protocol}\\{FileName}";

            if (!Directory.Exists(JSONDir)) AppLogger.WriteLog($"DIRECTORY FOR JSON MESSAGES MADE OK");
            Directory.CreateDirectory(PathToFileBase + $"\\{Protocol}");

            if (!File.Exists(FullFileName)) { File.Create(FullFileName); }
        }
        public SavePassThruMessages(byte[] AddressBytes, string VIN = "")
        {
            if (VIN == "") { VIN = "UNKNOWN_VIN"; }
            VIN_NUMBER = VIN;

            AddressByte = AddressBytes;
            Address = ConvertDataToString(AddressBytes).Replace(" ", String.Empty).Trim();
            FileName = $"Module_{Address}_CommsData.json";
            FullFileName = PathToUserData + $"\\Modules_{VIN_NUMBER}\\{FileName}";

            if (!Directory.Exists(PathToUserData)) AppLogger.WriteLog($"DIRECTORY FOR MODULE COMMUNICATIONS MESSAGES MADE OK");
            Directory.CreateDirectory(PathToUserData + $"\\Modules_{VIN_NUMBER}");

            if (!File.Exists(FullFileName)) { File.Create(FullFileName); }
        }

        public void SaveGeneratedMessages(List<PassThruMsg> AllMessagesToSave, string VIN_NUMBER = "")
        {
            // Message Set object and JSON it.
            var ProcID = AllMessagesToSave[0].protocolId;
            PassThruMessageSet MessageSet = new PassThruMessageSet(ProcID, AddressByte, AllMessagesToSave);
            string FullJSONString = JsonConvert.SerializeObject(MessageSet);

            // Write the file contents out now.
            while (true)
            {
                try
                {
                    var FileToWrite = new FileStream(FullFileName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                    var FileStreamWriter = new StreamWriter((Stream)FileToWrite);

                    FileStreamWriter.WriteLine(FullJSONString);

                    FileStreamWriter.Close();
                    FileToWrite.Close();

                    return;
                }
                catch { System.Threading.Thread.Sleep(1000); }
            }
        }
        public void SaveModuleCommunications(ModuleCommunicationResults Results)
        {
            // Make JSON String to save.
            string FullJSONString = JsonConvert.SerializeObject(Results);

            // Write the file contents out now.
            while (true)
            {
                try
                {
                    var FileToWrite = new FileStream(FullFileName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                    var FileStreamWriter = new StreamWriter((Stream)FileToWrite);

                    FileStreamWriter.WriteLine(FullJSONString);

                    FileStreamWriter.Close();
                    FileToWrite.Close();

                    return;
                }
                catch { System.Threading.Thread.Sleep(1000); }
            }
        }
    }
}
