using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Minimal_J2534_0500
{
    /// <summary>
    /// wrapper for the J2534Api with regular C# types as parameters
    /// this class should be the only place where marshaling is done
    /// </summary>
    class J2534ApiMarshal
    {
        J2534Api j2534Api;

        public J2534ApiMarshal(J2534Api api)
        {
            j2534Api = api;
        }

        // PassThruScanForDevices
        public uint PassThruScanForDevices()
        {
            uint deviceCount;
            j2534Api.PassThruScanForDevices(out deviceCount);
            return deviceCount;
        }

        // PassThruGetNextDevice
        public List<SDevice> PassThruGetNextDevice(uint deviceCount)
        {
            List<SDevice> sdList = new List<SDevice>();

            for (int i = 0; i < deviceCount; i++)
            {
                SDEVICE sdev = new SDEVICE();
                j2534Api.PassThruGetNextDevice(out sdev);

                sdList.Add(CopySDeviceFromNative(sdev));
            }
            return sdList;
        }

        // PassThruOpen
        public void PassThruOpen(out uint deviceId)
        {
            j2534Api.PassThruOpen(out deviceId);
        }
        public void PassThruOpen(string name, out uint deviceId)
        {
            IntPtr ptrName = Marshal.StringToHGlobalAnsi(name);
            try
            {
                j2534Api.PassThruOpen(ptrName, out deviceId);
            }
            finally
            {
                Marshal.FreeHGlobal(ptrName);
            }
        }

        // PassThruClose
        public void PassThruClose(uint deviceId)
        {
            j2534Api.PassThruClose(deviceId);
        }

        // PassThruConnect
        public void PassThruConnect(uint deviceId, ProtocolId protocolId, uint flags, uint baudRate, out uint channelId, int[] pins = null)
        {
            RESOURCE_STRUCT sResource = J2534DefineMethods.SetupConnectResource(protocolId, pins);
            j2534Api.PassThruConnect(deviceId, protocolId, flags, baudRate, sResource, out channelId);
            J2534DefineMethods.ReleaseConnectResource(sResource);
        }

        // PassThruDisconnect
        public void PassThruDisconnect(uint channelId)
        {
            j2534Api.PassThruDisconnect(channelId);
        }

        // PassThruLogicalConnect
        public void PassThruLogicalConnect(uint physicalChannelId, ProtocolId protocolId, uint flags, Iso15765ChannelDescriptor cD, out uint channelId)
        {
            ISO15765_CHANNEL_DESCRIPTOR channelDescriptor = new ISO15765_CHANNEL_DESCRIPTOR(cD.localTxFlags, cD.remoteTxFlags, cD.localAddress, cD.remoteAddress);
            j2534Api.PassThruLogicalConnect(physicalChannelId, (uint)protocolId, flags, channelDescriptor, out channelId);
        }

        // PassThruLogicalDisconnect
        public void PassThruLogicalDisconnect(uint channelId)
        {
            j2534Api.PassThruLogicalDisconnect(channelId);
        }

        // PassThruSelect
        public void PassThruSelect(ref SChannelSet cS, uint timeout)
        {
            if (cS.channelList.Count != cS.channelCount)
            {
                throw new Exception("PassThruSelect, SChannelSet.channelList count different than SChannelSet.channelCount");
            }

            SCHANNELSET channelSet = new SCHANNELSET{ ChannelCount = cS.channelCount, ChannelThreshold = cS.channelThreshold };

            // allocate unmanaged memory and create pointer for the ChannelList
            IntPtr ptrChannelList = Marshal.AllocHGlobal((int)(sizeof(uint) * cS.channelCount));
            int[] intArray = cS.channelList.ToArray();
            Marshal.Copy(intArray, 0, ptrChannelList, (int)cS.channelCount);

            // initialize the channel list pointer in the SCHANNELSET
            channelSet.ptrChannelList = ptrChannelList;

            // create a pointer to the channelset
            IntPtr ptrChannelSet = Marshal.AllocHGlobal(Marshal.SizeOf(channelSet));
            Marshal.StructureToPtr(channelSet, ptrChannelSet, true);

            j2534Api.PassThruSelect(ptrChannelSet, SelectType.READABLE_TYPE, timeout);

            // copy channelset data from unmanaged to managed
            channelSet = (SCHANNELSET)Marshal.PtrToStructure(ptrChannelSet, typeof(SCHANNELSET));

            // copy channel list data from unmanaged
            if (channelSet.ChannelCount > 0)
            {
                intArray = new int[channelSet.ChannelCount];
                Marshal.Copy(ptrChannelList, intArray, 0, (int)channelSet.ChannelCount);
            }
            else
            {
                intArray = new int[0];
            }

            // copy SCHANNELSET to SChannelSet
            cS.channelCount = channelSet.ChannelCount;
            cS.channelThreshold = channelSet.ChannelThreshold;
            cS.channelList = intArray.ToList();

            // free unmanaged memory
            Marshal.DestroyStructure(ptrChannelSet, typeof(SCHANNELSET));
            Marshal.FreeHGlobal(ptrChannelList);
        }

        // PassThruReadMsgs
        public void PassThruReadMsgs(uint channelId, out PassThruMsg[] msg, ref uint numMsgs, uint timeout, ProtocolId protocol)
        {
            msg = null;

            PASSTHRU_MSG[] rxMsg = new PASSTHRU_MSG[numMsgs];
            for (int i = 0; i < numMsgs; i++)
            {
                rxMsg[i] = new PASSTHRU_MSG(J2534DefineMethods.MaxDataBufferSize(protocol));
            }

            try
            {
                j2534Api.PassThruReadMsgs(channelId, ref rxMsg, out numMsgs, timeout);
                if (numMsgs > 0)
                {
                    msg = new PassThruMsg[numMsgs];
                    for (int i = 0; i < numMsgs; i++)
                    {
                        CopyPassThruMsgFromNative(ref msg[i], rxMsg[i]);
                    }
                }
            }
            catch (J2534Exception ex)
            {
                if (ex.j2534ErrorCode == J2534Err.ERR_TIMEOUT)  // if timeout, don't throw error
                {
                    if (numMsgs > 0)    // return what messages we did receive
                    {
                        msg = new PassThruMsg[numMsgs];
                        for (int i = 0; i < numMsgs; i++)
                        {
                            CopyPassThruMsgFromNative(ref msg[i], rxMsg[i]);
                        }
                    }
                }
                else
                {
                    throw;
                }
            }
        }
        // old
        // PassThruReadMsgs
        /*
        public void PassThruReadMsgs(uint channelId, out PassThruMsg[] msg, ref uint numMsgs, uint timeout, ProtocolId protocol)
        {
            PASSTHRU_MSG[] rxMsg = new PASSTHRU_MSG[numMsgs];
            for (int i = 0; i < numMsgs; i++)
            { 
                rxMsg[i] = new PASSTHRU_MSG(J2534DefineMethods.MaxDataBufferSize(protocol));
            }

            try
            {
                j2534Api.PassThruReadMsgs(channelId, ref rxMsg, out numMsgs, timeout);
            }
            catch (J2534Exception ex)
            {
                if (!(ex.j2534ErrorCode == J2534Err.ERR_TIMEOUT))  // if timeout, don't throw error
                {
                    throw;
                }
            }
            finally
            {
                //if (numMsgs > 0)
                //{
                    msg = new PassThruMsg[numMsgs];
                    for (int i = 0; i < numMsgs; i++)
                    {
                        CopyPassThruMsgFromNative(ref msg[i], rxMsg[i]);
                    }
                //}
                //else
                //{
                //    msg = null;
                //}
            }
        }
        */
        public void PassThruReadMsgs(uint channelId, ref PassThruMsg msg, ref uint numMsgs, uint timeout, ProtocolId protocol)
        {
            if (numMsgs > 1)
            {
                throw new Exception("PassThruReadMsgs, for this function overload, numMsgs to read must be 1");
            }

            PassThruMsg[] rxMsg = new PassThruMsg[1];
            rxMsg[0] = msg;

            PassThruReadMsgs(channelId, out rxMsg, ref numMsgs, timeout, protocol);
        }

        // PassThruQueueMsgs
        public void PassThruQueueMsgs(uint channelId, PassThruMsg[] msg, ref uint numMsgs) 
        {
            if (msg.Length < numMsgs)
            {
                throw new Exception("PassThruQueueMsgs, PassThruMsg array size smaller than numMsgs to write");
            }

            PASSTHRU_MSG[] txMsg = new PASSTHRU_MSG[numMsgs];
            for (int i = 0; i < msg.Length; i++)
            {
                txMsg[i] = new PASSTHRU_MSG();
                CopyPassThruMsgToNative(ref txMsg[i], msg[i]);
            }

            j2534Api.PassThruQueueMsgs(channelId, txMsg, ref numMsgs);
        }

        public void PassThruWriteMsgs(uint channelId, PassThruMsg[] msg, ref uint numMsgs, uint timeout)
        {
            if (numMsgs > 1)
            {
                throw new Exception("PassThruWriteMsgs, for this function overload, numMsgs to read must be 1");
            }

            PASSTHRU_MSG[] txMsg = new PASSTHRU_MSG[numMsgs];
            for (int i = 0; i < msg.Length; i++)
            {
                txMsg[i] = new PASSTHRU_MSG();
                CopyPassThruMsgToNative(ref txMsg[i], msg[i]);
            }

            txMsg[0] = new PASSTHRU_MSG(1);
            CopyPassThruMsgToNative(ref txMsg[0], msg[0]);

            j2534Api.PassThruWriteMsgs(channelId, txMsg, ref numMsgs, timeout);
        }

        public void PassThruQueueMsgs(uint channelId, PassThruMsg msg, ref uint numMsgs)
        {
            if (numMsgs > 1)
            {
                throw new Exception("PassThruQueueMsgs, for this function overload, numMsgs to read must be 1");
            }

            PassThruMsg[] txMsg = new PassThruMsg[1];
            txMsg[0] = msg;

            PassThruQueueMsgs(channelId, txMsg, ref numMsgs);
        }

        // PassThruStartPeriodicMsg
        public void PassThruStartPeriodicMsg(uint channelId, PassThruMsg msg, out uint msgId, uint timeInterval)
        {
            PASSTHRU_MSG txMsg = new PASSTHRU_MSG();
            CopyPassThruMsgToNative(ref txMsg, msg);

            j2534Api.PassThruStartPeriodicMsg(channelId, txMsg, out msgId, timeInterval);
        }

        // PassThruStopPeriodicMsg
        public void PassThruStopPeriodicMsg(uint channelId, uint msgId)
        {
            j2534Api.PassThruStopPeriodicMsg(channelId, msgId);
        }

        // PassThruStartMsgFilter
        public void PassThruStartMsgFilter(uint channelId, FilterDef filterType, PassThruMsg maskMsg, PassThruMsg patternMsg, out uint filterId)
        {
            PASSTHRU_MSG maskMsgNative = new PASSTHRU_MSG();
            CopyPassThruMsgToNative(ref maskMsgNative, maskMsg);

            PASSTHRU_MSG patternMsgNative = new PASSTHRU_MSG();
            CopyPassThruMsgToNative(ref patternMsgNative, patternMsg);

            j2534Api.PassThruStartMsgFilter(channelId, filterType, maskMsgNative, patternMsgNative, out filterId);
        }

        // PassThruStopMsgFilter
        public void PassThruStopMsgFilter(uint channelId, uint filterId)
        {
            j2534Api.PassThruStopMsgFilter(channelId, filterId);
        }

        // PassThruSetProgrammingVoltage
        public void PassThruSetProgrammingVoltage(uint deviceId, uint pinNumber, uint voltage)
        {
            RESOURCE_STRUCT r = new RESOURCE_STRUCT();
            r.Connector = (uint)Connector.J1962_CONNECTOR;
            r.NumOfResources = 1;
            r.ptrResourceList = Marshal.AllocHGlobal(sizeof(uint));
            Marshal.Copy(new int[] { (int)pinNumber }, 0, r.ptrResourceList, 1);

            j2534Api.PassThruSetProgrammingVoltage(deviceId, r, voltage);

            Marshal.FreeHGlobal(r.ptrResourceList);
        }

        // PassThruReadVersion
        public void PassThruReadVersion(uint deviceId, out string firmwareVersion, out string dllVersion, out string apiVersion)
        {
            StringBuilder firmwareVersionB = new StringBuilder(100);
            StringBuilder dllVersionB = new StringBuilder(100);
            StringBuilder apiVersionB = new StringBuilder(100);

            j2534Api.PassThruReadVersion(deviceId, firmwareVersionB, dllVersionB, apiVersionB);

            firmwareVersion = firmwareVersionB.ToString();
            dllVersion = dllVersionB.ToString();
            apiVersion = apiVersionB.ToString();
        }

        // PassThruIoctl
        /// <summary>
        /// J2534-1: use for GET_CONFIG, SET_CONFIG
        /// </summary>
        public void PassThruIoctl(uint channelId, IoctlId ioctlId, ref SConfigList sConfigList)
        {
            if (sConfigList.configList.Count != sConfigList.numOfParams)
            {
                throw new Exception("PassThruIoctl, sConfigList parameter count different than sConfigList.numOfParams");
            }

            SCONFIG_LIST sConfigListNative = new SCONFIG_LIST();
            SCONFIG[] configParamNative = new SCONFIG[sConfigList.numOfParams];

            sConfigListNative.NumOfParams = sConfigList.numOfParams;

            int i = 0;
            foreach (SConfig configParam in sConfigList.configList)
            {
                configParamNative[i].Parameter = (uint)configParam.parameter;
                configParamNative[i].Value = configParam.value;
                i++;
            }

            // allocate unmanaged memory and create pointer for the SCONFIG[], fill data manually as it is a variable size array
            IntPtr ptrConfigParamNative = Marshal.AllocHGlobal(Marshal.SizeOf(configParamNative[0]) * configParamNative.Length);
            for (int j = 0; j < configParamNative.Length; j++)
            {
                Marshal.WriteInt32(ptrConfigParamNative, (2*j) * sizeof(Int32), (int)configParamNative[j].Parameter);
                Marshal.WriteInt32(ptrConfigParamNative, (2*j+1) * sizeof(Int32), (int)configParamNative[j].Value);
            }

            // update the SCONFIG_LIST ConfigPtr
            sConfigListNative.ConfigPtr = ptrConfigParamNative;

            // allocate unmanaged memory and create pointer for the SCONFIG_LIST
            IntPtr ptrSConfigListNative = Marshal.AllocHGlobal(Marshal.SizeOf(sConfigListNative));
            Marshal.StructureToPtr(sConfigListNative, ptrSConfigListNative, true);

            // use try catch block so we can release the unmanaged memory whatever happens
            try
            {
                j2534Api.PassThruIoctl(channelId, ioctlId, ptrSConfigListNative, IntPtr.Zero);

                // copy unmanaged memory to managed struct
                sConfigListNative = (SCONFIG_LIST)Marshal.PtrToStructure(ptrSConfigListNative, typeof(SCONFIG_LIST));

                // read the parameter values into managed memory SCONFIG[] struct
                for (int k = 0; k < sConfigListNative.NumOfParams; k++)
                {
                    configParamNative[k].Parameter = (uint)Marshal.ReadInt32(sConfigListNative.ConfigPtr, (2 * k) * sizeof(Int32));
                    configParamNative[k].Value = (uint)Marshal.ReadInt32(sConfigListNative.ConfigPtr, (2 * k + 1) * sizeof(Int32));
                }

                // copy params from SCONFIG[] to C# friendly class SConfigList
                sConfigList.numOfParams = sConfigListNative.NumOfParams;
                i = 0;
                foreach (SConfig configParam in sConfigList.configList)
                {
                    configParam.parameter = (ConfigParamId)configParamNative[i].Parameter;
                    configParam.value = configParamNative[i].Value;
                    i++;
                }
            }
            catch(Exception)
            {
                throw;
            }
            finally
            {
                // release unmanaged memory
                Marshal.DestroyStructure(ptrSConfigListNative, sConfigListNative.GetType());
                Marshal.DestroyStructure(ptrConfigParamNative, configParamNative.GetType());

                Marshal.FreeHGlobal(ptrSConfigListNative);
                Marshal.FreeHGlobal(ptrConfigParamNative);
            }
        }

        /// <summary>
        /// J2534-1: use for READ_PIN_VOLTAGE
        /// </summary>
        public void PassThruIoctl(uint channelId, IoctlId ioctlId, ResourceStruct r, out uint val)
        {
            IntPtr pOutput = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(uint)));
            RESOURCE_STRUCT resourceStruct = new RESOURCE_STRUCT();
            resourceStruct.Connector = (uint)r.connector;
            resourceStruct.NumOfResources = r.numOfResources;

            // assign unallocated memory for resource list
            resourceStruct.ptrResourceList = Marshal.AllocHGlobal(sizeof(uint) * r.resourceList.Count);
            int[] intArray = r.resourceList.ToArray();
            Marshal.Copy(intArray, 0, resourceStruct.ptrResourceList, r.resourceList.Count);

            // make a pointer to the RESOURCE_STRUCT
            IntPtr ptrResourceStruct = Marshal.AllocHGlobal(Marshal.SizeOf(resourceStruct));
            Marshal.StructureToPtr(resourceStruct, ptrResourceStruct, true);

            j2534Api.PassThruIoctl(channelId, ioctlId, ptrResourceStruct, pOutput);

            val = (uint)Marshal.ReadInt32(pOutput);

            // free unmanaged memory
            Marshal.DestroyStructure(ptrResourceStruct, ptrResourceStruct.GetType());
            Marshal.FreeHGlobal(ptrResourceStruct);
            Marshal.FreeHGlobal(pOutput);
        }

        /// <summary>
        /// J2534-1: use for READ_PROG_VOLTAGE
        /// </summary>
        public void PassThruIoctl(uint channelId, IoctlId ioctlId, out uint val)
        {
            IntPtr pOutput = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(uint)));

            j2534Api.PassThruIoctl(channelId, ioctlId, IntPtr.Zero, pOutput);

            val = (uint)Marshal.ReadInt32(pOutput);
        }

        /// <summary>
        /// J2534-1: use for FIVE_BAUD_INIT
        /// </summary>
        public void PassThruIoctl(uint channelId, IoctlId ioctlId, SByteArray inSByte, ref SByteArray outSByte)
        {
            if (inSByte.numOfBytes > inSByte.data.Length)
            {
                throw new Exception("PassThruIoctl, SByteArray (in) numOfBytes larger than allocated data buffer");
            }
            if (outSByte.numOfBytes > outSByte.data.Length)
            {
                throw new Exception("PassThruIoctl, SByteArray (out) numOfBytes larger than allocated data buffer");
            }

            SBYTE_ARRAY inSByteNative = new SBYTE_ARRAY();
            SBYTE_ARRAY outSByteNative = new SBYTE_ARRAY();
            inSByteNative.NumOfBytes = inSByte.numOfBytes;
            outSByteNative.NumOfBytes = outSByte.numOfBytes;

            // allocate unmanaged memory for the byte arrays and copy data of arrays from managed to unmanaged
            inSByteNative.BytePtr = Marshal.AllocHGlobal(inSByte.data.Length);
            Marshal.Copy(inSByte.data, 0, inSByteNative.BytePtr, inSByte.data.Length);
            outSByteNative.BytePtr = Marshal.AllocHGlobal(outSByte.data.Length);
            //Marshal.Copy(outSByte.data, 0, outSByteNative.BytePtr, outSByte.data.Length);

            // allocate unmanaged memory and create pointer for the SBYTE_ARRAYs
            IntPtr ptrInSByteNative = Marshal.AllocHGlobal(Marshal.SizeOf(inSByteNative));
            Marshal.StructureToPtr(inSByteNative, ptrInSByteNative, true);
            IntPtr ptrOutSByteNative = Marshal.AllocHGlobal(Marshal.SizeOf(outSByteNative));
            Marshal.StructureToPtr(outSByteNative, ptrOutSByteNative, true);

            // use try catch block so we can release the unmanaged memory whatever happens
            try
            {
                j2534Api.PassThruIoctl(channelId, ioctlId, ptrInSByteNative, ptrOutSByteNative);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                // copy unmanaged memory to managed struct (SBYTE_ARRAYs)
                //inSByteNative = (SBYTE_ARRAY)Marshal.PtrToStructure(ptrInSByteNative, typeof(SBYTE_ARRAY));
                outSByteNative = (SBYTE_ARRAY)Marshal.PtrToStructure(ptrOutSByteNative, typeof(SBYTE_ARRAY));

                // copy data back to SByteArray classes
                //inSByte.numOfBytes = inSByteNative.NumOfBytes;
                //Marshal.Copy(inSByteNative.BytePtr, inSByte.data, 0, (int)inSByteNative.NumOfBytes);
                outSByte.numOfBytes = outSByteNative.NumOfBytes;
                Marshal.Copy(outSByteNative.BytePtr, outSByte.data, 0, (int)outSByteNative.NumOfBytes);

                // release unmanaged memory
                Marshal.DestroyStructure(ptrInSByteNative, inSByteNative.GetType());
                Marshal.DestroyStructure(ptrOutSByteNative, outSByteNative.GetType());
                
                Marshal.FreeHGlobal(ptrInSByteNative);
                Marshal.FreeHGlobal(ptrOutSByteNative);
            }
        }

        /// <summary>
        /// J2534-1: use for FAST_INIT
        /// </summary>
        public void PassThruIoctl(uint channelId, IoctlId ioctlId, PassThruMsg inMsg, ref PassThruMsg outMsg)
        {
            IntPtr ptrIn = IntPtr.Zero;
            IntPtr ptrOut = IntPtr.Zero;

            PASSTHRU_MSG inMsgNative;
            PASSTHRU_MSG outMsgNative;

            if (inMsg != null)
            {
                inMsgNative = new PASSTHRU_MSG();
                CopyPassThruMsgToNative(ref inMsgNative, inMsg);

                // allocate unmanaged memory and create pointer for the PASSTHRU_MSG
                ptrIn = Marshal.AllocHGlobal(Marshal.SizeOf(inMsgNative));
                Marshal.StructureToPtr(inMsgNative, ptrIn, true);
            }

            if (outMsg != null)
            {
                outMsgNative = new PASSTHRU_MSG();
                CopyPassThruMsgToNative(ref outMsgNative, outMsg);

                // allocate unmanaged memory and create pointer for the PASSTHRU_MSG
                ptrOut = Marshal.AllocHGlobal(Marshal.SizeOf(outMsgNative));
                Marshal.StructureToPtr(outMsgNative, ptrOut, true);
            }

            // use try catch block so we can release the unmanaged memory whatever happens
            try
            {
                j2534Api.PassThruIoctl(channelId, ioctlId, ptrIn, ptrOut);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (outMsg != null)
                {
                // copy unmanaged memory to managed struct (SBYTE_ARRAYs)
                outMsgNative = (PASSTHRU_MSG)Marshal.PtrToStructure(ptrOut, typeof(PASSTHRU_MSG));

                    // copy data back to SByteArray classes
                    CopyPassThruMsgFromNative(ref outMsg, outMsgNative);

                // release unmanaged memory
                Marshal.FreeHGlobal(ptrOut);
                }
            }
        }

        /// <summary>
        /// J2534-1: use for CLEAR_TX_BUFFER, CLEAR_RX_BUFFER, CLEAR_PERIODIC_MSGS, CLEAR_MSG_FILTERS, CLEAR_FUNCT_MSG_LOOKUP_TABLE
        /// </summary>
        public void PassThruIoctl(uint channelId, IoctlId ioctlId)
        {
            j2534Api.PassThruIoctl(channelId, ioctlId, IntPtr.Zero, IntPtr.Zero);
        }

        /// <summary>
        /// J2534-1: use for ADD_TO_FUNCT_MSG_LOOKUP_TABLE, DELETE_FROM_FUNCT_MSG_LOOKUP_TABLE
        /// </summary>
        public void PassThruIoctl(uint channelId, IoctlId ioctlId, SByteArray inSByte)
        {
            if (inSByte.numOfBytes > inSByte.data.Length)
            {
                throw new Exception("PassThruIoctl, SByteArray (in) numOfBytes larger than allocated data buffer");
            }

            SBYTE_ARRAY inSByteNative = new SBYTE_ARRAY();
            inSByteNative.NumOfBytes = inSByte.numOfBytes;

            // allocate unmanaged memory for the byte arrays and copy data of arrays from managed to unmanaged
            inSByteNative.BytePtr = Marshal.AllocHGlobal(inSByte.data.Length);
            Marshal.Copy(inSByte.data, 0, inSByteNative.BytePtr, inSByte.data.Length);

            // allocate unmanaged memory and create pointer for the SBYTE_ARRAYs
            IntPtr ptrInSByteNative = Marshal.AllocHGlobal(Marshal.SizeOf(inSByteNative));
            Marshal.StructureToPtr(inSByteNative, ptrInSByteNative, true);

            // use try catch block so we can release the unmanaged memory whatever happens
            try
            {
                j2534Api.PassThruIoctl(channelId, ioctlId, ptrInSByteNative, IntPtr.Zero);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                // copy unmanaged memory to managed struct (SBYTE_ARRAYs)
                inSByteNative = (SBYTE_ARRAY)Marshal.PtrToStructure(ptrInSByteNative, typeof(SBYTE_ARRAY));

                // copy data back to SByteArray classes
                inSByte.numOfBytes = inSByteNative.NumOfBytes;
                Marshal.Copy(inSByteNative.BytePtr, inSByte.data, 0, (int)inSByteNative.NumOfBytes);

                // release unmanaged memory
                Marshal.DestroyStructure(ptrInSByteNative, inSByteNative.GetType());
                Marshal.FreeHGlobal(ptrInSByteNative);
            }
        }

        /// <summary>
        /// use for DT_READ_CABLE_SERIAL_NUMBER, DT_IOCTL_GDP_BATT_INFO
        /// </summary>
        public void PassThruIoctl(uint channelId, IoctlId ioctlId, ref SByteArray outSByte)
        {
            if (outSByte.numOfBytes > outSByte.data.Length)
            {
                throw new Exception("PassThruIoctl, SByteArray (in) numOfBytes larger than allocated data buffer");
            }

            SBYTE_ARRAY outSByteNative = new SBYTE_ARRAY();
            outSByteNative.NumOfBytes = outSByte.numOfBytes;

            // allocate unmanaged memory for the byte arrays and copy data of arrays from managed to unmanaged
            outSByteNative.BytePtr = Marshal.AllocHGlobal(outSByte.data.Length);
            Marshal.Copy(outSByte.data, 0, outSByteNative.BytePtr, outSByte.data.Length);

            // allocate unmanaged memory and create pointer for the SBYTE_ARRAYs
            IntPtr ptrOutSByteNative = Marshal.AllocHGlobal(Marshal.SizeOf(outSByteNative));
            Marshal.StructureToPtr(outSByteNative, ptrOutSByteNative, true);

            // use try catch block so we can release the unmanaged memory whatever happens
            try
            {
                j2534Api.PassThruIoctl(channelId, ioctlId, IntPtr.Zero, ptrOutSByteNative);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                // copy unmanaged memory to managed struct (SBYTE_ARRAYs)
                outSByteNative = (SBYTE_ARRAY)Marshal.PtrToStructure(ptrOutSByteNative, typeof(SBYTE_ARRAY));

                // copy data back to SByteArray classes
                outSByte.numOfBytes = outSByteNative.NumOfBytes;
                Marshal.Copy(outSByteNative.BytePtr, outSByte.data, 0, (int)outSByteNative.NumOfBytes);

                // release unmanaged memory
                Marshal.DestroyStructure(ptrOutSByteNative, outSByteNative.GetType());
                Marshal.FreeHGlobal(ptrOutSByteNative);
            }
        }

        /// <summary>
        /// use for ETH_ACTIVATION_PULLUP
        /// </summary>
        public void PassThruIoctl(uint channelId, IoctlId ioctlId, int val)
        {
            IntPtr pInput = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(int)));
            Marshal.WriteInt32(pInput, (int)val);

            j2534Api.PassThruIoctl(channelId, ioctlId, pInput, IntPtr.Zero);
        }

        /*
        /// <summary>
        /// use for reading a string from FW, eg. DT_READ_CABLE_SERIAL_NUMBER
        /// specify number of bytes to allocate with numChars
        /// </summary>
        public void PassThruIoctl(uint channelId, IoctlId ioctlId, out string stringOut, int numChars)
        {
            StringBuilder strbOut = new StringBuilder(numChars);
            IntPtr ptrOut = Marshal.StringToHGlobalAnsi(strbOut.ToString());

            // use try catch block so we can release the unmanaged memory whatever happens
            try
            {
                j2534Api.PassThruIoctl(channelId, ioctlId, IntPtr.Zero, ptrOut);

                byte[] bytes = new byte[numChars];
                Marshal.Copy(ptrOut, bytes, 0, numChars);
                stringOut = Encoding.ASCII.GetString(bytes, 0, numChars);
            }
            catch (Exception)
            {
                throw;
            }
        }
        */

        private void CopyPassThruMsgToNative(ref PASSTHRU_MSG msgNative, PassThruMsg msg)
        {
            msgNative.ProtocolID = (uint)msg.protocolId;
            msgNative.RxStatus = msg.rxStatus;
            msgNative.TxFlags = msg.txFlags;
            msgNative.Timestamp = msg.timestamp;
            msgNative.DataLength = msg.dataLength;
            msgNative.ExtraDataIndex = msg.extraDataIndex;

            msgNative.DataBufferSize = (uint)msg.data.Length;
            msgNative.ptrDataBuffer = Marshal.AllocHGlobal((int)msgNative.DataBufferSize);
            Marshal.Copy(msg.data, 0, msgNative.ptrDataBuffer, msg.data.Length);
        }

        private void CopyPassThruMsgFromNative(ref PassThruMsg msg, PASSTHRU_MSG msgNative)
        {
            msg = new PassThruMsg(msgNative.DataBufferSize);
            msg.protocolId = (ProtocolId)msgNative.ProtocolID;
            msg.rxStatus = msgNative.RxStatus;
            msg.txFlags = msgNative.TxFlags;
            msg.timestamp = msgNative.Timestamp;
            msg.dataLength = msgNative.DataLength;
            msg.extraDataIndex = msgNative.ExtraDataIndex;

            msg.data = new byte[msgNative.DataLength];
            Marshal.Copy(msgNative.ptrDataBuffer, msg.data, 0, (int)msgNative.DataLength);
        }

        private SDevice CopySDeviceFromNative(SDEVICE sdev)
        {
            SDevice sd = new SDevice();

            sd.deviceName = sdev.DeviceName;
            sd.deviceAvailable = sdev.DeviceAvailable;
            sd.deviceDLLFWStatus = sdev.DeviceAvailable;
            sd.deviceConnectMedia = sdev.DeviceAvailable;
            sd.deviceConnectSpeed = sdev.DeviceAvailable;
            sd.deviceSignalQuality = sdev.DeviceAvailable;
            sd.deviceSignalStrength = sdev.DeviceAvailable;

            return sd;
        }

    }
}
