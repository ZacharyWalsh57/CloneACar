﻿using System.Collections.Generic;

namespace Minimal_J2534_0500
{
    /// <summary>
    /// list of installed dlls that satisfy conditions to be a dut for this test program
    /// </summary>
    internal class Device1InstalledDlls
    {
        public static List<J2534Dll> DllList = new List<J2534Dll>();

        public static void Init()
        {
            DllList.Clear();
            foreach (J2534Dll jdll in J2534InstalledDlls.DllList)
            {
                if (SatisfiesDutConditions(jdll))
                {
                    DllList.Add(jdll);
                }
            }
            DllList.Sort();
        }

        /// <summary>
        /// to be a dut, needs to support 1 of the protocols
        /// </summary>
        private static bool SatisfiesDutConditions(J2534Dll jdll)
        {
            /*
            if (jdll.Name.Contains("MongoosePro Honda"))
            {
                return true;
            }
            else if (jdll.Name.Contains("MongoosePro ISO2"))
            {
                return true;
            }
            else if (jdll.Name.Contains("MongoosePro MFC2"))
            {
                return true;
            }
            else if (jdll.Name.Contains("MongoosePro VW"))
            {
                return true;
            }
            
            return false;
            */
            return true;
        }

        // get serials of plugged in units for a dll
        public static List<J2534PresentDevice> GetPresentDevices(J2534Dll dll)
        {
            uint nDevices;

            List<J2534PresentDevice> tempList = new List<J2534PresentDevice>();

            J2534Device d = new J2534Device(dll);
            d.Japi.PassThruScanForDevices(out nDevices);
            for (int i = 0; i < nDevices; i++)
            {
                SDEVICE sDevice;
                d.Japi.PassThruGetNextDevice(out sDevice);
                tempList.Add(new J2534PresentDevice(sDevice, dll) );
            }

            return tempList;
        }

    }
}