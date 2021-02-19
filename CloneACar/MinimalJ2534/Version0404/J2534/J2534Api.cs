using System;
using System.Runtime.InteropServices;
using System.Text;
using static Minimal_J2534_0404.DTECH;   // DTECH enums in J2534Define.cs

namespace Minimal_J2534_0404
{
    internal class J2534Api
    {
        private readonly string importDll; // = @"C:\Program Files (x86)\Drew Technologies, Inc\J2534\MongoosePro MFC2\MongooseProMFC2.dll";
        private IntPtr pDll;
        public bool DTGetDeviceTypeExists = false;

        public J2534Api(string pathDll)
        {
            importDll = pathDll;
            LoadJ2534Library(importDll);
        }

        ~J2534Api()
        {
            FreeLibrary();
        }

        private bool LoadJ2534Library(string path)
        {
            pDll = KernelFunctions.LoadLibrary(importDll);

            if (pDll == IntPtr.Zero)
                return false;

            IntPtr pAddressOfFunctionToCall = KernelFunctions.GetProcAddress(pDll, "PassThruOpen");
            if (pAddressOfFunctionToCall != IntPtr.Zero)
                PTOpen = (DelegatePassThruOpen)Marshal.GetDelegateForFunctionPointer(
                                                                                    pAddressOfFunctionToCall,
                                                                                    typeof(DelegatePassThruOpen));

            pAddressOfFunctionToCall = KernelFunctions.GetProcAddress(pDll, "PassThruClose");
            if (pAddressOfFunctionToCall != IntPtr.Zero)
                PTClose = (DelegatePassThruClose)Marshal.GetDelegateForFunctionPointer(
                                                                                    pAddressOfFunctionToCall,
                                                                                    typeof(DelegatePassThruClose));

            pAddressOfFunctionToCall = KernelFunctions.GetProcAddress(pDll, "PassThruConnect");
            if (pAddressOfFunctionToCall != IntPtr.Zero)
                PTConnect = (DelegatePassThruConnect)Marshal.GetDelegateForFunctionPointer(
                                                                                        pAddressOfFunctionToCall,
                                                                                        typeof(DelegatePassThruConnect));

            pAddressOfFunctionToCall = KernelFunctions.GetProcAddress(pDll, "PassThruDisconnect");
            if (pAddressOfFunctionToCall != IntPtr.Zero)
                PTDisconnect = (DelegatePassThruDisconnect)Marshal.GetDelegateForFunctionPointer(
                                                                                        pAddressOfFunctionToCall,
                                                                                        typeof(DelegatePassThruDisconnect));

            pAddressOfFunctionToCall = KernelFunctions.GetProcAddress(pDll, "PassThruReadMsgs");
            if (pAddressOfFunctionToCall != IntPtr.Zero)
                PTReadMsgs = (DelegatePassThruReadMsgs)Marshal.GetDelegateForFunctionPointer(
                                                                                        pAddressOfFunctionToCall,
                                                                                        typeof(DelegatePassThruReadMsgs));

            pAddressOfFunctionToCall = KernelFunctions.GetProcAddress(pDll, "PassThruWriteMsgs");
            if (pAddressOfFunctionToCall != IntPtr.Zero)
                PTWriteMsgs = (DelegatePassThruWriteMsgs)Marshal.GetDelegateForFunctionPointer(
                                                                                        pAddressOfFunctionToCall,
                                                                                        typeof(DelegatePassThruWriteMsgs));

            pAddressOfFunctionToCall = KernelFunctions.GetProcAddress(pDll, "PassThruStartPeriodicMsg");
            if (pAddressOfFunctionToCall != IntPtr.Zero)
                PTStartPeriodicMsg = (DelegatePassThruStartPeriodicMsg)Marshal.GetDelegateForFunctionPointer(
                                                                                        pAddressOfFunctionToCall,
                                                                                        typeof(DelegatePassThruStartPeriodicMsg));

            pAddressOfFunctionToCall = KernelFunctions.GetProcAddress(pDll, "PassThruStopPeriodicMsg");
            if (pAddressOfFunctionToCall != IntPtr.Zero)
                PTStopPeriodicMsg = (DelegatePassThruStopPeriodicMsg)Marshal.GetDelegateForFunctionPointer(
                                                                                        pAddressOfFunctionToCall,
                                                                                        typeof(DelegatePassThruStopPeriodicMsg));

            pAddressOfFunctionToCall = KernelFunctions.GetProcAddress(pDll, "PassThruStartMsgFilter");
            if (pAddressOfFunctionToCall != IntPtr.Zero)
                PTStartMsgFilter = (DelegatePassThruStartMsgFilter)Marshal.GetDelegateForFunctionPointer(
                                                                                        pAddressOfFunctionToCall,
                                                                                        typeof(DelegatePassThruStartMsgFilter));

            pAddressOfFunctionToCall = KernelFunctions.GetProcAddress(pDll, "PassThruStartMsgFilter");
            if (pAddressOfFunctionToCall != IntPtr.Zero)
                PTStartMsgFilterFlowPtr = (DelegatePassThruStartMsgFilterFlowPtr)Marshal.GetDelegateForFunctionPointer(
                                                                                        pAddressOfFunctionToCall,
                                                                                        typeof(DelegatePassThruStartMsgFilterFlowPtr));

            /*
            pAddressOfFunctionToCall = KernelFunctions.GetProcAddress(pDll, "PassThruStartMsgFilter");
            if (pAddressOfFunctionToCall != IntPtr.Zero)
                PTStartPassBlockMsgFilter = (DelegatePassThruStartPassBlockMsgFilter)Marshal.GetDelegateForFunctionPointer(
                                                                                        pAddressOfFunctionToCall,
                                                                                        typeof(DelegatePassThruStartPassBlockMsgFilter));
            */

            pAddressOfFunctionToCall = KernelFunctions.GetProcAddress(pDll, "PassThruStopMsgFilter");
            if (pAddressOfFunctionToCall != IntPtr.Zero)
                PTStopMsgFilter = (DelegatePassThruStopMsgFilter)Marshal.GetDelegateForFunctionPointer(
                                                                                        pAddressOfFunctionToCall,
                                                                                        typeof(DelegatePassThruStopMsgFilter));

            pAddressOfFunctionToCall = KernelFunctions.GetProcAddress(pDll, "PassThruSetProgrammingVoltage");
            if (pAddressOfFunctionToCall != IntPtr.Zero)
                PTSetProgrammingVoltage = (DelegatePassThruSetProgrammingVoltage)Marshal.GetDelegateForFunctionPointer(
                                                                                        pAddressOfFunctionToCall,
                                                                                        typeof(DelegatePassThruSetProgrammingVoltage));

            pAddressOfFunctionToCall = KernelFunctions.GetProcAddress(pDll, "PassThruReadVersion");
            if (pAddressOfFunctionToCall != IntPtr.Zero)
                PTReadVersion = (DelegatePassThruReadVersion)Marshal.GetDelegateForFunctionPointer(
                                                                                        pAddressOfFunctionToCall,
                                                                                        typeof(DelegatePassThruReadVersion));

            pAddressOfFunctionToCall = KernelFunctions.GetProcAddress(pDll, "PassThruGetLastError");
            if (pAddressOfFunctionToCall != IntPtr.Zero)
                PTGetLastError = (DelegatePassThruGetLastError)Marshal.GetDelegateForFunctionPointer(
                                                                                        pAddressOfFunctionToCall,
                                                                                        typeof(DelegatePassThruGetLastError));

            pAddressOfFunctionToCall = KernelFunctions.GetProcAddress(pDll, "PassThruIoctl");
            if (pAddressOfFunctionToCall != IntPtr.Zero)
                PTIoctl = (DelegatePassThruIoctl)Marshal.GetDelegateForFunctionPointer(
                                                                                        pAddressOfFunctionToCall,
                                                                                        typeof(DelegatePassThruIoctl));

            pAddressOfFunctionToCall = KernelFunctions.GetProcAddress(pDll, "PassThruGetNextCarDAQ");
            if (pAddressOfFunctionToCall != IntPtr.Zero)
                del_DTGetNextCarDAQ = (DelegateDTGetNextCarDAQ)Marshal.GetDelegateForFunctionPointer(
                                                                                        pAddressOfFunctionToCall,
                                                                                        typeof(DelegateDTGetNextCarDAQ));

            // loading up the same function as above but with a different delegate that will allow
            // us to send in NULL pointers to use the function in a different way, in this case to 
            // initiate a device re-enumeration within the DLL
            pAddressOfFunctionToCall = KernelFunctions.GetProcAddress(pDll, "PassThruGetNextCarDAQ");
            if (pAddressOfFunctionToCall != IntPtr.Zero)
                del_DTInitGetNextCarDAQ = (DelegateDTInitGetNextCarDAQ)Marshal.GetDelegateForFunctionPointer(
                                                                                        pAddressOfFunctionToCall,
                                                                                        typeof(DelegateDTInitGetNextCarDAQ));

            pAddressOfFunctionToCall = KernelFunctions.GetProcAddressOrdinal(pDll, 30);
            if (pAddressOfFunctionToCall != IntPtr.Zero)
                del_DTEraseDevice = (DelegateDTEraseDevice)Marshal.GetDelegateForFunctionPointer(
                                                                                        pAddressOfFunctionToCall,
                                                                                        typeof(DelegateDTEraseDevice));

            pAddressOfFunctionToCall = KernelFunctions.GetProcAddressOrdinal(pDll, 31);
            if (pAddressOfFunctionToCall != IntPtr.Zero)
                del_DTWipeDevice = (DelegateDTWipeDevice)Marshal.GetDelegateForFunctionPointer(
                                                                                        pAddressOfFunctionToCall,
                                                                                        typeof(DelegateDTWipeDevice));

            pAddressOfFunctionToCall = KernelFunctions.GetProcAddressOrdinal(pDll, 40);
            if (pAddressOfFunctionToCall != IntPtr.Zero)
                del_DTGetSerial = (DelegateDTGetSerial)Marshal.GetDelegateForFunctionPointer(
                                                                                        pAddressOfFunctionToCall,
                                                                                        typeof(DelegateDTGetSerial));

            pAddressOfFunctionToCall = KernelFunctions.GetProcAddressOrdinal(pDll, 41);
            if (pAddressOfFunctionToCall != IntPtr.Zero)
                del_DTGetRnd = (DelegateDTGetRnd)Marshal.GetDelegateForFunctionPointer(
                                                                                        pAddressOfFunctionToCall,
                                                                                        typeof(DelegateDTGetRnd));

            pAddressOfFunctionToCall = KernelFunctions.GetProcAddressOrdinal(pDll, 42);
            if (pAddressOfFunctionToCall != IntPtr.Zero)
            {
                del_DTGetDeviceType = (DelegateDTGetDeviceType)Marshal.GetDelegateForFunctionPointer(
                                                                                        pAddressOfFunctionToCall,
                                                                                        typeof(DelegateDTGetDeviceType));
                DTGetDeviceTypeExists = true;
            }

            pAddressOfFunctionToCall = KernelFunctions.GetProcAddressOrdinal(pDll, 43);
            if (pAddressOfFunctionToCall != IntPtr.Zero)
                del_DTMisc = (DelegateDTMisc)Marshal.GetDelegateForFunctionPointer(
                                                                                        pAddressOfFunctionToCall,
                                                                                        typeof(DelegateDTMisc));

            return true;
        }

