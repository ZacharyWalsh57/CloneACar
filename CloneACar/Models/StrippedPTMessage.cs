using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// V0404 API
using Minimal_J2534_0404;

// Data Helpers
using static CloneACar.LogicalHelpers.DataByteHelpers;

namespace CloneACar.Models
{
    /// <summary>
    /// Class of PT message but in string form.
    /// </summary>
    public class StrippedPTMessage
    {
        public ProtocolId Protocol;
        public string MessageData;
        public uint MessageLength;
        public uint MessageTxFlags;
        public uint MessageRxFlags;
        public uint MessageTimeStamp;
        public uint MessageExtraDataIndex;

        public StrippedPTMessage(PassThruMsg MessageToConvert)
        {
            Protocol = MessageToConvert.protocolId;
            MessageData = ConvertDataToString(MessageToConvert.data);
            MessageLength = MessageToConvert.dataLength;
            MessageTxFlags = MessageToConvert.txFlags;
            MessageRxFlags = MessageToConvert.rxStatus;
            MessageTimeStamp = MessageToConvert.timestamp;
            MessageExtraDataIndex = MessageToConvert.extraDataIndex;
        }
    }
}
