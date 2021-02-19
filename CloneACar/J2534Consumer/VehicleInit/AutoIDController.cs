using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloneACar.J2534Consumer.VehicleCloning;
using CloneACar.J2534Consumer.VehicleInit.AutoIDMethods;
using CloneACar.LoggingHelpers;

// Using call for the Minimal 0404 DLL
using Minimal_J2534_0404;
using CloneACar.Models;

// Globals
using static CloneACar.GlobalObjects;
using static CloneACar.GlobalObjects.Version0404Objects;

namespace CloneACar.J2534Consumer.VehicleInit
{
    public class AutoIDController
    {
        public string VIN;
        public ProtocolId ProtocolFound;

        /// <summary>
        /// Does auto ID for ALL protocols.
        /// </summary>
        /// <returns></returns>
        public bool TryAutoID()
        {
            if (TryISO15765()) { return true; }

            AppLogger.WriteLog("UNABLE TO PULL A VIN NUMBER FROM THE VEHICLE. THIS ISNT GOOD", LogTypes.LogItemType.ERROR);
            return false;
        }

        /// <summary>
        /// Does AutoID for only ISO15765 11 BIT can.
        /// </summary>
        /// <returns></returns>
        public bool TryISO15765()
        {
            var ElevenBitCAN = new AutoID_CAN11BIT();
            if (ElevenBitCAN.ReadVIN(out VIN))
            {
                // Set the protocol type here.
                ProtocolFound = ProtocolId.ISO15765;
                VIN_NUMBER = VIN;

                AppLogger.WriteLog("GOT A GOOD VIN NUMBER FROM THE 11 BIT CAN CONTROLLER", LogTypes.LogItemType.EXEOK);
                AppLogger.WriteLog($"VIN NUMBER IS RECORDED AS: {VIN}", LogTypes.LogItemType.EXEOK);

                VehicleCloner = new VehicleCloningController(this);
                return true;
            }

            return false;
        }
    }
}