        private bool FreeLibrary()
        {
            return KernelFunctions.FreeLibrary(pDll);
        }

        #region api calls

        

        public void PassThruOpen(IntPtr p, out uint device_id)
        {
            J2534Err e = (J2534Err)PTOpen(p, out device_id);
            if (e != J2534Err.STATUS_NOERROR)
            {
                StringBuilder sb = new StringBuilder(100);
                PassThruGetLastError(sb);
                throw new J2534Exception(e, sb);
            }
        }

        public void PassThruClose(uint device_id)
        {
            J2534Err e = (J2534Err)PTClose(device_id);
            if (e != J2534Err.STATUS_NOERROR)
            {
                StringBuilder sb = new StringBuilder(100);
                PassThruGetLastError(sb);
                throw new J2534Exception(e, sb);
            }
        }

        public void PassThruConnect(uint device_id, ProtocolId protocol_id, uint flags, uint baud_rate, out uint channel_id)
        {
            J2534Err e = (J2534Err)PTConnect(device_id, (uint)protocol_id, flags, baud_rate, out channel_id);
            if (e != J2534Err.STATUS_NOERROR)
            {
                StringBuilder sb = new StringBuilder(100);
                PassThruGetLastError(sb);
                throw new J2534Exception(e, sb);
            }
        }

