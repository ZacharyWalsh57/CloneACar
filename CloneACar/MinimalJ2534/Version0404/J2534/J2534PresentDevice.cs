using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Minimal_J2534_0404
{
    /// <summary>
    /// class to represent a device that is connected. Contains dll information and SDevice information from PassThruGetNextDevice()
    /// </summary>
    class J2534PresentDevice
    {
        public J2534Dll jdll;
        public string connectedDevice;

        public J2534PresentDevice(string name, J2534Dll dll)
        {
            jdll = dll;
            connectedDevice = name;
        }


        public override string ToString()
        {
            return connectedDevice;
        }
    }
}
