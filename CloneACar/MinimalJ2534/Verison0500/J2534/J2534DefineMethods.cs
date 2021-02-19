using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Minimal_J2534_0500
{
    class J2534DefineMethods
    {
        // from section 7.2.3 of J2534-1
        static public uint MaxDataBufferSize(ProtocolId p)
        {
            switch (p)
            {
                case ProtocolId.J1850VPW:
                    return 4128;

                case ProtocolId.J1850PWM:
                    return 4128;

                case ProtocolId.ISO9141:
                    return 4128;

                case ProtocolId.ISO14230:
                    return 260;

                case ProtocolId.CAN:
                    return 800;

                case ProtocolId.SW_CAN:
                    return 8;

                case ProtocolId.FT_CAN:
                    return 8;

                case ProtocolId.J2610:
                    return 4128;

                case ProtocolId.ETHERNET:
                    return 4128;

                case ProtocolId.ISO15765_LOGICAL:
                    return 4095;

                //case ProtocolId.GM_UART:
                //    return 4128;

                //case ProtocolId.UART_ECHO_BYTE:
                //    return 4128;

                //case ProtocolId.HONDA_DIAGH:
                //    return 4128;

                //case ProtocolId.J1708:
                //    return 4128;

                //case ProtocolId.J1939_LOGICAL:
                //    return 4128;

                //case ProtocolId.TP2_0_LOGICAL:
                //    return 4128;

                //case ProtocolId.TP1_6_LOGICAL:
                //    return 4128;

                //case ProtocolId.DT_CEC1:
                //    return 4128;

                //case ProtocolId.DT_KW82:
                //    return 4128;

                default:
                    return 4128;
            }
        }

        static public uint MaxDataBufferSizePeriodic(ProtocolId p)
        {
            switch (p)
            {
                case ProtocolId.J1850VPW:
                    return 12;

                case ProtocolId.J1850PWM:
                    return 10;

                case ProtocolId.ISO9141:
                    return 12;

                case ProtocolId.ISO14230:
                    return 12;

                case ProtocolId.CAN:
                    return 12;

                case ProtocolId.SW_CAN:
                    return 12;

                case ProtocolId.FT_CAN:
                    return 12;

                case ProtocolId.J2610:
                    return 12;

                case ProtocolId.ETHERNET:
                    return 4128;

                case ProtocolId.ISO15765_LOGICAL:
                    return 11;

                //case ProtocolId.GM_UART:
                //    return 4128;

                //case ProtocolId.UART_ECHO_BYTE:
                //    return 4128;

                //case ProtocolId.HONDA_DIAGH:
                //    return 4128;

                //case ProtocolId.J1708:
                //    return 4128;

                //case ProtocolId.J1939_LOGICAL:
                //    return 4128;

                //case ProtocolId.TP2_0_LOGICAL:
                //    return 4128;

                //case ProtocolId.TP1_6_LOGICAL:
                //    return 4128;

                //case ProtocolId.DT_CEC1:
                //    return 4128;

                //case ProtocolId.DT_KW82:
                //    return 4128;

                default:
                    return 12;
            }
        }

        static public uint MaxDataBufferSizeFilter(ProtocolId p)
        {
            switch (p)
            {
                case ProtocolId.J1850VPW:
                    return 12;

                case ProtocolId.J1850PWM:
                    return 10;

                case ProtocolId.ISO9141:
                    return 12;

                case ProtocolId.ISO14230:
                    return 12;

                case ProtocolId.CAN:
                    return 12;

                case ProtocolId.SW_CAN:
                    return 12;

                case ProtocolId.FT_CAN:
                    return 8;

                case ProtocolId.J2610:
                    return 12;

                //case ProtocolId.GM_UART:
                //    return 4128;

                //case ProtocolId.UART_ECHO_BYTE:
                //    return 4128;

                //case ProtocolId.HONDA_DIAGH:
                //    return 4128;

                //case ProtocolId.J1708:
                //    return 4128;

                //case ProtocolId.J1939_LOGICAL:
                //    return 4128;

                //case ProtocolId.TP2_0_LOGICAL:
                //    return 4128;

                //case ProtocolId.TP1_6_LOGICAL:
                //    return 4128;

                //case ProtocolId.DT_CEC1:
                //    return 4128;

                //case ProtocolId.DT_KW82:
                //    return 4128;

                default:
                    return 12;
            }
        }

        // return an SRESOURCE based on protocol and pins, if pins is null, use default pins for protocol
        static public RESOURCE_STRUCT SetupConnectResource(ProtocolId p, int[] pins = null)
        {
            RESOURCE_STRUCT connectResource = new RESOURCE_STRUCT();
            connectResource.Connector = (uint)Connector.J1962_CONNECTOR;

            int[] resourceList = new int[2];

            if (pins != null)
            {
                resourceList = new int[pins.Length];
                for (int i=0; i<pins.Length; i++)
                {
                    resourceList[i] = pins[i];
                }
                connectResource.NumOfResources = (uint)pins.Length;
            }
            else
            {
                switch (p)
                {
                    case ProtocolId.J1850PWM:
                        connectResource.NumOfResources = 2;
                        resourceList[0] = 2;
                        resourceList[1] = 10;
                        break;

                    case ProtocolId.J1850VPW:
                        connectResource.NumOfResources = 1;
                        resourceList[0] = 2;
                        break;

                    case ProtocolId.ISO9141:
                        connectResource.NumOfResources = 2;
                        resourceList[0] = 7;
                        resourceList[1] = 15;
                        break;

                    case ProtocolId.ISO14230:
                        connectResource.NumOfResources = 2;
                        resourceList[0] = 7;
                        resourceList[1] = 15;
                        break;

                    case ProtocolId.CAN:
                        connectResource.NumOfResources = 2;
                        resourceList[0] = 6;
                        resourceList[1] = 14;
                        break;
                }
            }

            connectResource.ptrResourceList = Marshal.AllocHGlobal(resourceList.Length * Marshal.SizeOf(resourceList[0]));
            Marshal.Copy(resourceList, 0, connectResource.ptrResourceList, resourceList.Length);

            return connectResource;
        }

        static public void ReleaseConnectResource(RESOURCE_STRUCT connectResource)
        {
            Marshal.FreeHGlobal(connectResource.ptrResourceList);
        }
    }
}
