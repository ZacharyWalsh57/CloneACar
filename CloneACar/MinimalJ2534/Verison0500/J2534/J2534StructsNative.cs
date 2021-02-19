using System;
using System.Runtime.InteropServices;

namespace Minimal_J2534_0500
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi), Serializable]
    public struct PASSTHRU_MSG
    {
        public UInt32 ProtocolID;
        public UInt32 MsgHandle;
        public UInt32 RxStatus;
        public UInt32 TxFlags;
        public UInt32 Timestamp;
        public UInt32 DataLength;
        public UInt32 ExtraDataIndex;

        public IntPtr ptrDataBuffer;

        public UInt32 DataBufferSize;

        public PASSTHRU_MSG(uint bufferSize) : this()
        {
            DataBufferSize = bufferSize;
            ptrDataBuffer = Marshal.AllocHGlobal((int)bufferSize);
        }

        public void DeallocateDataBuffer()
        {
            Marshal.FreeHGlobal(ptrDataBuffer);
        }
    };

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi), Serializable]
    public struct SDEVICE
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
        public string DeviceName;
        public UInt32 DeviceAvailable;
        public UInt32 DeviceDLLFWStatus;
        public UInt32 DeviceConnectMedia;
        public UInt32 DeviceConnectSpeed;
        public UInt32 DeviceSignalQuality;
        public UInt32 DeviceSignalStrength;
    };

    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct RESOURCE_STRUCT
    {
        public UInt32 Connector;
        public UInt32 NumOfResources;
        public IntPtr ptrResourceList;
    };

    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct SCHANNELSET
    {
        public UInt32 ChannelCount;
        public UInt32 ChannelThreshold;
        public IntPtr ptrChannelList;
    };

    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct SCONFIG_LIST
    {
        public UInt32 NumOfParams;
        public IntPtr ConfigPtr;
    };

    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct SCONFIG
    {
        public UInt32 Parameter;
        public UInt32 Value;
    };

    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct SBYTE_ARRAY
    {
        public UInt32 NumOfBytes;
        public IntPtr BytePtr;
    };

    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct ISO15765_CHANNEL_DESCRIPTOR
    {
        public UInt32 LocalTxFlags;
        public UInt32 RemoteTxFlags;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public Byte[] LocalAddress;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public Byte[] RemoteAddress;

        public ISO15765_CHANNEL_DESCRIPTOR(UInt32 localTxFlags, UInt32 remoteTxFlags, Byte[] localAdd, Byte[] remoteAdd)
        {
            LocalTxFlags = localTxFlags;
            RemoteTxFlags = remoteTxFlags;
            LocalAddress = localAdd;
            RemoteAddress = remoteAdd;
        }
    };

    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct SPARAM_LIST
    {
        public UInt32 NumOfParameters;
        public IntPtr SParamPtr;
    };

    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Size = 12)]
    public struct SPARAM
    {
        public UInt32 Parameter;
        public UInt32 Value;
        public UInt32 Supported;
    };
}