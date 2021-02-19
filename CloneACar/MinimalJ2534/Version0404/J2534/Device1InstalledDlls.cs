using System.Collections.Generic;

namespace Minimal_J2534_0404
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

        
    }
}