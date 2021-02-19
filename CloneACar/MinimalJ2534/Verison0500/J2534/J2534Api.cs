using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Minimal_J2534_0500
{
    internal class J2534Api
    {
        private readonly string importDll;   // = @"C:\Program Files (x86)\Drew Technologies, Inc\J2534\MongoosePro MFC2\MongooseProMFC2.dll";
        private IntPtr pDll;
        public bool noActivation = false;    // set to true if DTErase not supported   

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

            IntPtr pAddressOfFunctionToCall = KernelFunctions.GetProcAddress(pDll, "PassThruScanForDevices");
            if (pAddressOfFunctionToCall != IntPtr.Zero)
                PTScanForDevices = (DelegatePassThruScanForDevices)Marshal.GetDelegateForFunctionPointer(
                                                                                        pAddressOfFunctionToCall,
                                                                                        typeof(DelegatePassThruScanForDevices));

            pAddressOfFunctionToCall = KernelFunctions.GetProcAddress(pDll, "PassThruWriteMsgs");
            if (pAddressOfFunctionToCall != IntPtr.Zero)
                PTWriteMsgs = (DelegatePassThruWriteMsgs)Marshal.GetDelegateForFunctionPointer(
                                                                                        pAddressOfFunctionToCall,
                                                                                        typeof(DelegatePassThruWriteMsgs));

            pAddressOfFunctionToCall = KernelFunctions.GetProcAddress(pDll, "PassThruGetNextDevice");
            if (pAddressOfFunctionToCall != IntPtr.Zero)
                PTGetNextDevice = (DelegatePassThruGetNextDevice)Marshal.GetDelegateForFunctionPointer(
                                                                                        pAddressOfFunctionToCall,
                                                                                        typeof(DelegatePassThruGetNextDevice));

            pAddressOfFunctionToCall = KernelFunctions.GetProcAddress(pDll, "PassThruOpen");
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

            pAddressOfFunctionToCall = KernelFunctions.GetProcAddress(pDll, "PassThruLogicalConnect");
            if (pAddressOfFunctionToCall != IntPtr.Zero)
                PTLogicalConnect = (DelegatePassThruLogicalConnect)Marshal.GetDelegateForFunctionPointer(
                                                                                        pAddressOfFunctionToCall,
                                                                                        typeof(DelegatePassThruLogicalConnect));   

            pAddressOfFunctionToCall = KernelFunctions.GetProcAddress(pDll, "PassThruLogicalDisconnect");
            if (pAddressOfFunctionToCall != IntPtr.Zero)
                PTLogicalDisconnect = (DelegatePassThruLogicalDisconnect)Marshal.GetDelegateForFunctionPointer(
                                                                                        pAddressOfFunctionToCall,
                                                                                        typeof(DelegatePassThruLogicalDisconnect));

            pAddressOfFunctionToCall = KernelFunctions.GetProcAddress(pDll, "PassThruSelect");
            if (pAddressOfFunctionToCall != IntPtr.Zero)
                PTSelect = (DelegatePassThruSelect)Marshal.GetDelegateForFunctionPointer(
                                                                                        pAddressOfFunctionToCall,
                                                                                        typeof(DelegatePassThruSelect));

            pAddressOfFunctionToCall = KernelFunctions.GetProcAddress(pDll, "PassThruReadMsgs");
            if (pAddressOfFunctionToCall != IntPtr.Zero)
                PTReadMsgs = (DelegatePassThruReadMsgs)Marshal.GetDelegateForFunctionPointer(
                                                                                        pAddressOfFunctionToCall,
                                                                                        typeof(DelegatePassThruReadMsgs));

            pAddressOfFunctionToCall = KernelFunctions.GetProcAddress(pDll, "PassThruQueueMsgs");
            if (pAddressOfFunctionToCall != IntPtr.Zero)
                PTQueueMsgs = (DelegatePassThruQueueMsgs)Marshal.GetDelegateForFunctionPointer(
                                                                                        pAddressOfFunctionToCall,
                                                                                        typeof(DelegatePassThruQueueMsgs));

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

            pAddressOfFunctionToCall = KernelFunctions.GetProcAddressOrdinal(pDll, 30);
            if (pAddressOfFunctionToCall != IntPtr.Zero)
            {
                del_DTEraseDevice = (DelegateDTEraseDevice)Marshal.GetDelegateForFunctionPointer(
                                                                                        pAddressOfFunctionToCall,
                                                                                        typeof(DelegateDTEraseDevice));
            }
            else
            {
                noActivation = true;
            }

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

            pAddressOfFunctionToCall = KernelFunctions.GetProcAddressOrdinal(pDll, 43);
            if (pAddressOfFunctionToCall != IntPtr.Zero)
                del_DTMisc = (DelegateDTMisc)Marshal.GetDelegateForFunctionPointer(
                                                                                        pAddressOfFunctionToCall,
                                                                                        typeof(DelegateDTMisc));
            //=========================================================
            // special delegates for IntPtr parameters
            // usually only used for testing error codes (null pointer)
            //=========================================================
            pAddressOfFunctionToCall = KernelFunctions.GetProcAddress(pDll, "PassThruScanForDevices");
            if (pAddressOfFunctionToCall != IntPtr.Zero)
                PTScanForDevicesPtr = (DelegatePassThruScanForDevicesPtr)Marshal.GetDelegateForFunctionPointer(
                                                                                        pAddressOfFunctionToCall,
                                                                                        typeof(DelegatePassThruScanForDevicesPtr));

            pAddressOfFunctionToCall = KernelFunctions.GetProcAddress(pDll, "PassThruGetNextDevice");
            if (pAddressOfFunctionToCall != IntPtr.Zero)
                PTGetNextDevicePtr = (DelegatePassThruGetNextDevicePtr)Marshal.GetDelegateForFunctionPointer(
                                                                                        pAddressOfFunctionToCall,
                                                                                        typeof(DelegatePassThruGetNextDevicePtr));

            pAddressOfFunctionToCall = KernelFunctions.GetProcAddress(pDll, "PassThruOpen");
            if (pAddressOfFunctionToCall != IntPtr.Zero)
                PTOpenPtr = (DelegatePassThruOpenPtr)Marshal.GetDelegateForFunctionPointer(
                                                                                    pAddressOfFunctionToCall,
                                                                                    typeof(DelegatePassThruOpenPtr));

            pAddressOfFunctionToCall = KernelFunctions.GetProcAddress(pDll, "PassThruConnect");
            if (pAddressOfFunctionToCall != IntPtr.Zero)
                PTConnectPtr = (DelegatePassThruConnectPtr)Marshal.GetDelegateForFunctionPointer(
                                                                                        pAddressOfFunctionToCall,
                                                                                        typeof(DelegatePassThruConnectPtr));

            pAddressOfFunctionToCall = KernelFunctions.GetProcAddress(pDll, "PassThruLogicalConnect");
            if (pAddressOfFunctionToCall != IntPtr.Zero)
                PTLogicalConnectPtr = (DelegatePassThruLogicalConnectPtr)Marshal.GetDelegateForFunctionPointer(
                                                                                        pAddressOfFunctionToCall,
                                                                                        typeof(DelegatePassThruLogicalConnectPtr));

            pAddressOfFunctionToCall = KernelFunctions.GetProcAddress(pDll, "PassThruQueueMsgs");
            if (pAddressOfFunctionToCall != IntPtr.Zero)
                PTQueueMsgsPtr = (DelegatePassThruQueueMsgsPtr)Marshal.GetDelegateForFunctionPointer(
                                                                                        pAddressOfFunctionToCall,
                                                                                        typeof(DelegatePassThruQueueMsgsPtr));

            pAddressOfFunctionToCall = KernelFunctions.GetProcAddress(pDll, "PassThruStartPeriodicMsg");
            if (pAddressOfFunctionToCall != IntPtr.Zero)
                PTStartPeriodicMsgPtr = (DelegatePassThruStartPeriodicMsgPtr)Marshal.GetDelegateForFunctionPointer(
                                                                                        pAddressOfFunctionToCall,
                                                                                        typeof(DelegatePassThruStartPeriodicMsgPtr));

            pAddressOfFunctionToCall = KernelFunctions.GetProcAddress(pDll, "PassThruStartPeriodicMsg");
            if (pAddressOfFunctionToCall != IntPtr.Zero)
                PTStartPeriodicMsgPtr2 = (DelegatePassThruStartPeriodicMsgPtr2)Marshal.GetDelegateForFunctionPointer(
                                                                                        pAddressOfFunctionToCall,
                                                                                        typeof(DelegatePassThruStartPeriodicMsgPtr2));

            pAddressOfFunctionToCall = KernelFunctions.GetProcAddress(pDll, "PassThruStartMsgFilter");
            if (pAddressOfFunctionToCall != IntPtr.Zero)
                PTStartMsgFilterPtr1 = (DelegatePassThruStartMsgFilterPtr1)Marshal.GetDelegateForFunctionPointer(
                                                                                        pAddressOfFunctionToCall,
                                                                                        typeof(DelegatePassThruStartMsgFilterPtr1));
            pAddressOfFunctionToCall = KernelFunctions.GetProcAddress(pDll, "PassThruStartMsgFilter");
            if (pAddressOfFunctionToCall != IntPtr.Zero)
                PTStartMsgFilterPtr2 = (DelegatePassThruStartMsgFilterPtr2)Marshal.GetDelegateForFunctionPointer(
                                                                                        pAddressOfFunctionToCall,
                                                                                        typeof(DelegatePassThruStartMsgFilterPtr2));

            pAddressOfFunctionToCall = KernelFunctions.GetProcAddress(pDll, "PassThruStartMsgFilter");
            if (pAddressOfFunctionToCall != IntPtr.Zero)
                PTStartMsgFilterPtr3 = (DelegatePassThruStartMsgFilterPtr3)Marshal.GetDelegateForFunctionPointer(
                                                                                        pAddressOfFunctionToCall,
                                                                                        typeof(DelegatePassThruStartMsgFilterPtr3));

            pAddressOfFunctionToCall = KernelFunctions.GetProcAddress(pDll, "PassThruSetProgrammingVoltage");
            if (pAddressOfFunctionToCall != IntPtr.Zero)
                PTSetProgrammingVoltagePtr = (DelegatePassThruSetProgrammingVoltagePtr)Marshal.GetDelegateForFunctionPointer(
                                                                                        pAddressOfFunctionToCall,
                                                                                        typeof(DelegatePassThruSetProgrammingVoltagePtr));

            pAddressOfFunctionToCall = KernelFunctions.GetProcAddress(pDll, "PassThruReadVersion");
            if (pAddressOfFunctionToCall != IntPtr.Zero)
                PTReadVersionPtr1 = (DelegatePassThruReadVersionPtr1)Marshal.GetDelegateForFunctionPointer(
                                                                                        pAddressOfFunctionToCall,
                                                                                        typeof(DelegatePassThruReadVersionPtr1));

            pAddressOfFunctionToCall = KernelFunctions.GetProcAddress(pDll, "PassThruReadVersion");
            if (pAddressOfFunctionToCall != IntPtr.Zero)
                PTReadVersionPtr2 = (DelegatePassThruReadVersionPtr2)Marshal.GetDelegateForFunctionPointer(
                                                                                        pAddressOfFunctionToCall,
                                                                                        typeof(DelegatePassThruReadVersionPtr2));

            pAddressOfFunctionToCall = KernelFunctions.GetProcAddress(pDll, "PassThruReadVersion");
            if (pAddressOfFunctionToCall != IntPtr.Zero)
                PTReadVersionPtr3 = (DelegatePassThruReadVersionPtr3)Marshal.GetDelegateForFunctionPointer(
                                                                                        pAddressOfFunctionToCall,
                                                                                        typeof(DelegatePassThruReadVersionPtr3));

            pAddressOfFunctionToCall = KernelFunctions.GetProcAddress(pDll, "PassThruReadVersion");
            if (pAddressOfFunctionToCall != IntPtr.Zero)
                PTReadVersionPtrAll = (DelegatePassThruReadVersionPtrAll)Marshal.GetDelegateForFunctionPointer(
                                                                                        pAddressOfFunctionToCall,
                                                                                        typeof(DelegatePassThruReadVersionPtrAll));

            pAddressOfFunctionToCall = KernelFunctions.GetProcAddress(pDll, "PassThruGetLastError");
            if (pAddressOfFunctionToCall != IntPtr.Zero)
                PTGetLastErrorPtr = (DelegatePassThruGetLastErrorPtr)Marshal.GetDelegateForFunctionPointer(
                                                                                        pAddressOfFunctionToCall,
                                                                                        typeof(DelegatePassThruGetLastErrorPtr));

            return true;
        }

        private bool FreeLibrary()
        {
            return KernelFunctions.FreeLibrary(pDll);
        }

        #region api calls

        public void PassThruOpen(out uint device_id)
        {
            PassThruOpen(IntPtr.Zero, out device_id);
        }
        
        public void PassThruOpen(string name, out uint device_id)
        {
            IntPtr ptrName = Marshal.StringToHGlobalAnsi(name);
            try
            {
                PassThruOpen(ptrName, out device_id);
            }
            finally
            {
                Marshal.FreeHGlobal(ptrName);
            }
        }

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

        public void PassThruConnect(uint device_id, ProtocolId protocol_id, uint flags, uint baud_rate, RESOURCE_STRUCT connectResource, out uint channel_id)
        {
            J2534Err e = (J2534Err)PTConnect(device_id, (uint)protocol_id, flags, baud_rate, connectResource, out channel_id);
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

        public void PassThruLogicalConnect(uint physicalChannelId, uint protocolId, uint flags, ISO15765_CHANNEL_DESCRIPTOR channelDescriptor, out uint channelId)
        {
            IntPtr ptrChannelDescriptor = Marshal.AllocHGlobal(Marshal.SizeOf(channelDescriptor));
            Marshal.StructureToPtr(channelDescriptor, ptrChannelDescriptor, true);

            J2534Err e = (J2534Err)PTLogicalConnect(physicalChannelId, protocolId, flags, ptrChannelDescriptor, out channelId);

            Marshal.FreeHGlobal(ptrChannelDescriptor);

            if (e != J2534Err.STATUS_NOERROR)
            {
                StringBuilder sb = new StringBuilder(100);
                PassThruGetLastError(sb);
                throw new J2534Exception(e, sb);
            }
        }

        

        public void PassThruLogicalDisconnect(uint channelId)
        {
            J2534Err e = (J2534Err)PTLogicalDisconnect(channelId);
            if (e != J2534Err.STATUS_NOERROR)
            {
                StringBuilder sb = new StringBuilder(100);
                PassThruGetLastError(sb);
                throw new J2534Exception(e, sb);
            }
        }

        public void PassThruSelect(IntPtr ptrChannelSet, SelectType selectType, uint timeout)
        {
            J2534Err e = (J2534Err)PTSelect(ptrChannelSet, (uint)selectType, timeout);
            if (e != J2534Err.STATUS_NOERROR)
            {
                StringBuilder sb = new StringBuilder(100);
                PassThruGetLastError(sb);
                throw new J2534Exception(e, sb);
            }
        }

        public void PassThruReadMsgs(uint channel_id, ref PASSTHRU_MSG[] msg, out uint num_msgs, uint timeout)
        {
            J2534Err e = (J2534Err)PTReadMsgs(channel_id, msg, out num_msgs, timeout);

            if ((e != J2534Err.STATUS_NOERROR))
            {
                StringBuilder sb = new StringBuilder(100);
                PassThruGetLastError(sb);
                throw new J2534Exception(e, sb);
            }
        }

        public void PassThruQueueMsgs(uint channel_id, PASSTHRU_MSG[] msg, ref uint num_msgs)
        {
            J2534Err e = (J2534Err)PTQueueMsgs(channel_id, msg, ref num_msgs);
            if (e != J2534Err.STATUS_NOERROR)
            {
                StringBuilder sb = new StringBuilder(100);
                PassThruGetLastError(sb);
                throw new J2534Exception(e, sb);
            }
            //System.Threading.Thread.Sleep(2100);
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

        public void PassThruStartPeriodicMsg(uint channel_id, PASSTHRU_MSG msg, out uint msg_id, uint time_interval)
        {
            J2534Err e = (J2534Err)PTStartPeriodicMsg(channel_id, ref msg, out msg_id, time_interval);
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
        
        public void PassThruStartMsgFilter(uint channelId, FilterDef filterType, PASSTHRU_MSG maskMsg, PASSTHRU_MSG patternMsg, out uint filterId)
        {
            J2534Err e = (J2534Err)PTStartMsgFilter(channelId, (uint)filterType, ref maskMsg, ref patternMsg, out filterId);
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

        public void PassThruSetProgrammingVoltage(uint device_id, RESOURCE_STRUCT resourceStruct, uint voltage)
        {
            J2534Err e = (J2534Err)PTSetProgrammingVoltage(device_id, resourceStruct, voltage);
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

        public void PassThruGetLastError(IntPtr p)
        {
            J2534Err e = (J2534Err)PTGetLastErrorPtr(p);
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

        public void PassThruScanForDevices(out uint deviceCount)
        {
            J2534Err e = (J2534Err)PTScanForDevices(out deviceCount);
            if (e != J2534Err.STATUS_NOERROR)
        {
                StringBuilder sb = new StringBuilder(100);
                PassThruGetLastError(sb);
                throw new J2534Exception(e, sb);
            }
        }

        public void PassThruScanForDevices(IntPtr pDeviceCount)
        {
            J2534Err e = (J2534Err)PTScanForDevicesPtr(pDeviceCount);
            if (e != J2534Err.STATUS_NOERROR)
        {
                StringBuilder sb = new StringBuilder(100);
                PassThruGetLastError(sb);
                throw new J2534Exception(e, sb);
            }
        }

        public void PassThruGetNextDevice(out SDEVICE sdev)
        {
            J2534Err e = (J2534Err)PTGetNextDevice(out sdev);
            if (e != J2534Err.STATUS_NOERROR)
            {
                StringBuilder sb = new StringBuilder(100);
                PassThruGetLastError(sb);
                throw new J2534Exception(e, sb);
            }
        }

        public void PassThruGetNextDevice(IntPtr psdev)
        {
            J2534Err e = (J2534Err)PTGetNextDevicePtr(psdev);
            if (e != J2534Err.STATUS_NOERROR)
            {
                StringBuilder sb = new StringBuilder(100);
                PassThruGetLastError(sb);
                throw new J2534Exception(e, sb);
            }
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

        public int DTMisc(DTECH.DtMiscActionId action_id, IntPtr pInput, IntPtr pOutput)
        {
            return del_DTMisc((uint)action_id, pInput, pOutput);
        }

        //=========================================================
        // special function overloads for IntPtr parameters
        // usually only used for testing error codes (null pointer)
        //=========================================================
        public void PassThruOpen(IntPtr p, IntPtr pd)
        {
            J2534Err e = (J2534Err)PTOpenPtr(p, pd);
            if (e != J2534Err.STATUS_NOERROR)
            {
                StringBuilder sb = new StringBuilder(100);
                PassThruGetLastError(sb);
                throw new J2534Exception(e, sb);
            }
        }
        public void PassThruOpen(string name, IntPtr pd)
        {
            IntPtr ptrName = Marshal.StringToHGlobalAnsi(name);
            try
            {
                PassThruOpen(ptrName, pd);
            }
            finally
            {
                Marshal.FreeHGlobal(ptrName);
            }
        }

        public void PassThruConnect(uint device_id, ProtocolId protocol_id, uint flags, uint baud_rate, RESOURCE_STRUCT connectResource, IntPtr ptr_channel_id)
        {
            J2534Err e = (J2534Err)PTConnectPtr(device_id, (uint)protocol_id, flags, baud_rate, connectResource, ptr_channel_id);
            if (e != J2534Err.STATUS_NOERROR)
            {
                StringBuilder sb = new StringBuilder(100);
                PassThruGetLastError(sb);
                throw new J2534Exception(e, sb);
            }
        }

        public void PassThruLogicalConnect(uint physicalChannelId, uint protocolId, uint flags, IntPtr ptrChannelDescriptor, out uint channelId)
        {
            J2534Err e = (J2534Err)PTLogicalConnect(physicalChannelId, protocolId, flags, ptrChannelDescriptor, out channelId);
            if (e != J2534Err.STATUS_NOERROR)
            {
                StringBuilder sb = new StringBuilder(100);
                PassThruGetLastError(sb);
                throw new J2534Exception(e, sb);
            }
        }
        public void PassThruLogicalConnect(uint physicalChannelId, uint protocolId, uint flags, IntPtr ptrChannelDescriptor, IntPtr ptrChannelId)
        {
            J2534Err e = (J2534Err)PTLogicalConnectPtr(physicalChannelId, protocolId, flags, ptrChannelDescriptor, ptrChannelId);
            if (e != J2534Err.STATUS_NOERROR)
            {
                StringBuilder sb = new StringBuilder(100);
                PassThruGetLastError(sb);
                throw new J2534Exception(e, sb);
            }
        }

        public void PassThruQueueMsgs(uint channel_id, PASSTHRU_MSG[] msg, IntPtr ptrNumMsgs)
        {
            J2534Err e = (J2534Err)PTQueueMsgsPtr(channel_id, msg, ptrNumMsgs);
            if (e != J2534Err.STATUS_NOERROR)
            {
                StringBuilder sb = new StringBuilder(100);
                PassThruGetLastError(sb);
                throw new J2534Exception(e, sb);
            }
        }

        public void PassThruStartPeriodicMsg(uint channel_id, IntPtr ptrMsg, out uint msg_id, uint timeout)
        {
            J2534Err e = (J2534Err)PTStartPeriodicMsgPtr(channel_id, ptrMsg, out msg_id, timeout);
            if (e != J2534Err.STATUS_NOERROR)
            {
                StringBuilder sb = new StringBuilder(100);
                PassThruGetLastError(sb);
                throw new J2534Exception(e, sb);
            }
        }
        public void PassThruStartPeriodicMsg(uint channel_id, PASSTHRU_MSG msg, IntPtr ptrMsgId, uint timeout)
        {
            J2534Err e = (J2534Err)PTStartPeriodicMsgPtr2(channel_id, ref msg, ptrMsgId, timeout);
            if (e != J2534Err.STATUS_NOERROR)
            {
                StringBuilder sb = new StringBuilder(100);
                PassThruGetLastError(sb);
                throw new J2534Exception(e, sb);
            }
        }

        public void PassThruStartMsgFilter(uint channelId, FilterDef filterType, IntPtr ptrMaskMsg, PASSTHRU_MSG patternMsg, out uint filterId)
        {
            J2534Err e = (J2534Err)PTStartMsgFilterPtr1(channelId, (uint)filterType, ptrMaskMsg, ref patternMsg, out filterId);
            if (e != J2534Err.STATUS_NOERROR)
            {
                StringBuilder sb = new StringBuilder(100);
                PassThruGetLastError(sb);
                throw new J2534Exception(e, sb);
            }
        }
        public void PassThruStartMsgFilter(uint channelId, FilterDef filterType, PASSTHRU_MSG maskMsg, IntPtr ptrPatternMsg, out uint filterId)
        {
            J2534Err e = (J2534Err)PTStartMsgFilterPtr2(channelId, (uint)filterType, ref maskMsg, ptrPatternMsg, out filterId);
            if (e != J2534Err.STATUS_NOERROR)
            {
                StringBuilder sb = new StringBuilder(100);
                PassThruGetLastError(sb);
                throw new J2534Exception(e, sb);
            }
        }
        public void PassThruStartMsgFilter(uint channelId, FilterDef filterType, PASSTHRU_MSG maskMsg, PASSTHRU_MSG patternMsg, IntPtr ptrFilterId)
        {
            J2534Err e = (J2534Err)PTStartMsgFilterPtr3(channelId, (uint)filterType, ref maskMsg, ref patternMsg, ptrFilterId);
            if (e != J2534Err.STATUS_NOERROR)
            {
                StringBuilder sb = new StringBuilder(100);
                PassThruGetLastError(sb);
                throw new J2534Exception(e, sb);
            }
        }

        public void PassThruSetProgrammingVoltage(uint device_id, IntPtr ptrResourceStruct, uint voltage)
        {
            J2534Err e = (J2534Err)PTSetProgrammingVoltagePtr(device_id, ptrResourceStruct, voltage);
            if (e != J2534Err.STATUS_NOERROR)
            {
                StringBuilder sb = new StringBuilder(100);
                PassThruGetLastError(sb);
                throw new J2534Exception(e, sb);
            }
        }

        public void PassThruReadVersion(uint device_id, IntPtr ptr_firmware_version, StringBuilder dll_version, StringBuilder api_version)
        {
            J2534Err e = (J2534Err)PTReadVersionPtr1(device_id, ptr_firmware_version, dll_version, api_version);
            if (e != J2534Err.STATUS_NOERROR)
            {
                StringBuilder sb = new StringBuilder(100);
                PassThruGetLastError(sb);
                throw new J2534Exception(e, sb);
            }
        }
        public void PassThruReadVersion(uint device_id, StringBuilder firmware_version, IntPtr ptr_dll_version, StringBuilder api_version)
        {
            J2534Err e = (J2534Err)PTReadVersionPtr2(device_id, firmware_version, ptr_dll_version, api_version);
            if (e != J2534Err.STATUS_NOERROR)
            {
                StringBuilder sb = new StringBuilder(100);
                PassThruGetLastError(sb);
                throw new J2534Exception(e, sb);
            }
        }
        public void PassThruReadVersion(uint device_id, StringBuilder firmware_version, StringBuilder dll_version, IntPtr ptr_api_version)
        {
            J2534Err e = (J2534Err)PTReadVersionPtr3(device_id, firmware_version, dll_version, ptr_api_version);
            if (e != J2534Err.STATUS_NOERROR)
            {
                StringBuilder sb = new StringBuilder(100);
                PassThruGetLastError(sb);
                throw new J2534Exception(e, sb);
            }
        }
        public void PassThruReadVersion(uint device_id, IntPtr ptr_firmware_version, IntPtr ptr_dll_version, IntPtr ptr_api_version)
        {
            J2534Err e = (J2534Err)PTReadVersionPtrAll(device_id, ptr_firmware_version, ptr_dll_version, ptr_api_version);
            if (e != J2534Err.STATUS_NOERROR)
            {
                StringBuilder sb = new StringBuilder(100);
                PassThruGetLastError(sb);
                throw new J2534Exception(e, sb);
            }
        }

        #endregion api calls

        #region function delegates

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int DelegatePassThruScanForDevices(out uint deviceCount);
        private DelegatePassThruScanForDevices PTScanForDevices;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int DelegatePassThruGetNextDevice(out SDEVICE sDevice);
        private DelegatePassThruGetNextDevice PTGetNextDevice;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int DelegatePassThruOpen(IntPtr p, out uint deviceId);
        private DelegatePassThruOpen PTOpen;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int DelegatePassThruClose(uint deviceId);
        private DelegatePassThruClose PTClose;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int DelegatePassThruConnect(uint deviceId, uint protocolId, uint flags, uint baudRate, RESOURCE_STRUCT connectResource, out uint channelId);
        private DelegatePassThruConnect PTConnect;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int DelegatePassThruDisconnect(uint channelId);
        private DelegatePassThruDisconnect PTDisconnect;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int DelegatePassThruLogicalConnect(uint physicalChannelId, uint protocolId, uint flags, IntPtr ptrChannelDescriptor, out uint channelId);
        private DelegatePassThruLogicalConnect PTLogicalConnect;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int DelegatePassThruLogicalDisconnect(uint physicalChannelId);
        private DelegatePassThruLogicalDisconnect PTLogicalDisconnect;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int DelegatePassThruSelect(IntPtr ptrChannelSet, uint selectType, uint timeout);
        private DelegatePassThruSelect PTSelect;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int DelegatePassThruReadMsgs(uint channelId, [In, Out] PASSTHRU_MSG[] pMsgs, out uint numMsgs, uint timeout);
        private DelegatePassThruReadMsgs PTReadMsgs;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int DelegatePassThruQueueMsgs(uint channelId, [In] PASSTHRU_MSG[] pMsg, ref uint numMsgs);
        private DelegatePassThruQueueMsgs PTQueueMsgs;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int DelegatePassThruStartPeriodicMsg(uint channelId, [In]ref PASSTHRU_MSG pMsg, out uint pMsgId, uint timeInterval);
        private DelegatePassThruStartPeriodicMsg PTStartPeriodicMsg;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int DelegatePassThruWriteMsgs(uint channelId, [In] PASSTHRU_MSG[] pMsg, ref uint numMsgs, uint timeout);               
        private DelegatePassThruWriteMsgs PTWriteMsgs;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int DelegatePassThruStopPeriodicMsg(uint channelId, uint msgId);
        private DelegatePassThruStopPeriodicMsg PTStopPeriodicMsg;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int DelegatePassThruStartMsgFilter(uint channelid, uint filterType, [In]ref PASSTHRU_MSG pMaskMsg, [In]ref PASSTHRU_MSG pPatternMsg, out uint filterId);
        private DelegatePassThruStartMsgFilter PTStartMsgFilter;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int DelegatePassThruStopMsgFilter(uint channelId, uint filterId);
        private DelegatePassThruStopMsgFilter PTStopMsgFilter;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int DelegatePassThruSetProgrammingVoltage(uint deviceId, RESOURCE_STRUCT resourceStruct, uint voltage);
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

        //=========================================================
        // special delegates for IntPtr parameters
        // usually only used for testing error codes (null pointer)
        //=========================================================
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int DelegatePassThruScanForDevicesPtr(IntPtr p);
        private DelegatePassThruScanForDevicesPtr PTScanForDevicesPtr;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int DelegatePassThruGetNextDevicePtr(IntPtr p);
        private DelegatePassThruGetNextDevicePtr PTGetNextDevicePtr;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int DelegatePassThruOpenPtr(IntPtr p, IntPtr pDeviceId);
        private DelegatePassThruOpenPtr PTOpenPtr;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int DelegatePassThruConnectPtr(uint deviceId, uint protocolId, uint flags, uint baudRate, RESOURCE_STRUCT connectResource, IntPtr ptrChannelId);
        private DelegatePassThruConnectPtr PTConnectPtr;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int DelegatePassThruLogicalConnectPtr(uint physicalChannelId, uint protocolId, uint flags, IntPtr ptrChannelDescriptor, IntPtr ptrChannelId);
        private DelegatePassThruLogicalConnectPtr PTLogicalConnectPtr;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int DelegatePassThruQueueMsgsPtr(uint channelId, [In] PASSTHRU_MSG[] pMsg, IntPtr ptrNumMsgs);
        private DelegatePassThruQueueMsgsPtr PTQueueMsgsPtr;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int DelegatePassThruStartPeriodicMsgPtr(uint channelId, IntPtr pMsg, out uint pMsgId, uint timeInterval);
        private DelegatePassThruStartPeriodicMsgPtr PTStartPeriodicMsgPtr;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int DelegatePassThruStartPeriodicMsgPtr2(uint channelId, [In]ref PASSTHRU_MSG pMsg, IntPtr pMsgId, uint timeInterval);
        private DelegatePassThruStartPeriodicMsgPtr2 PTStartPeriodicMsgPtr2;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int DelegatePassThruStartMsgFilterPtr1(uint channelid, uint filterType, IntPtr pMaskMsg, [In]ref PASSTHRU_MSG pPatternMsg, out uint filterId);
        private DelegatePassThruStartMsgFilterPtr1 PTStartMsgFilterPtr1;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int DelegatePassThruStartMsgFilterPtr2(uint channelid, uint filterType, [In]ref PASSTHRU_MSG pMaskMsg, IntPtr pPatternMsg, out uint filterId);
        private DelegatePassThruStartMsgFilterPtr2 PTStartMsgFilterPtr2;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int DelegatePassThruStartMsgFilterPtr3(uint channelid, uint filterType, [In]ref PASSTHRU_MSG pMaskMsg, [In]ref PASSTHRU_MSG pPatternMsg, IntPtr ptrFilterId);
        private DelegatePassThruStartMsgFilterPtr3 PTStartMsgFilterPtr3;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int DelegatePassThruSetProgrammingVoltagePtr(uint deviceId, IntPtr ptrResourceStruct, uint voltage);
        private DelegatePassThruSetProgrammingVoltagePtr PTSetProgrammingVoltagePtr;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int DelegatePassThruReadVersionPtr1(uint deviceId, IntPtr ptrFirmwareVersion, StringBuilder dllVersion, StringBuilder apiVersion);
        private DelegatePassThruReadVersionPtr1 PTReadVersionPtr1;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int DelegatePassThruReadVersionPtr2(uint deviceId, StringBuilder firmwareVersion, IntPtr ptrDllVersion, StringBuilder apiVersion);
        private DelegatePassThruReadVersionPtr2 PTReadVersionPtr2;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int DelegatePassThruReadVersionPtr3(uint deviceId, StringBuilder firmwareVersion, StringBuilder dllVersion, IntPtr ptrApiVersion);
        private DelegatePassThruReadVersionPtr3 PTReadVersionPtr3;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int DelegatePassThruReadVersionPtrAll(uint deviceId, IntPtr ptrfirmwareVersion, IntPtr ptrdllVersion, IntPtr ptrApiVersion);
        private DelegatePassThruReadVersionPtrAll PTReadVersionPtrAll;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int DelegatePassThruGetLastErrorPtr(IntPtr p);
        private DelegatePassThruGetLastErrorPtr PTGetLastErrorPtr;

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


        #endregion function delegates
    }

    
}