        public void PassThruDisconnect(uint channel_id)
        {
            J2534Err e = (J2534Err)PTDisconnect(channel_id);
            if (e != J2534Err.STATUS_NOERROR)
            {
                StringBuilder sb = new StringBuilder(100);
                PassThruGetLastError(sb);
                throw new J2534Exception(e, sb);
            }
        }

        public void PassThruReadMsgs(uint channel_id, PASSTHRU_MSG[] msg, out uint num_msgs, uint timeout)
        {
            J2534Err e = (J2534Err)PTReadMsgs(channel_id, msg, out num_msgs, timeout);

            if ((e != J2534Err.STATUS_NOERROR))
            {
                StringBuilder sb = new StringBuilder(100);
                PassThruGetLastError(sb);
                throw new J2534Exception(e, sb);
            }
        }

        /// <summary>
        /// send single message without needing PASSTHRU_MSG[]
        /// </summary>
        public void PassThruWriteMsgs(uint channel_id, PASSTHRU_MSG msg, out uint num_msgs, uint timeout)
        {
            PASSTHRU_MSG[] msga = new PASSTHRU_MSG[1];

            num_msgs = 1;
            msga[0] = msg;

            J2534Err e = (J2534Err)PTWriteMsgs(channel_id, msga, ref num_msgs, timeout);
            if (e != J2534Err.STATUS_NOERROR)
            {
                StringBuilder sb = new StringBuilder(100);
                PassThruGetLastError(sb);
                throw new J2534Exception(e, sb);
            }
        }

