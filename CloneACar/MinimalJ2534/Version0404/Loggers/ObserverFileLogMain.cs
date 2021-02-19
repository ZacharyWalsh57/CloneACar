using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minimal_J2534_0404
{
    // class to log LogMain entries to file
    // turn log on and off by setting MiscParams.LogEnabled
    class ObserverFileLogMain : ILogObserver
    {
        StreamWriter sw;
        string MainLogDir = @"C:\DrewTech\Logs\GDPRemoter\";

        public ObserverFileLogMain()
        {
            CreateDirIfNeeded(MainLogDir);

            string path = MainLogDir + GenerateFileName();
            sw = new StreamWriter(path); 
        }

        private void CreateDirIfNeeded(string dir)
        {
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }

        public void Update()
        {
            string s = LogMain.GetLastEntry();

            sw.WriteLine(s);
            sw.Flush();
        }

        private string GenerateFileName()
        {
            return "GDP_REMOTER_0404" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".txt";
        }
    }
}
