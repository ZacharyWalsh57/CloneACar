using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Minimal_J2534_0404;

// Globals use
using static CloneACar.GlobalObjects;
using static CloneACar.LogicalHelpers.DataByteHelpers;

namespace CloneACar.LoggingHelpers
{
    public class Logger
    {
        private static string LogBase = @"C:\DrewTech\CloneACar\CloneACar_Logs\";   // Base output path.
        public string PathToJSON = @"C:\Drewtech\CloneACar\CloneACar_JSON_Messages";
        public string PathToComms = @"C:\Drewtech\CloneACar\CloneACar_SavedComms";

        private static bool IsMessageLog = false;               // Sets if we are logging clone messages or not.
        private static string BaseLogFile;                      // Log File for Main Logging.
        private static string BaseLogFilePath;                  // Path for base log file.
        private static string MessageLogFile;                   // Log File For Messages.
        private static string MessageLogFilePath;               // Path for Log File Messages.

        // Name of log without path
        public static string LogFileName
        {
            get
            {
                if (IsMessageLog) { return MessageLogFile; }
                return BaseLogFile;
            }

            set
            {
                if (IsMessageLog) { MessageLogFile = value; }
                BaseLogFile = value;
            }
        }

        // Name of log with path.
        public static string LogFilePath
        {
            get
            {
                if (IsMessageLog) { return MessageLogFilePath; }
                return BaseLogFilePath;
            }

            set
            {
                if (IsMessageLog) { MessageLogFilePath = value; }
                BaseLogFilePath = value;
            }
        }

        public Logger(bool IsMessageLogger = false, string ProtocolID = "", string LoggerName = "")
        {
            // Make LogDir If it does not exist.
            Directory.CreateDirectory(LogBase);
            Directory.CreateDirectory(PathToJSON);
            Directory.CreateDirectory(PathToComms);

            // Delete all old logs if we're in debug mode.
            if (!IsMessageLogger)
            {
                EraseAllHistory(@"C:\Drewtech\CloneACar\CloneACar_Logs");
                EraseAllHistory(@"C:\Drewtech\CloneACar\CloneACar_JSON_Messages");
                // EraseAllHistory(@"C:\Drewtech\CloneACar\CloneACar_SavedComms");
            }

            // Save IsMessageLogger and Protocol.
            IsMessageLog = IsMessageLogger;

            // Get DateTimeNow.
            var strTime = DateTime.Now.ToString("s")
                .Replace("T", "_")
                .Replace("-", "")
                .Replace(":", "");

            // Setup new log file if needed.
            if (BaseLogFile == null)
            {
                // Make Log Base Dir
                LogBase += "CloneACar_" + strTime + "\\";

                // Make a new Main Log file and set its path
                BaseLogFile = "CloneACar_MainLog.txt";
                BaseLogFilePath = LogBase + BaseLogFile;

                // Make the Base Log Dir if it does not exist.
                Directory.CreateDirectory(LogBase);
            }

            // If we're doing message logging only.
            if (IsMessageLogger)
            {
                // Set Proc Name.
                if (ProtocolID == "") { ProtocolID = "UNKNOWN-PROC"; }

                // Make new base path for the message log files.
                // Sample Dir
                // C:\DrewTech\Logs\CloneACar_YYYYMMDD_HHMMSS\MessageLogging\PROTOCOL\

                // Sample File
                // C:\DrewTech\Logs\CloneACar_YYYYMMDD_HHMMSS\MessageLogging\PROTOCOL\CloneACar_NAME_PROTOCOL_MESSAGES.txt

                string PathBase = BaseLogFilePath.Substring(0, BaseLogFilePath.LastIndexOf(Path.DirectorySeparatorChar));
                PathBase += Path.DirectorySeparatorChar + "MessageLogging" + Path.DirectorySeparatorChar + ProtocolID + "_Messages";
                Directory.CreateDirectory(PathBase);

                // Assign message log file values now.
                if (LoggerName == "") { MessageLogFile = BaseLogFile.Split('_')[0] + "_" + ProtocolID + ".txt"; }
                else { MessageLogFile = BaseLogFile.Split('_')[0] + "_" + LoggerName + "_" + ProtocolID + ".txt"; }
                MessageLogFilePath = PathBase + Path.DirectorySeparatorChar + MessageLogFile;
            }

            // Write file heading to our desired file. 
            // Since the LogFilePath var is get/set, it checks to see if this logger object is a message logger or not.
            // If it is, we get the last known log file object for the specified protocol. If not, we just get main log file.

            Version VersionValue = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            DateTime BuildDate = new DateTime(2000, 1, 1)
                .AddDays(VersionValue.Build).AddSeconds(VersionValue.Revision * 2);
            string DisplayVersion = $"{VersionValue} ({BuildDate})";

            File.WriteAllText(LogFilePath, $"CloneACar Version {DisplayVersion} - LOGGING ON - " + LogFileName + "\n");
            File.AppendAllText(LogFilePath, "THIS PROGRAM WAS CREATED BY ZACHARY WALSH (C 2021)\n");
            if (IsMessageLogger) { File.AppendAllText(LogFilePath, $"LOGGING {ProtocolID} MESSAGES FOR {LoggerName} PROCESS NOW\n"); }
            File.AppendAllText(LogFilePath, "------------------------------------------------------------------------------------\n");
        }

