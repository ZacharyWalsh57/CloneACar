using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minimal_J2534_0500
{
    static class J2534DeviceFinder
    {
        /* 
        returns the 1st device from list that is installed (J2534 dll) and plugged in.
        usage:
        List<string> deviceNames = new List<string>();

        deviceNames.Add("CarDAQ-Plus 3");
        deviceNames.Add("CarDAQ-Plus 2");
        deviceNames.Add("GDP3");
        deviceNames.Add("GDP2020"); 
        deviceNames.Add("Remote Vehicle Interface");
            
        J2534DeviceFinder.FindPluggedInDevice(deviceNames);
        */
        public static J2534PresentDevice FindPluggedInDevice(List<string> deviceRegistryNames)
        {
            // find which devices in list are installed
            List<J2534Dll> installedJ2534Devices = FindInstalledDevices(deviceRegistryNames);
            if (installedJ2534Devices.Count == 0)
            {
                throw new ApplicationException("no J2354 devices installed");
            }

            // find first plugged in device among those installed
            J2534PresentDevice firstPluggedIn = FindFirstPluggedInDevice(installedJ2534Devices);
            return firstPluggedIn;
        }

        // check which device in list are installed
        private static List<J2534Dll> FindInstalledDevices(List<string> deviceRegistryNames)
        {
            List<J2534Dll> installedDevices = new List<J2534Dll>();
            foreach (string n in deviceRegistryNames)
            {
                J2534Dll jd;
                if ((jd = IsInstalled(n)) != null)
                {
                    installedDevices.Add(jd);
                }
            }
            return installedDevices;
        }

        private static J2534PresentDevice FindFirstPluggedInDevice(List<J2534Dll> installed)
        {
            foreach (J2534Dll i in installed)
            {
                List<J2534PresentDevice> pd = GetPresentDevices(i);
                if (pd.Count > 0)
                {
                    return pd[0];
                }
            }
            throw new ApplicationException("No plugged in J2534 devices found");
        }

        // Return if J2534 dll identified by its registry name is installed
        private static J2534Dll IsInstalled(string name)
        {
            foreach (J2534Dll d in J2534InstalledDlls.DllList)
            {
                if (name == d.Name)
                {
                    return d;
                }
            }
            return null;
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
                tempList.Add(new J2534PresentDevice(sDevice, dll));
            }

            return tempList;
        }
    }
}
