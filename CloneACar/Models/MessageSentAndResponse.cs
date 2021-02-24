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
    /// Contains a Sent and Received Message pair.
    /// </summary>
    public class StringMessagePairs
    {
        public ProtocolId Protocol;
        public string SendAddress;
        public string ReceiveAddress;

        public List<StrippedPTMessage> MessagesSent = new List<StrippedPTMessage>();
        public List<StrippedPTMessage> MessagesRead = new List<StrippedPTMessage>();


        public StringMessagePairs(PassThruMsg[] Sent, PassThruMsg[] Read)
        {
            Protocol = GetProtocol(Sent[0]);
            SendAddress = GetSendAddress(Sent[0]);
            ReceiveAddress = GetReadAddress(Read[0]);

            foreach (var Message in Sent) { MessagesSent.Add(new StrippedPTMessage(Message)); }
            foreach (var Message in Read) { MessagesRead.Add(new StrippedPTMessage(Message)); }
        }
        public StringMessagePairs(PassThruMsg Sent, PassThruMsg Read)
        {
            MessagesSent.Add(new StrippedPTMessage(Sent));
            MessagesRead.Add(new StrippedPTMessage(Read));

            Protocol = GetProtocol(Sent);
            SendAddress = GetSendAddress(Sent);
            ReceiveAddress = GetReadAddress(Read);
        }


        private ProtocolId GetProtocol(PassThruMsg Msg) { return Msg.protocolId; }
        private string GetSendAddress(PassThruMsg SendMsg) { return ConvertDataToString(SendMsg.data.Skip(2).Take(2).ToArray()); }
        private string GetReadAddress(PassThruMsg ReadMsg) { return ConvertDataToString(ReadMsg.data.Skip(2).Take(2).ToArray()); }
    }
}