        public void PassThruWriteMsgs(uint channel_id, PASSTHRU_MSG[] msg, ref uint num_msgs, uint timeout)
        {
            J2534Err e = (J2534Err)PTWriteMsgs(channel_id, msg, ref num_msgs, timeout);
            if (e != J2534Err.STATUS_NOERROR)
            {
                StringBuilder sb = new StringBuilder(100);
                PassThruGetLastError(sb);
                throw new J2534Exception(e, sb);
            }
        }

        public void PassThruStartPeriodicMsg(uint channel_id, PASSTHRU_MSG msg, out uint msg_id, uint timeout)
        {
            J2534Err e = (J2534Err)PTStartPeriodicMsg(channel_id, ref msg, out msg_id, timeout);
            if (e != J2534Err.STATUS_NOERROR)
            {
                StringBuilder sb = new StringBuilder(100);
                PassThruGetLastError(sb);
                throw new J2534Exception(e, sb);
            }
        }

        public void PassThruStopPeriodicMsg(uint channel_id, uint msg_id)
        {
            J2534Err e = (J2534Err)PTStopPeriodicMsg(channel_id, msg_id);
            if (e != J2534Err.STATUS_NOERROR)
            {
                StringBuilder sb = new StringBuilder(100);
                PassThruGetLastError(sb);
                throw new J2534Exception(e, sb);
            }
        }

