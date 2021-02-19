using System;
using System.Collections.Generic;
using System.Linq;
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
    public class GetAllDLLs_0404
    { 
        public List<J2534Dll> DLLs;                        // List of all V0404 DLLs on the machine.
        public List<Verison0404_DLLModel> DLLModels;        // List of all V0404 DLLs AS A MODEL OBJECT TO UI!

        public int DLLIndex;                                // Index of the picked DLL.
        public J2534Dll SelectedDLL;                        // Currently Picked DLL object.
        public Verison0404_DLLModel SelectedDLLModel;       // DLL Selected model.  

        public GetAllDLLs_0404()
        {
            // Log whats up.
            AppLogger.WriteLog("SETTING UP DLLS FOR V0404 NOW");

            // Init constants if needed.
            DLLModels = new List<Verison0404_DLLModel>();

            // Init the DLL List for all installed DLL Objects and save them.
            Device1InstalledDlls.Init();
            DLLs = Device1InstalledDlls.DllList;
            AppLogger.WriteLog("PERFORMED AN INIT OF THE DLL LIST OK!");

            // Append dlls into the DLLModels now.
            if (UpdateDLLModels(out var DLLInitStatus)) return;
            AppLogger.WriteLog($"FAILED TO INIT DLLS LIST. ERROR: {DLLInitStatus}", LogTypes.LogItemType.ERROR);
        }

        public bool UpdateDLLModels(out VehicleInitEnums.DLLInitStatus InitStatus)
        {
            // If no DLLS found return false.
            if (DLLs.Count == 0)
            {
                AppLogger.WriteLog("NO DLLS FOUND ON THIS MACHINE FOR THE V0404 API", LogTypes.LogItemType.ERROR);
                InitStatus = VehicleInitEnums.DLLInitStatus.NO_DLLS_FOUND;

                return false;
            }

            // Store our init status here.
            InitStatus = VehicleInitEnums.DLLInitStatus.ONLY_ONE_DLL_FOUND;
            if (DLLs.Count > 1) { InitStatus = VehicleInitEnums.DLLInitStatus.MULTIPLE_DLLS_FOUND; }
            AppLogger.WriteLog($"FOUND A TOTAL OF {DLLs.Count} DLLS FOR V0404");

            // Make the DLL objects now.
            foreach (var DLL in DLLs)
            {
                DLLModels.Add(new Verison0404_DLLModel
                {
                    DLLName = DLL.LongName,
                    Version = "V0404",
                    LongName = DLL.LongName,
                    Vendor = DLL.Vendor,
                    FunctionLib = DLL.FunctionLibrary,
                    ProcCount = DLL.SupportedProtocols.Count
                });

                AppLogger.WriteLog($"ADDED DLL NAMED: {DLL.LongName} TO THE LIST OF ALL DLLS");
            }

            DLLIndex = 0;
            SelectedDLL = DLLs[0];
            SelectedDLLModel = DLLModels[0];

            AppLogger.WriteLog("DLL INDEX: " + DLLIndex, LogTypes.LogItemType.EXEOK);
            AppLogger.WriteLog("SELECTED DLL: " + SelectedDLL.LongName, LogTypes.LogItemType.EXEOK);
            AppLogger.WriteLog("DLL IS VERSION O404 AND EXISTS. THIS IS OUR DEFAULT FOR NOW", LogTypes.LogItemType.EXEOK);

            AppLogger.WriteLog("ADDED ALL V0404 DLLS TO THE DLL MODEL OBJECTS OK!", LogTypes.LogItemType.EXEOK);
            return true;
        }
    }
}
