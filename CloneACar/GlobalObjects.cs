using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CloneACar.J2534Consumer.VehicleCloning;
using CloneACar.J2534Consumer.VehicleInit;
using CloneACar.LoggingHelpers;
using CloneACar.Models;

namespace CloneACar
{
    public class GlobalObjects
    {
        public static Logger AppLogger;         // App logger object.
        public static string VersionNumber;     // Version of this app.

        // Class for any and all V0404 related object creation and use.
        public static class Version0404Objects
        {
            public static GetAllDLLs_0404 V0404DLLs;            // Init for V0404 DLL objects. Contains model object for all DLLs found.
            public static GetAllDevices_0404 V0404Devices;      // All Devices found for the setup DLL Value.

            public static List<Version0404_DLLAndDevice> PairedDLLsAndDevices;  // List of all usable Hardware.
            public static Version0404_DLLAndDevice SelectedHW;                  // Final picked HW items.  Call SelectedHW.GetDeviceValues to pull info from the J2534Device object.
        }

        public static string VIN_NUMBER;                                       // VIN of the attached car.
        public static AutoIDController AutoID = new AutoIDController();        // Used to autoid a connected vehicle if wanted.
        public static VehicleCloningController VehicleCloner;                  // Used for cloning vehicles.

        public GlobalObjects()
        {
            // Get Version Number.
            VersionNumber = GetVersionInfo();

            // Init the logger.
            AppLogger = new Logger();
            AppLogger.WriteLog("LOGGER OBJECT WAS SETUP OK!");

            // Init our 0404 Items
            SetupV0404DLLsAndDevices();
        }

        private static string GetVersionInfo()
        {
            Version VersionValue = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            DateTime BuildDate = new DateTime(2000, 1, 1)
                .AddDays(VersionValue.Build).AddSeconds(VersionValue.Revision * 2);

            string DisplayVersion = $"{VersionValue} ({BuildDate})";
            return DisplayVersion;
        }

        private static void SetupV0404DLLsAndDevices()
        {
            AppLogger.WriteLog("SETTING UP DLL OBJECT NOW...");
            Version0404Objects.V0404DLLs = new GetAllDLLs_0404();

            AppLogger.WriteLog("SETTING UP DEVICES OBJECT NOW");
            Version0404Objects.V0404Devices = new GetAllDevices_0404();
        }

        public static string ConvertDataToString(byte[] DataToConvert)
        {
            StringBuilder HexString = new StringBuilder(DataToConvert.Length * 2);
            foreach (byte ByteItem in DataToConvert)
            {
                HexString.Append("0x");
                HexString.AppendFormat("{0:x2} ".ToUpper(), ByteItem);
            }

            return  HexString.ToString();
        }
    }
}
