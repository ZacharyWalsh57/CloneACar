using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloneACar.J2534Consumer.VehicleInit;
using Minimal_J2534_0404;

namespace CloneACar.J2534Consumer.VehicleCloning
{
    public class VehicleCloningController
    {
        public AutoIDController AutoIDSetup;
        public ProtocolId Protocol;
        public string VIN;

        public VehicleCloningController(AutoIDController AutoIDResults)
        {
            AutoIDSetup = AutoIDResults;
            Protocol = AutoIDResults.ProtocolFound;
            VIN = AutoIDResults.VIN;
        }

        public void Clone15765_11Bit()
        {
           var ISO15765_Cloner = new ISO15765_11BIT_Cloning();
           ISO15765_Cloner.StartClone(); 
        }
    }
}
