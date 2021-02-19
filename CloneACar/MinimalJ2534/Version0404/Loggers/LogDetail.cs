using System;
using System.Collections.Generic;
using System.Linq;


namespace Minimal_J2534_0404
{
    internal static class LogDetail
    {
        private static List<ILogObserver> logObs = new List<ILogObserver>();
        private static List<SingleLog> logData = new List<SingleLog>();                     // array of indexed string log entries
        private static int currentIndex = -1;
        private static int maxEntries = 5000;
        private static DateTime startDateTime;

        public static void RemoveObserver(ILogObserver o)
        {
            logObs.Remove(o);
        }

        public static void RegisterObserver(ILogObserver o)
        {
            logObs.Add(o);
        }

        public static void NotifyObservers()
        {
            foreach (ILogObserver o in logObs)
            {
                o.Update();
            }
        }

        public static List<SingleLog> GetAllEntries()
        {
            // make a copy so that the log can't be changed
            List<SingleLog> logCopy = new List<SingleLog>(logData);
            return logCopy;
        }

        public static string GetLastEntry()
        {
            if (logData.Count > 0)
            {
                return logData.Last<SingleLog>().Entry;
            }
            else
            {
                return MiscConstants.ObserverSubjectEmpty;
            }
        }

        public static void WriteEntry(string s, bool timestamped = false, bool tabbed = false)
        {
            if (timestamped)
            {
                s = GetDateTime() + " " + s;
            }

            if (tabbed)
            {
                s = "  " + s;
            }

            logData.Add(new SingleLog { Index = ++currentIndex, Entry = s });
            NotifyObservers();
            if (logData.Count > maxEntries)
            {
                logData.RemoveRange(0, logData.Count - maxEntries);
            }
        }

        public static void WriteCoreEntry(string s)
        {
            WriteEntry(s, false, true);
        }

        public static void WriteCoreNotSupported(string device)
        {
            string s = "Not supported on " + device + ", skipping";
            WriteEntry(s, false, true);
        }

        public static void WriteSubPass()
        {
            WriteEntry("PASS", false, true);
        }

        private static string GetDateTime()
        {
            TimeSpan span = DateTime.Now.Subtract(startDateTime);

            return "(" + Math.Floor(span.TotalHours) + ":" + span.Minutes + ":" + span.Seconds + ")";
            //return "(" + DateTime.Now.ToString("HH:mm:ss") + ")";
        }

        public static void SubtestStart(string s)
        {
            s = " Subtest: " + s;

            logData.Add(new SingleLog { Index = ++currentIndex, Entry = s });
            NotifyObservers();
        }

        public static void NewLine()
        {
            logData.Add(new SingleLog { Index = ++currentIndex, Entry = "" });
            NotifyObservers();
        }

        internal static void WriteStackDump(Exception ex)
        {
            string divider = "********************";

            logData.Add(new SingleLog { Index = ++currentIndex, Entry = divider });
            NotifyObservers();

            logData.Add(new SingleLog { Index = ++currentIndex, Entry = ex.ToString() });
            NotifyObservers();

            logData.Add(new SingleLog { Index = ++currentIndex, Entry = divider });
            NotifyObservers();
        }

        public static void Clear()
        {
            logData.Clear();
        }

        public static void SetStartTime()
        {
            startDateTime = DateTime.Now;
        }

        public static void WriteHeader(string theOperator)
        {
            string s = "=====================";
            logData.Add(new SingleLog { Index = ++currentIndex, Entry = s });
            NotifyObservers();

            s = "Detail Log";
            logData.Add(new SingleLog { Index = ++currentIndex, Entry = s });
            NotifyObservers();

            s = DateTime.Now.ToLongDateString();
            logData.Add(new SingleLog { Index = ++currentIndex, Entry = s });
            NotifyObservers();

            s = DateTime.Now.ToLongTimeString();
            logData.Add(new SingleLog { Index = ++currentIndex, Entry = s });
            NotifyObservers();

            s = "Tested by " + theOperator;
            logData.Add(new SingleLog { Index = ++currentIndex, Entry = s });
            NotifyObservers();

            s = "=====================";
            logData.Add(new SingleLog { Index = ++currentIndex, Entry = s });
            NotifyObservers();
        }
    }
}