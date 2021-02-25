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
        public static Logger AppLogger;             // App logger object.

        public static string VIN_NUMBER;                                       // VIN of the attached car.
        public static V0404Hardware HardwareConfig;                            // Hardware selected for this run.
        public static AutoIDController AutoID = new AutoIDController();        // Used to autoid a connected vehicle if wanted.
        public static VehicleCloningController VehicleCloner;                  // Used for cloning vehicles.

        public static List<ModuleCommunicationResults> FinalCloneResults;      // Holds all the module comms for anything that worked in clone.

        public static void SetupGlobalObjects()
        {
            // Init the logger.
            AppLogger = new Logger();
            AppLogger.WriteLog("LOGGER OBJECT WAS SETUP OK!");

            // Init Misc Items
            FinalCloneResults = new List<ModuleCommunicationResults>();

            // Init our 0404 Items - This is done via MainWindow Now.
            // SetupV0404DLLsAndDevices();
        }

        /// <summary>
        /// Setup new V0404 Device and DLL object.
        /// This picks the first found DLL and device and stores them in the Version0404Objects class.
        /// </summary>
        public static void SetupV0404DLLsAndDevices()
        {
            // Init the HW object. This holds all DLLs, All Devices, 
            // along with the selected DLL and Device. This is accessed 
            // by HardwareConfig.SelectedHW
            HardwareConfig = new V0404Hardware();

            // Init the DLL objects
            AppLogger.WriteLog("SETTING UP DLL OBJECT NOW...");
            HardwareConfig.V0404DLLs = new GetAllDLLs_0404();

            // Init the Device objects.
            AppLogger.WriteLog("SETTING UP DEVICES OBJECT NOW");
            HardwareConfig.V0404Devices = new GetAllDevices_0404();
        }
    }
}
