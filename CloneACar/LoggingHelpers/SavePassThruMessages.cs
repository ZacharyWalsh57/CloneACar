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
        public string PathToFileBase;
        public string FullFileName;
        public string FileName;
        public string Address;

        public byte[] AddressByte;

        public SavePassThruMessages(string Protocol, byte[] AddressBytes)
        {
            var Paths = AppConfigHelper.ReturnDebugPaths();
            PathToFileBase = Paths[1].Item2;

            AddressByte = AddressBytes;
            Address = ConvertDataToString(AddressBytes).Replace(" ", String.Empty).Trim();
            FileName = $"Address_{Address}_{Protocol}.json";

            FullFileName = PathToFileBase + $"\\{Protocol}\\{FileName}";

            string JSONDir = PathToFileBase + $"{Protocol}";
            if (!Directory.Exists(JSONDir))
            {
                Directory.CreateDirectory(PathToFileBase + $"\\{Protocol}");
                AppLogger.WriteLog($"DIRECTORY FOR JSON MESSAGES MADE OK");
            }

            if (!File.Exists(FullFileName)) { File.Create(FullFileName); }
            AppLogger.WriteLog($"SET UP A NEW MESSAGE WRITER. FILE {FileName}");
        }

        public void SaveGeneratedMessages(List<PassThruMsg> AllMessagesToSave)
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
    }
}
