using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CloneACar.LoggingHelpers;
using Minimal_J2534_0404;

namespace CloneACar.Models
{
    public class Version0404_DLLAndDevice
    {
        public J2534Dll DLL { get; set; }
        private J2534Device Device { get; set; }

        public Verison0404_DLLModel DLLModel { get; set; }
        public Version0404_DeviceModel DeviceModel { get; set; }


        public T GetDeviceValues<T>()
        {
            return (T)this.GetType()
                .GetProperty("Device", BindingFlags.Instance | BindingFlags.NonPublic)
                .GetValue(this);
        }
        public void SetDeviceValues<T>(T value)
        {
            this.GetType()
                .GetProperty("Device", BindingFlags.Instance | BindingFlags.NonPublic)
                .SetValue(this, value);
        }

        public void SetupNewDevice(J2534Dll SetDLL = null)
        {
            if (SetDLL != null) { this.DLL = SetDLL; }
            Device = new J2534Device(this.DLL);

            // Pops open the J2534 Device quick to init and closes it back down till we wanna do shit.
            Device.PTOpen(DeviceModel.DeviceName);
            if (!Device.isOpen) { return; }

            // Pull a whole bunch of useful info from the device.
            try
            {
                DeviceModel.DeviceID = Device.deviceId;
                DeviceModel.SerialNumber = Device.ReadSerial();
                DeviceModel.CableSerial = Device.ReadCableSerial();
                DeviceModel.Voltage = (double)(Device.ReadVBatt() / 1000.0);
            }
            catch (J2534Exception Ex)
            {
                GlobalObjects.AppLogger.WriteLog($"ERROR WHILE PULLING ONE OR MORE VALUES! --> {Ex.Message}", LogTypes.LogItemType.ERROR);
                GlobalObjects.AppLogger.WriteErrorLog(Ex);
            }

            // Close it down.
            Device.PTClose();
        }
    }
}
