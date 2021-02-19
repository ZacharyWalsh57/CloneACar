using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Using call for the Minimal 0404 DLL
using Minimal_J2534_0404;
using CloneACar.Models;

// Globals
using static CloneACar.GlobalObjects;
using static CloneACar.GlobalObjects.Version0404Objects;

namespace CloneACar.J2534Consumer.VehicleCloning
{
    public class ISO15765_11BIT_Cloning
    {
        private J2534Device Device
        {
            get { return SelectedHW.GetDeviceValues<J2534Device>(); }
            set { SelectedHW.SetDeviceValues(Device); }
        }

        public ISO15765_11BIT_Cloning()
        {
            // Log whats going on.
            AppLogger.WriteLog("SETTING UP FOR 11 BIT CLONING SESSION. SETTING DEVICE OBJECT NOW");

            // Open the device and connect to an 11bit can channel.
            if (!Device.isOpen) { Device.PTOpen(Device.name); }
            if (Device.channels[0] != null)
                if (Device.channels[0].protocol != ProtocolId.ISO15765)
                    Device.PTDisconnect(0);
            if (Device.channels[0] == null)
                Device.PTConnect(0, ProtocolId.ISO15765, 0x00, 500000);
            
            // AutoID Process can now be attempted.
            AppLogger.WriteLog("11 BIT CAN CHANNEL WAS OPENED AND CONNECTED OK. READY TO RUN CLONING PROCESS NOW");
        }

        public void StartClone()
        {

        }
    }
}
