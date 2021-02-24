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

namespace CloneACar.J2534Consumer.VehicleInit
{
    public class GetAllDevices_0404
    {
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
            if (HardwareConfig.V0404DLLs == null) { HardwareConfig.V0404DLLs = new GetAllDLLs_0404(); }
            AppLogger.WriteLog("DLLS WERE IMPORTED TO THIS CLASS OK! GETTING DEVICES NOW", TextLogTypes.LogItemType.EXEOK);

            // Find Devices here.
            FindAllDevices();
        }

        public void FindAllDevices()
        {
            HardwareConfig.PairedDLLsAndDevices = new List<Version0404_DLLAndDevice>();
            var NextDevices = new List<Version0404_DeviceModel>();

            // Loop all the DLLs and append them to the CombinedV0404 DLLs and Devices.
            foreach (var DLLEntry in HardwareConfig.V0404DLLs.DLLs)
            {
                AppLogger.WriteLog($"CHECKING DLL: {DLLEntry.LongName}");
                if (!FindDevicesForDLL(DLLEntry, out NextDevices)) { continue; }

                foreach (var DeviceItem in NextDevices)
                {
                    AppLogger.WriteLog($"APPENDING DEVICE {DeviceItem.DeviceName} NOW");
                    var DLLModel = HardwareConfig.V0404DLLs.DLLModels[HardwareConfig.V0404DLLs.DLLs.IndexOf(DLLEntry)];
                    var PairedHW = new Version0404_DLLAndDevice
                    {
                        DLL = DLLEntry,
                        DLLModel = DLLModel,
                        DeviceModel = DeviceItem,
                    };

                    PairedHW.SetupNewDevice(DLLEntry);
                    HardwareConfig.PairedDLLsAndDevices.Add(PairedHW);
                }
            }

            if (HardwareConfig.PairedDLLsAndDevices.Count == 0)
            {
                AppLogger.WriteLog("FAILED TO FIND ANY USABLE DLLS OR DEVICES. CLOSING NOW");
                Environment.Exit(0);
            }

            HardwareConfig.SelectedHW = HardwareConfig.PairedDLLsAndDevices[0];
            var DeviceMade = HardwareConfig.SelectedHW.GetDeviceValues<J2534Device>();

            var SelectedHW = HardwareConfig.SelectedHW;
            AppLogger.WriteLog("-------------------------------------------------------------------------------------------------------------------------");
            AppLogger.WriteLog($"\n\nDEVICE DETAILS\n   \\__ NAME: {DeviceMade.name}\n   \\__ ID: {DeviceMade.deviceId}\n   " + 
                               $"\\__ CONNECTED: {DeviceMade.isConnected}\n   \\__ SN: {SelectedHW.DeviceModel.SerialNumber}\n   " +
                               $"\\__ CABLE SN: {SelectedHW.DeviceModel.CableSerial}\n   \\__ VOLTAGE: {SelectedHW.DeviceModel.Voltage}\n");
            
            AppLogger.WriteLog($"\n\nDLL INFO\n   \\__ NAME: {DeviceMade.Jdll.LongName}\n  " +
                               $" \\__ VENDOR: {DeviceMade.Jdll.Vendor}\n   " +
                               $"\\__ SUPPORTED PROCS: {DeviceMade.Jdll.SupportedProtocols.Count}\n", TextLogTypes.LogItemType.EXEOK);
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

                AppLogger.WriteLog($"FOUND A TOTAL OF {DevicesFound.Count} DEVICES FOR THIS DLL", TextLogTypes.LogItemType.EXEOK);
                return true;
            }

            catch (Exception ex)
            {
                AppLogger.WriteLog($"FAILED TO SETUP DEVICES FOR DLL {SelectedDLL.LongName}", TextLogTypes.LogItemType.ERROR);
                AppLogger.WriteErrorLog(ex);

                DevicesFound = null;
                return false;
            }
        }
    }
}
