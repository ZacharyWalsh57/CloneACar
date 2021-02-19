namespace Minimal_J2534_0500
{
    internal class SupportedFeatures
    {
        public static bool J1926Pin(J2534Dll jdll)
        {
            switch (jdll.Name)
            {
                case "MongoosePro Honda":
                    return false;

                case "MongoosePro Honda-BT":
                    return true;

                case "MongoosePro Honda-CVG":
                    return true;

                case "MongoosePro ISO2":
                    return false;

                case "MongoosePro MFC2":
                    return true;

                case "MongoosePro MFC2-BT":
                    return true;

                case "MongoosePro VW":
                    return true;

                default:
                    return false;
            }
        }

        public static bool Bluetooth(J2534Dll jdll)
        {
            switch (jdll.Name)
            {
                case "MongoosePro Honda":
                    return false;

                case "MongoosePro Honda-BT":
                    return true;

                case "MongoosePro Honda-CVG":
                    return false;

                case "MongoosePro ISO2":
                    return false;

                case "MongoosePro MFC2":
                    return false;

                case "MongoosePro MFC2-BT":
                    return true;

                case "MongoosePro VW":
                    return false;

                default:
                    return false;
            }
        }

        public static bool Wifi(J2534Dll jdll)
        {
            switch (jdll.Name)
            {
                case "MongoosePro Honda":
                    return false;

                case "MongoosePro Honda-BT":
                    return false;

                case "MongoosePro Honda-CVG":
                    return true;

                case "MongoosePro ISO2":
                    return false;

                case "MongoosePro MFC2":
                    return false;

                case "MongoosePro MFC2-BT":
                    return false;

                case "MongoosePro VW":
                    return false;

                default:
                    return false;
            }
        }

        public static bool ShipActivated(J2534Dll jdll)
        {
            switch (jdll.Name)
            {
                case "MongoosePro Honda":
                    return true;

                case "MongoosePro Honda-BT":
                    return true;

                case "MongoosePro Honda-CVG":
                    return true;

                case "MongoosePro ISO2":
                    return false;

                case "MongoosePro MFC2":
                    return true;

                case "MongoosePro MFC2-BT":
                    return true;

                case "MongoosePro VW":
                    return false;

                default:
                    return false;
            }
        }

        /// <summary>
        /// determine if device has EU and US versions
        /// </summary>
        public static bool EuUsSelection(string dut)
        {
            if (dut == "MongoosePro Honda")
            {
                return false;
            }
            else if (dut == "MongoosePro Honda-BT")
            {
                return false;
            }
            else if (dut == "MongoosePro Honda-CVG")
            {
                return false;
            }
            else if (dut == "MongoosePro ISO2")
            {
                return false;
            }
            else if (dut == "MongoosePro MFC2")
            {
                return true;
            }
            else if (dut == "MongoosePro MFC2-BT")
            {
                return true;
            }
            else if (dut == "MongoosePro VW")
            {
                return false;
            }

            return false;
        }

        /// <summary>
        /// determine if device has Rc2 / non-Rc2 options
        /// </summary>
        public static bool Rc2Selection(string dut)
        {
            if (dut == "MongoosePro Honda")
            {
                return false;
            }
            else if (dut == "MongoosePro Honda-BT")
            {
                return false;
            }
            else if (dut == "MongoosePro Honda-CVG")
            {
                return false;
            }
            else if (dut == "MongoosePro ISO2")
            {
                return false;
            }
            else if (dut == "MongoosePro MFC2")
            {
                return true;
            }
            else if (dut == "MongoosePro MFC2-BT")
            {
                return false;
            }
            else if (dut == "MongoosePro VW")
            {
                return false;
            }

            return false;
        }
    }
}