        /// <summary>
        /// function wrapper for pass and block filters that don't need to pass a flowMsg struct
        /// </summary>
        public void PassThruStartMsgFilter(uint channel_id, FilterDef filter_type, PASSTHRU_MSG mask_msg, PASSTHRU_MSG pattern_msg, out uint filter_id)
        {
            PASSTHRU_MSG flowMsg = new PASSTHRU_MSG(-1);
            PassThruStartMsgFilter(channel_id, filter_type, mask_msg, pattern_msg, flowMsg, out filter_id);
        }

        public void PassThruStartMsgFilter(uint channel_id, FilterDef filter_type, PASSTHRU_MSG mask_msg, PASSTHRU_MSG pattern_msg, PASSTHRU_MSG? flowcontrol_msg, out uint filter_id)
        {
            J2534Err e;
            if (flowcontrol_msg == null)
            {
                e = (J2534Err)PTStartMsgFilterFlowPtr(channel_id, (uint)filter_type, ref mask_msg, ref pattern_msg, IntPtr.Zero, out filter_id);
            }
            else
            {
                PASSTHRU_MSG flowcontrol_msg_no_null = new PASSTHRU_MSG();
                flowcontrol_msg_no_null = (PASSTHRU_MSG)flowcontrol_msg;
                e = (J2534Err)PTStartMsgFilter(channel_id, (uint)filter_type, ref mask_msg, ref pattern_msg, ref flowcontrol_msg_no_null, out filter_id);
            }
      
            if (e != J2534Err.STATUS_NOERROR)
            {
                StringBuilder sb = new StringBuilder(100);
                PassThruGetLastError(sb);
                throw new J2534Exception(e, sb);
            }
        }

        public void PassThruStopMsgFilter(uint channel_id, uint filter_id)
        {
            J2534Err e = (J2534Err)PTStopMsgFilter(channel_id, filter_id);
            if (e != J2534Err.STATUS_NOERROR)
            {
                StringBuilder sb = new StringBuilder(100);
                PassThruGetLastError(sb);
                throw new J2534Exception(e, sb);
            }
        }

        public void PassThruSetProgrammingVoltage(uint device_id, uint pin_number, uint voltage)
        {
            J2534Err e = (J2534Err)PTSetProgrammingVoltage(device_id, pin_number, voltage);
            if (e != J2534Err.STATUS_NOERROR)
            {
                StringBuilder sb = new StringBuilder(100);
                PassThruGetLastError(sb);
                throw new J2534Exception(e, sb);
            }
        }

        public void PassThruReadVersion(uint device_id, StringBuilder firmware_version, StringBuilder dll_version, StringBuilder api_version)
        {
            J2534Err e = (J2534Err)PTReadVersion(device_id, firmware_version, dll_version, api_version);
            if (e != J2534Err.STATUS_NOERROR)
            {
                StringBuilder sb = new StringBuilder(100);
                PassThruGetLastError(sb);
                throw new J2534Exception(e, sb);
            }
        }

        public void PassThruGetLastError(StringBuilder last_error_str)
        {
            J2534Err e = (J2534Err)PTGetLastError(last_error_str);
            if (e != J2534Err.STATUS_NOERROR)
            {
                throw new J2534Exception(e);
            }
        }

        public void PassThruIoctl(uint channel_id, IoctlId ioctl_id, IntPtr pInput, IntPtr pOutput)
        {
            J2534Err e = (J2534Err)PTIoctl(channel_id, (uint)ioctl_id, pInput, pOutput);
            if (e != J2534Err.STATUS_NOERROR)
            {
                StringBuilder sb = new StringBuilder(100);
                PassThruGetLastError(sb);
                throw new J2534Exception(e, sb);
            }
        }

