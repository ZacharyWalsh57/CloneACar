using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CloneACar.LoggingHelpers;

// Using call for the Minimal 0404 DLL
using Minimal_J2534_0404;
using CloneACar.Models;

// Globals
using static CloneACar.GlobalObjects;
using static CloneACar.GlobalObjects.Version0404Objects;

namespace CloneACar.J2534Consumer.VehicleInit
{
    public class GetAllDevices_0404
    {
        public Version0404_DeviceModel SelectedDevice;              // Selected V0404 Device object.
        public List<Version0404_DeviceModel> V0404DeviceModels;     // All V0404 devices for the specified DLL AS MODELS!

        // API Cast Device types. List of Devices and a single device we picked.
        #region  DEVICES FROM THE API. PRIVATE NORMALLY SO SOME MAGIC IS NEEDED
        private List<J2534Device> V0404Devices { get; set; }        // List of all installed V0404 Devices
        private J2534Device DeviceSet { get; set; }                 // V0404 Device object from the API.

        // HELPER METHODS TO RETURN THIS SHIT BACK OK.
        public static T GetPrivateProperty<T>(object obj, string propertyName)
        {
            return (T)obj.GetType()
                .GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic)
                .GetValue(obj);
        }
        public static void SetPrivateProperty<T>(object obj, string propertyName, T value)
        {
            obj.GetType()
                .GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic)
                .SetValue(obj, value);
        }
        #endregion

        public GetAllDevices_0404()
        {
            // Log it up.
            AppLogger.WriteLog("SETTING UP DEVICE INSTANCES NOW");

            // Just use our global object for the DLL stuff I think...
            if (V0404DLLs == null) { V0404DLLs = new GetAllDLLs_0404(); }
            AppLogger.WriteLog("DLLS WERE IMPORTED TO THIS CLASS OK! GETTING DEVICES NOW", LogTypes.LogItemType.EXEOK);

            // Find Devices here.
            FindAllDevices();
        }

        public void FindAllDevices()
        {
            PairedDLLsAndDevices = new List<Version0404_DLLAndDevice>();
            var NextDevices = new List<Version0404_DeviceModel>();

            // Loop all the DLLs and append them to the CombinedV0404 DLLs and Devices.
            foreach (var DLLEntry in V0404DLLs.DLLs)
            {
                AppLogger.WriteLog($"CHECKING DLL: {DLLEntry.LongName}");
                if (!FindDevicesForDLL(DLLEntry, out NextDevices)) { continue; }

                foreach (var DeviceItem in NextDevices)
                {
                    AppLogger.WriteLog($"APPENDING DEVICE {DeviceItem.DeviceName} NOW");
                    var PairedHW = new Version0404_DLLAndDevice
                    {
                        DLL = DLLEntry,
                        DLLModel = V0404DLLs.DLLModels[V0404DLLs.DLLs.IndexOf(DLLEntry)],
                        DeviceModel = DeviceItem,
                    };

                    PairedHW.SetupNewDevice(DLLEntry);
                    PairedDLLsAndDevices.Add(PairedHW);
                }
            }

            if (PairedDLLsAndDevices.Count == 0)
            {
                AppLogger.WriteLog("FAILED TO FIND ANY USABLE DLLS OR DEVICES. CLOSING NOW");
                Environment.Exit(0);
            }

            SelectedHW = PairedDLLsAndDevices[0];
            var DeviceMade = SelectedHW.GetDeviceValues<J2534Device>();

            AppLogger.WriteLog("-------------------------------------------------------------------------------------------------------------------------");
            AppLogger.WriteLog($"\n\nDEVICE DETAILS\n   \\__ NAME: {DeviceMade.name}\n   \\__ ID: {DeviceMade.deviceId}\n   " + 
                               $"\\__ CONNECTED: {DeviceMade.isConnected}\n   \\__ SN: {SelectedHW.DeviceModel.SerialNumber}\n   " +
                               $"\\__ CABLE SN: {SelectedHW.DeviceModel.CableSerial}\n   \\__ VOLTAGE: {SelectedHW.DeviceModel.Voltage}\n");
            
            AppLogger.WriteLog($"\n\nDLL INFO\n   \\__ NAME: {DeviceMade.Jdll.LongName}\n  " +
                               $" \\__ VENDOR: {DeviceMade.Jdll.Vendor}\n   " +
                               $"\\__ SUPPORTED PROCS: {DeviceMade.Jdll.SupportedProtocols.Count}\n", LogTypes.LogItemType.EXEOK);
            AppLogger.WriteLog("-------------------------------------------------------------------------------------------------------------------------");
        }

        public bool FindDevicesForDLL(J2534Dll SelectedDLL, out List<Version0404_DeviceModel> DevicesFound)
        {
            try
            {
                var J2534DevicesFound = J2534InstalledDlls.GetAvailableDevicesForProductType(SelectedDLL);
                if (J2534DevicesFound.Count == 0)
                {
                    DevicesFound = null;
                    AppLogger.WriteLog($"NO DEVICES FOUND FOR DLL {SelectedDLL.LongName} RETURNING NOW");

                    return false;
                }

                DevicesFound = new List<Version0404_DeviceModel>();
                foreach (var SDevice in J2534DevicesFound)
                {
                    AppLogger.WriteLog($"ADDING DEVICE {SDevice.deviceName}");
                    DevicesFound.Add(new Version0404_DeviceModel
                    {
                        DLLName = SelectedDLL.LongName,
                        Version = "V0404",
                        DeviceName = SDevice.deviceName,
                    });
                }

                AppLogger.WriteLog($"FOUND A TOTAL OF {DevicesFound.Count} DEVICES FOR THIS DLL", LogTypes.LogItemType.EXEOK);
                return true;
            }

            catch (Exception ex)
            {
                AppLogger.WriteLog($"FAILED TO SETUP DEVICES FOR DLL {SelectedDLL.LongName}", LogTypes.LogItemType.ERROR);
                AppLogger.WriteErrorLog(ex);

                DevicesFound = null;
                return false;
            }
        }
    }
}
