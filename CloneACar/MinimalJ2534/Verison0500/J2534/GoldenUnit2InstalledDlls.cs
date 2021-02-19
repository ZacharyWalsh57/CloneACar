using System.Collections.Generic;

namespace Minimal_J2534_0500
{
    internal class GoldenUnit2InstalledDlls
    {
        public static List<J2534Dll> DllList = new List<J2534Dll>();

        public static void Init()
        {
            DllList.Clear();
            foreach (J2534Dll jdll in J2534InstalledDlls.DllList)
            {
                if (SatisfiesGoldenUnitConditions(jdll))
                {
                    DllList.Add(jdll);
                }
            }
        }

        /// <summary>
        /// to be a golden unit
        /// </summary>
        private static bool SatisfiesGoldenUnitConditions(J2534Dll jdll)
        {
            if (jdll.Name == "CarDAQ-M")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}