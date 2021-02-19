using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloneACar.Models
{
    public class Verison0404_DLLModel
    {
        public string DLLName { get; set; }         // Name of DLL in use
        public string Version { get; set; }         // Version (0404 vs 0500)        
        public string LongName { get; set; }        // Long name of the device/lib
        public string Vendor { get; set; }          // Vendor of the device
        public string FunctionLib { get; set; }     // Location of the DLL being called.
        public int ProcCount { get; set; }          // How many protocols the device can support.
    }
}
