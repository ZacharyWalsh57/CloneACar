using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

// Globals use
using static CloneACar.GlobalObjects;

namespace CloneACar.LoggingHelpers
{
    public class Logger
    {
        private string LogBase = @"C:\DrewTech\Logs\";   // Base output path.

        public string LogFileName;                      // Name of log without path.
        public string LogFilePath;                      // Name of log with path.

        public Logger()
        {
            // Get DateTimeNow.
            var strTime = DateTime.Now.ToString("s")
                .Replace("T", "_")
                .Replace("-", "")
                .Replace(":", "");

            LogFileName = "CloneACar_" + strTime + ".txt";
            LogFilePath = LogBase + LogFileName;

            Directory.CreateDirectory(LogBase);

            File.WriteAllText(LogFilePath, $"CloneACar Version {VersionNumber} - LOGGING ON - " + LogFileName + "\n");
            File.AppendAllText(LogFilePath, "THIS PROGRAM WAS CREATED BY ZACHARY WALSH (C 2021)\n");
            File.AppendAllText(LogFilePath, "------------------------------------------------------------------------------------\n");
        }


        /// <summary>
        /// Writes to log file.
        /// </summary>
        /// <param name="LogString">Value to write to file.</param>
        /// <param name="TypeOfLog">LogType for TYPE OF LOG value.</param>
        /// <returns>True if good write. False if not.</returns>
        public bool WriteLog(string LogString, LogTypes.LogItemType TypeOfLog = LogTypes.LogItemType.DEBUG)
        {
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
        /// Logs an exceptions information to the debug log.
        /// </summary>
        /// <param name="ExToLog">Exception Item to log into logs.</param>
        public void WriteErrorLog(Exception ExToLog)
        {
            string Message = ExToLog.Message;
            string[] ErrorStackTrace = ExToLog.StackTrace.Split('\n');

            WriteLog(Message, LogTypes.LogItemType.ERROR);
            foreach (var StringItem in ErrorStackTrace) { WriteLog(StringItem.TrimEnd(), LogTypes.LogItemType.ERROR); }
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
