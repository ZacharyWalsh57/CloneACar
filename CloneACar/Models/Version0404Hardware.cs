using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloneACar.J2534Consumer.VehicleInit;

namespace CloneACar.Models
{
    public class V0404Hardware
    {
        public GetAllDLLs_0404 V0404DLLs;            // Init for V0404 DLL objects. Contains model object for all DLLs found.
        public GetAllDevices_0404 V0404Devices;      // All Devices found for the setup DLL Value.

        public List<Version0404_DLLAndDevice> PairedDLLsAndDevices;  // List of all usable Hardware.
        public Version0404_DLLAndDevice SelectedHW;                  // Final picked HW items.  Call SelectedHW.GetDeviceValues to pull info from the J2534Device object.
    }
}
