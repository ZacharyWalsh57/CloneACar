using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloneACar.J2534Consumer.VehicleInit;
using CloneACar.Models;
using Minimal_J2534_0404;

namespace CloneACar.J2534Consumer.VehicleCloning
{
    public class VehicleCloningController
    {
        public AutoIDController AutoIDSetup;
        public ProtocolId Protocol;
        public string VIN;

        public List<ModuleCommunicationResults> ModuleCommsList;
        private ISO15765_11BIT_Cloning ISO15765_Cloner;

        public VehicleCloningController(AutoIDController AutoIDResults)
        {
            AutoIDSetup = AutoIDResults;
            Protocol = AutoIDResults.ProtocolFound;
            VIN = AutoIDResults.VIN;

            // Switch for each protocol option in the cloner.
            // ONLY MAKE ONE CLONING OBJECT BASED ON THE PROTOCOL!
            switch (Protocol)
            {
                case ProtocolId.ISO15765:
                    ISO15765_Cloner = new ISO15765_11BIT_Cloning();
                    break;
            }
        }

        /// <summary>
        /// Makes messages to send using cloner.
        /// </summary>
        public void GenerateMessages()
        {
            // Switch for each protocol option in the cloner.
            switch (Protocol)
            {
                case ProtocolId.ISO15765:
                    if (ISO15765_Cloner == null) { ISO15765_Cloner = new ISO15765_11BIT_Cloning(); } 
                    ISO15765_Cloner.GenerateAllMessages();
                    break;
            }
        }
        /// <summary>
        /// Sends messages made from message generation based on protocol type.
        /// </summary>
        public void RunCloner()
        {
            // Switch for each protocol option in the cloner.
            switch (Protocol)
            {
                case ProtocolId.ISO15765:
                    if (ISO15765_Cloner == null) { ISO15765_Cloner = new ISO15765_11BIT_Cloning(); }
                    if (ISO15765_Cloner.DiagCommandMessages == null) { ISO15765_Cloner.GenerateAllMessages(); }
                    
                    if (ISO15765_Cloner.CloneAllModules()) { ModuleCommsList = ISO15765_Cloner.ModuleCommResults; }
                    break; 
            }
        }
    }
}