        /// <summary>
        /// function wrapper to initiate the GetNextCarDAQ sequence, this will cause the DLL to "discover" currently connected devices
        /// (this must be called before repeatedly calling DTGetNextCarDAQ to get the list list of devices one by one)
        /// </summary>
        public void DTInitGetNextCarDAQ()
        {
            // passing in NULLs for any one of the parameters will initiate a re-enumeration procedure and return immediately
            J2534Err e = (J2534Err)del_DTInitGetNextCarDAQ(IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
        }

        /// <summary>
        /// function wrapper to get the list of connected devices, one be one, until parameters come back as empty strings("")
        /// (DTInitGetNextCarDAQ must be called to initialize the procedure)
        /// </summary>
        public void DTGetNextCarDAQ(out String name, out uint version, out String address)
        {
            int pName = 0;
            int pAddress = 0;

            IntPtr ppName = Marshal.AllocHGlobal(Marshal.SizeOf(pName));
            IntPtr ppNameCopy = ppName;             // need to do this as GetNextCarDAQ modifies the pointer
            Marshal.StructureToPtr(pName, ppName, true);

            IntPtr ppAddress = Marshal.AllocHGlobal(Marshal.SizeOf(pAddress));
            IntPtr ppAddressCopy = ppAddress;        // need to do this as GetNextCarDAQ modifies the pointer
            Marshal.StructureToPtr(pAddress, ppAddress, true);

            J2534Err e = (J2534Err)del_DTGetNextCarDAQ(ref ppName, out version, ref ppAddress);
            if (e != J2534Err.STATUS_NOERROR)
            {
                StringBuilder sb = new StringBuilder(100);
                PassThruGetLastError(sb);
                throw new J2534Exception(e, sb);
            }

            //Marshal.FreeHGlobal(ppName);
            name = Marshal.PtrToStringAnsi(ppName);
            address = Marshal.PtrToStringAnsi(ppAddress);

            Marshal.FreeHGlobal(ppNameCopy);
            Marshal.FreeHGlobal(ppAddressCopy);
        }

        public int DTEraseDevice(uint device_id)
        {
            return del_DTEraseDevice(device_id);
        }

        public int DTWipeDevice(uint a, uint b, StringBuilder theString)
        {
            return del_DTWipeDevice(a, b, theString);
        }

        public int DTGetSerial(StringBuilder theSerial)
        {
            return del_DTGetSerial(theSerial);
        }

        public int DTGetRnd(IntPtr theRnd)
        {
            return del_DTGetRnd(theRnd);
        }

        public int DTMisc(DTECH.DtMiscActionId action_id, IntPtr pInput, IntPtr pOutput)
        {
            return del_DTMisc((uint)action_id, pInput, pOutput);
        }

        public int DTGetDeviceType(IntPtr pTheDeviceType)
        {
            return del_DTGetDeviceType(pTheDeviceType);
        }

        #endregion api calls

        #region function delegates

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int DelegatePassThruOpen(IntPtr p, out uint deviceId);
        private DelegatePassThruOpen PTOpen;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int DelegatePassThruClose(uint deviceId);
        private DelegatePassThruClose PTClose;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int DelegatePassThruConnect(uint deviceId, uint protocolId, uint flags, uint baudRate, out uint channelId);
        private DelegatePassThruConnect PTConnect;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int DelegatePassThruDisconnect(uint channelId);
        private DelegatePassThruDisconnect PTDisconnect;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int DelegatePassThruReadMsgs(uint channelId, [In, Out]PASSTHRU_MSG[] pMsg, out uint numMsgs, uint timeout);            // note: adding 'ref' to pMsg parameter causes crash
        private DelegatePassThruReadMsgs PTReadMsgs;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int DelegatePassThruWriteMsgs(uint channelId, [In]PASSTHRU_MSG[] pMsg, ref uint numMsgs, uint timeout);                // note: adding 'ref' to pMsg parameter causes crash
        private DelegatePassThruWriteMsgs PTWriteMsgs;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int DelegatePassThruStartPeriodicMsg(uint channelId, [In]ref PASSTHRU_MSG pMsg, out uint pMsgId, uint timeInterval);   // note: removing 'ref' from pMsg parameter causes crash
        private DelegatePassThruStartPeriodicMsg PTStartPeriodicMsg;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int DelegatePassThruStopPeriodicMsg(uint channelId, uint msgId);
        private DelegatePassThruStopPeriodicMsg PTStopPeriodicMsg;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int DelegatePassThruStartMsgFilter(uint channelid, uint filterType, [In]ref PASSTHRU_MSG pMaskMsg, [In]ref PASSTHRU_MSG pPatternMsg, [In]ref PASSTHRU_MSG pFlowMsg, out uint filterId);
        private DelegatePassThruStartMsgFilter PTStartMsgFilter;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int DelegatePassThruStartMsgFilterFlowPtr(uint channelid, uint filterType, [In]ref PASSTHRU_MSG pMaskMsg, [In]ref PASSTHRU_MSG pPatternMsg, [In]IntPtr pFlowMsg, out uint filterId);
        private DelegatePassThruStartMsgFilterFlowPtr PTStartMsgFilterFlowPtr;

        /*
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int DelegatePassThruStartPassBlockMsgFilter(uint channelid, uint filterType, ref PASSTHRU_MSG uMaskMsg, ref PASSTHRU_MSG uPatternMsg, uint zero, ref int filterId);
        private DelegatePassThruStartPassBlockMsgFilter PTStartPassBlockMsgFilter;
        */

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int DelegatePassThruStopMsgFilter(uint channelId, uint filterId);
        private DelegatePassThruStopMsgFilter PTStopMsgFilter;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int DelegatePassThruSetProgrammingVoltage(uint deviceId, uint pinNumber, uint voltage);
        private DelegatePassThruSetProgrammingVoltage PTSetProgrammingVoltage;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int DelegatePassThruReadVersion(uint deviceId, StringBuilder firmwareVersion, StringBuilder dllVersion, StringBuilder apiVersion);
        private DelegatePassThruReadVersion PTReadVersion;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int DelegatePassThruGetLastError(StringBuilder errorDescription);
        private DelegatePassThruGetLastError PTGetLastError;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int DelegatePassThruIoctl(uint channelId, uint ioctlID, IntPtr pInput, IntPtr pOutput);
        private DelegatePassThruIoctl PTIoctl;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int DelegateDTGetNextCarDAQ([In, Out]ref IntPtr name, out uint pVersion, [In, Out]ref IntPtr pIPAddress);
        private DelegateDTGetNextCarDAQ del_DTGetNextCarDAQ;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        // essentially the same as the one above but allows us to send in null pointers to initiate a re-enumeration of connected devices
        private delegate int DelegateDTInitGetNextCarDAQ(IntPtr name, IntPtr pVersion, IntPtr pIPAddress);
        private DelegateDTInitGetNextCarDAQ del_DTInitGetNextCarDAQ;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int DelegateDTEraseDevice(uint deviceId);
        private DelegateDTEraseDevice del_DTEraseDevice;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int DelegateDTWipeDevice(uint a, uint b, StringBuilder theString);
        private DelegateDTWipeDevice del_DTWipeDevice;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int DelegateDTGetSerial(StringBuilder theSerial);
        private DelegateDTGetSerial del_DTGetSerial;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int DelegateDTMisc(uint ioctlID, IntPtr pInput, IntPtr pOutput);
        private DelegateDTMisc del_DTMisc;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int DelegateDTGetRnd(IntPtr theRnd);
        private DelegateDTGetRnd del_DTGetRnd;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int DelegateDTGetDeviceType(IntPtr theDeviceType);
        private DelegateDTGetDeviceType del_DTGetDeviceType;

        #endregion function delegates
    }

    
}