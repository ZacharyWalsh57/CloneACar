using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloneACar.Models
{
    public class Version0404_DeviceModel
    {
        // FROM AN SDEVICE MODEL
        public string DeviceName { get; set; }     // Name of the device we picked.
        public string Version { get; set; }        // V0404 or V0500 Identifier.
        public string DLLName { get; set; }        // Name of the DLL used to open the device.

        // FROM AN INITALIZED J2534 DEVICE.
        public uint DeviceID { get; set; }        // Device ID
        public double Voltage { get; set; }        // Voltage from Pin 16
        public int SerialNumber { get; set; }      // SN Of the box.
        public string CableSerial { get; set; }    // SN of Cable for the box.
    }
}
