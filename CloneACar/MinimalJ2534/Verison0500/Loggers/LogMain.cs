using System;
using System.Collections.Generic;
using System.Linq;

namespace Minimal_J2534_0500
{
    internal static class LogMain
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

        public static void WriteEntry(string s, bool timestamped = false)
        {
            if (timestamped)
            {
                s = GetDateTime() + " " + s;
            }

            logData.Add(new SingleLog { Index = ++currentIndex, Entry = s });

            NotifyObservers();
            if (logData.Count > maxEntries)
            {
                logData.RemoveRange(0, logData.Count - maxEntries);
            }
        }

        private static string GetDateTime()
        {
            TimeSpan span = DateTime.Now.Subtract(startDateTime);

            return "(" + Math.Floor(span.TotalHours) + ":" + span.Minutes.ToString("D2") + ":" + span.Seconds.ToString("D2") + ")";
            //return "(" + DateTime.Now.ToString("HH:mm:ss") + ")";
        }

        public static void SubtestStart(string s)
        {
            LogDetail.SubtestStart(s);
            s = GetDateTime() + " Subtest: " + s;

            logData.Add(new SingleLog { Index = ++currentIndex, Entry = s });
            NotifyObservers();
        }

        public static void SubtestEnd()
        {
        }

        /*
        public static void WriteError(J2534Err err, bool timestamped = false)
        {
            if (err == J2534Err.STATUS_NOERROR)
            {
                WriteEntry("ok", timestamped);
                return;
            }

            WriteEntry(String.Format("{0} (0x{1:X})", err.ToString(), (int)err), timestamped);
            Logger.NewLine();
        }
        */

        public static void NewLine()
        {
            logData.Add(new SingleLog { Index = ++currentIndex, Entry = "" });
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

            s = "Main Log";
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

    internal class SingleLog
    {
        public int Index;
        public string Entry;
    }
}