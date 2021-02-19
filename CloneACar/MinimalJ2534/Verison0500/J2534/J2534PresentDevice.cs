using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Minimal_J2534_0500
{
    /// <summary>
    /// class to represent a device that is connected. Contains dll information and SDevice information from PassThruGetNextDevice()
    /// </summary>
    class J2534PresentDevice
    {
        public J2534Dll jdll;
        public SDEVICE connectedDevice;

        public J2534PresentDevice(SDEVICE connectedDevice, J2534Dll dll)
        {
            jdll = dll;
            this.connectedDevice = connectedDevice;
        }

        public override string ToString()
        {
            return connectedDevice.DeviceName;
        }
    }
}
