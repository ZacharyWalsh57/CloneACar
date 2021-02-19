using Microsoft.Win32;
using System;
using System.Collections.Generic;

namespace Minimal_J2534_0404
{
    /// <summary>
    /// find all installed devices in registry, and which protocols are supported
    /// </summary>
    internal class J2534InstalledDlls
    {
        private const string PASSTHRU_REGISTRY_PATH = "Software\\PassThruSupport.04.04";
        private const string PASSTHRU_REGISTRY_PATH_6432 = "Software\\Wow6432Node\\PassThruSupport.04.04";

        public static List<J2534Dll> DllList = new List<J2534Dll>();

        private static RegistryKey myKey;

        static J2534InstalledDlls()
        {
            CoreDetermineRegistryPath();
            CoreFindDlls();
            CoreGetRegistryDetails();
            CoreGetRegistryProtocols();
        }

        private static void CoreDetermineRegistryPath()
        {
            myKey = Registry.LocalMachine.OpenSubKey(PASSTHRU_REGISTRY_PATH, false);
            if (myKey == null)
            {
                myKey = Registry.LocalMachine.OpenSubKey(PASSTHRU_REGISTRY_PATH_6432, false);
            }
        }

        private static void CoreFindDlls()
        {
            string[] dlls = myKey.GetSubKeyNames();

            foreach (string dll in dlls)
            {
                DllList.Add(new J2534Dll(dll));
            }
        }

        private static void CoreGetRegistryDetails()
        {
            foreach (J2534Dll jdll in DllList)
            {
                RegistryKey deviceKey = myKey.OpenSubKey(jdll.LongName);

                jdll.Vendor = (string)deviceKey.GetValue("Vendor", "");
                jdll.Name = (string)deviceKey.GetValue("Name", "");
                jdll.FunctionLibrary = (string)deviceKey.GetValue("FunctionLibrary", "");
            }
        }

        private static void CoreGetRegistryProtocols()
        {
            foreach (J2534Dll jdll in DllList)
            {
                RegistryKey deviceKey = myKey.OpenSubKey(jdll.LongName);

                var protocols = Enum.GetValues(typeof(ProtocolId));
                foreach (ProtocolId p in protocols)
                {
                    if (((int)deviceKey.GetValue(p.ToString(), 0)) == 1)
                    {
                        jdll.SupportedProtocols.Add(p);
                    }
                }
            }
        }
        public static List<SDevice> GetAvailableDevicesForProductType(J2534Dll dll)
        {
            List<SDevice> devicesAvailable = new List<SDevice>();

            J2534Device dev = new J2534Device(dll);

            string name = "";
            string address;
            uint version;

            try
            {
                dev.Japi.DTInitGetNextCarDAQ();

                while (name != null)
                {
                    dev.Japi.DTGetNextCarDAQ(out name, out version, out address);
                    if (name != null)
                        devicesAvailable.Add(new SDevice
                        {
                            deviceName = name,
                            deviceAvailable = 1,
                            deviceConnectMedia = 1,
                            deviceConnectSpeed = 1,
                            deviceDLLFWStatus = 1,
                            deviceSignalQuality = 1,
                            deviceSignalStrength = 1
                        });
                }
            }
            catch { }
            return devicesAvailable;
        }
    }

    public class J2534Dll : IComparable
    {
        public string Name { get; set; }
        public string LongName { get; set; }
        public string FunctionLibrary { get; set; }
        public List<ProtocolId> SupportedProtocols = new List<ProtocolId>();
        public string Vendor { get; set; }

        public J2534Dll(string name)
        {
            LongName = name;
        }

        public bool IsProtocolSupported(ProtocolId p)
        {
            foreach (ProtocolId sp in SupportedProtocols)
            {
                if (sp == p)
                {
                    return true;
                }
            }
            return false;
        }

        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// implement this so that we can sort the combo boxes (lists of J2534Dll)
        /// </summary>
        public int CompareTo(object obj)
        {
            J2534Dll x = (J2534Dll)obj;
            return String.Compare(this.Name, x.Name);
        }

        
    }
}