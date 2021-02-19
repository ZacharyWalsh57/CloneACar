using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minimal_J2534_0404
{
    public class PassThruMsg
    {
        public ProtocolId protocolId;
        public uint rxStatus;
        public uint txFlags;
        public uint timestamp;
        public uint dataLength;
        public uint extraDataIndex;
        public byte[] data;

        public PassThruMsg(uint nbytes)
        {
            data = new byte[nbytes];
        }
    }

    public class SConfigList
    {
        public uint numOfParams;
        public List<SConfig> configList;

        public SConfigList()
        {
            configList = new List<SConfig>();
        }
    }

    public class SConfig
    {
        public ConfigParamId parameter;
        public uint value;

        public SConfig(ConfigParamId param)
        {
            parameter = param;
        }
    }

    public class SByteArray
    {
        public uint numOfBytes;
        public byte[] data;

        public SByteArray(uint numBytes)
        {
            numOfBytes = numBytes;
            data = new byte[numBytes];
        }
    }

    public class SDevice
    {
        public string deviceName;
        public uint deviceAvailable;
        public uint deviceDLLFWStatus;
        public uint deviceConnectMedia;
        public uint deviceConnectSpeed;
        public uint deviceSignalQuality;
        public uint deviceSignalStrength;

        // to control what shows up in the combobox
        public override string ToString()
        {
            return deviceName;
        }
    };

    public class ResourceStruct
    {
        public Connector connector;
        public uint numOfResources;
        public List<int> resourceList = new List<int>();
    }
}