        /// <summary>
        /// Removes all previous log files and dirs from the C:\DrewTech\Logs directory.
        /// </summary>
        public void EraseAllHistory(string DirItem)
        {
            // If not debug mode return out.
            if (!Debugger.IsAttached) { return; }

            DirectoryInfo LogInfo = new DirectoryInfo(DirItem);
            foreach (FileInfo FileItem in LogInfo.GetFiles())
            {
                if (FileItem.Name == BaseLogFile) { continue; }
                if (FileItem.Name == MessageLogFile) { continue; }

                try { FileItem.Delete(); }
                catch { }
            }

            foreach (DirectoryInfo Dir in LogInfo.GetDirectories())
            {
                if (Dir.Name == "CloneACar_JSON_Messages") { continue; }

                try { Dir.Delete(true); }
                catch { }
            }
        }
        


        /// <summary>
        /// Writes to log file.
        /// </summary>
        /// <param name="LogString">Value to write to file.</param>
        /// <param name="TypeOfLog">LogType for TYPE OF LOG value.</param>
        /// <returns>True if good write. False if not.</returns>
        public bool WriteLog(string LogString, TextLogTypes.LogItemType TypeOfLog = TextLogTypes.LogItemType.DEBUG)
        {
            IsMessageLog = false;

            try
            {
                var FileToWrite = new FileStream(LogFilePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                var FileStreamWriter = new StreamWriter((Stream)FileToWrite);

                FileStreamWriter.WriteLine(
                    $"[{DateTime.Now:T}] " +
                    $"::: [{NameOfCallingClass()}] " +
                    $"::: [{PadBothEndsOfString(TypeOfLog.ToString(), 5)}] " +
                    $"::: {LogString}"
                );

                FileStreamWriter.Close();
                FileToWrite.Close();
                return true;
            }
            catch { return false; }
        }


        /// <summary>
        /// Writes a PTMessage to the clone log file of the current protocol.
        /// </summary>
        /// <param name="PTMessage">PTMessage to Write to log file.</param>
        /// <param name="MessageType">Type of PTMessage.</param>
        /// <returns>True if written ok, false if not.</returns>
        public bool WriteMessageLog(PassThruMsg PTMessage, MessageLogTypes.MessageTypes MessageType = MessageLogTypes.MessageTypes.PT_MESSG)
        {
            IsMessageLog = true;
            return WriteMessageLog(ConvertDataToString(PTMessage.data), MessageType);
        }
        /// <summary>
        /// Writes a PTMessage to the clone log file of the current protocol.
        /// </summary>
        /// <param name="PTMessages">PTMessage Array for all messages read or sent.</param>
        /// <param name="MessageType">Type of PTMessage.</param>
        /// <returns>True if written ok, false if not.</returns>
        public bool WriteMessageLog(PassThruMsg[] PTMessages, MessageLogTypes.MessageTypes MessageType = MessageLogTypes.MessageTypes.PT_MESSG)
        {
            IsMessageLog = true;

            var MessagesToString = PTMessages.Select(PTMessage => ConvertDataToString(PTMessage.data)).ToList();
            return WriteMessageLog(MessagesToString, MessageType);
        }
        /// <summary>
        /// Writes a byte array to the log file as a PTMessage.
        /// </summary>
        /// <param name="MessageBytes">Bytes to write.</param>
        /// <param name="MessageType">Type of PTMessage.</param>
        /// <returns>True if written ok, false if not.</returns>
        public bool WriteMessageLog(byte[] MessageBytes, MessageLogTypes.MessageTypes MessageType = MessageLogTypes.MessageTypes.PT_MESSG)
        {
            IsMessageLog = true;
            return WriteMessageLog(ConvertDataToString(MessageBytes), MessageType);
        }
        /// <summary>
        /// Writes a PTMessage to the clone log file of the current protocol.
        /// </summary>
        /// <param name="MessageString">String of data to Write to log file.</param>
        /// <param name="MessageType">Type of PTMessage.</param>
        /// <returns>True if written ok, false if not.</returns>
        public bool WriteMessageLog(string MessageString, MessageLogTypes.MessageTypes MessageType = MessageLogTypes.MessageTypes.PT_MESSG)
        {
            IsMessageLog = true;
            return WriteMessageLog(new List<string> { MessageString }, MessageType);
        }
        /// <summary>
        /// Writes a List of PTMessages to the clone log file of the current protocol.
        /// </summary>
        /// <param name="MessagesString">Strings of data to Write to log file.</param>
        /// <param name="MessageType">Type of PTMessage.</param>
        /// <returns>True if written ok, false if not.</returns>
        public bool WriteMessageLog(List<string> MessagesString, MessageLogTypes.MessageTypes MessageType = MessageLogTypes.MessageTypes.PT_MESSG)
        {
            IsMessageLog = true;

            try
            {
                var FileToWrite = new FileStream(LogFilePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                var FileStreamWriter = new StreamWriter((Stream)FileToWrite);

                int Counter = 0;
                foreach (var PTMessage in MessagesString)
                {
                    string NewMessage = "";
                    if (MessageType == MessageLogTypes.MessageTypes.PT_WRITE) { NewMessage =  "--> "; }
                    else if (MessageType == MessageLogTypes.MessageTypes.PT_READS) { NewMessage += "<-- "; }
                    else { NewMessage += "::: "; }

                    NewMessage += PTMessage;

                    FileStreamWriter.WriteLine(
                        // $"[{DateTime.Now:T}] ::: " +
                        $"[{PadBothEndsOfString(MessageType.ToString(), 5)}]" +
                        $" :::{PadBothEndsOfString("MESSAGE [" + Counter.ToString("D3") + "]", 15)}" +
                        $"{NewMessage}"
                    );

                    Counter++;
                }

                FileStreamWriter.Close();
                FileToWrite.Close();
                return true;
            }
            catch { return false; }
        }


        /// <summary>
        /// Logs an exceptions information to the debug log.
        /// </summary>
        /// <param name="ExToLog">Exception Item to log into logs.</param>
        public void WriteErrorLog(Exception ExToLog)
        {
            IsMessageLog = false;

            string Message = ExToLog.Message;
            string[] ErrorStackTrace = ExToLog.StackTrace.Split('\n');

            WriteLog(Message, TextLogTypes.LogItemType.ERROR);
            foreach (var StringItem in ErrorStackTrace) { WriteLog(StringItem.TrimEnd(), TextLogTypes.LogItemType.ERROR); }
        }


        /// <summary>
        /// Gets name of calling Method and uses it to append into the debug log file lines.
        /// </summary>
        /// <returns></returns>
        private string NameOfCallingClass()
        {
            string fullName;
            Type declaringType;
            int skipFrames = 2;
            do
            {
                MethodBase method = new StackFrame(skipFrames, false).GetMethod();
                declaringType = method.DeclaringType;
                if (declaringType == null) { return method.Name; }

                skipFrames++;
                fullName = declaringType.FullName;
            }

            while (declaringType.Module.Name.Equals("mscorlib.dll", StringComparison.OrdinalIgnoreCase));
            var fullNameSplit = fullName.Split('.');
            fullName = fullNameSplit[fullNameSplit.Length - 1];

            return fullName;
        }
        /// <summary>
        /// Padds both ends of a stirng passed in.
        /// </summary>
        /// <param name="source">String to pad</param>
        /// <param name="length">Desired length of output string.</param>
        /// <returns>Centered pad string.</returns>
        private string PadBothEndsOfString(string source, int length)
        {
            int spaces = length - source.Length;
            int padLeft = spaces / 2 + source.Length;
            return source.PadLeft(padLeft).PadRight(length);

        }
    }
}
