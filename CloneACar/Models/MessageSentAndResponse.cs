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
    public class ModuleCommunicationResults
    {
        public List<Tuple<PassThruMessageSet, PassThruMessageSet>> MessagePairs;
        private bool Initalized = false;

        private ProtocolId Protocol;
        public string SendAddressString;
        public string ReadAddressString;
        public byte[] SendAddressBytes;
        public byte[] ReadAddressBytes;

        public ModuleCommunicationResults() { }
        public ModuleCommunicationResults(PassThruMsg SentMsg, PassThruMsg ReadMsg)
        {
            var Sent = new PassThruMsg[1] { SentMsg };
            var Read = new PassThruMsg[1] { ReadMsg };

            Protocol = GetProtocol(Sent[0]);
            SendAddressString = GetAddressString(Sent[0]);
            SendAddressBytes = GetAddressBytes(Sent[0]);
            ReadAddressString = GetAddressString(Read[0]);
            ReadAddressBytes = GetAddressBytes(Read[0]);

            var SentSet = new PassThruMessageSet(Protocol, SendAddressBytes, Sent.ToList());
            var ReadSet = new PassThruMessageSet(Protocol, ReadAddressBytes, Read.ToList());

            MessagePairs = new List<Tuple<PassThruMessageSet, PassThruMessageSet>>();
            MessagePairs.Add(new Tuple<PassThruMessageSet, PassThruMessageSet>(SentSet, ReadSet));

            Initalized = true;
        }
        public ModuleCommunicationResults(PassThruMsg[] Sent, PassThruMsg[] Read)
        {
            Protocol = GetProtocol(Sent[0]);
            SendAddressString = GetAddressString(Sent[0]);
            SendAddressBytes = GetAddressBytes(Sent[0]);
            ReadAddressString = GetAddressString(Read[0]);
            ReadAddressBytes = GetAddressBytes(Read[0]);

            var SentSet = new PassThruMessageSet(Protocol, SendAddressBytes, Sent.ToList());
            var ReadSet = new PassThruMessageSet(Protocol, ReadAddressBytes, Read.ToList());

            MessagePairs = new List<Tuple<PassThruMessageSet, PassThruMessageSet>>();
            MessagePairs.Add(new Tuple<PassThruMessageSet, PassThruMessageSet>(SentSet, ReadSet));

            Initalized = true;
        }


        /// <summary>
        /// Adds message pair to tuple list item.
        /// </summary>
        /// <param name="SentMsg">Single sent PT message.</param>
        /// <param name="ReadMsg">Single Read PT Message.</param>
        /// <returns>True if added ok, false if not.</returns>
        public bool AddMessageTuple(PassThruMsg SentMsg, PassThruMsg ReadMsg)
        {
            var Sent = new PassThruMsg[1] { SentMsg };
            var Read = new PassThruMsg[1] { ReadMsg };

            return AddMessageTuple(Sent, Read);
        }
        /// <summary>
        /// Adds message pair to tuple list item.
        /// </summary>
        /// <param name="Sent">Array of sent PT messages.</param>
        /// <param name="Read">Array of Read PT Messages.</param>
        /// <returns>True if added ok, false if not.</returns>
        public bool AddMessageTuple(PassThruMsg[] Sent, PassThruMsg[] Read)
        {
            // Setup this instance if needed.
            if (!Initalized) { SetupInstance(Sent, Read); }

            // Make send and read message sets.
            var SentSet = new PassThruMessageSet(Protocol, SendAddressBytes, Sent.ToList());
            var ReadSet = new PassThruMessageSet(Protocol, ReadAddressBytes, Read.ToList());
            var NextSet = new Tuple<PassThruMessageSet, PassThruMessageSet>(SentSet, ReadSet);

            // Add to list of sets of messages.
            MessagePairs.Add(NextSet);
            return MessagePairs.Contains(NextSet);
        }


        /// <summary>
        /// Init this object to a new instance of this class type.
        /// </summary>
        /// <param name="Sent">Array of PT message Sent.</param>
        /// <param name="Read">Array of PT message Read.</param>
        private void SetupInstance(PassThruMsg[] Sent, PassThruMsg[] Read)
        {
            var SentAndResponse = new ModuleCommunicationResults(Sent, Read);

            this.MessagePairs = SentAndResponse.MessagePairs;
            this.Protocol = SentAndResponse.Protocol;
            this.SendAddressString = SentAndResponse.SendAddressString;
            this.SendAddressBytes = SentAndResponse.SendAddressBytes;
            this.ReadAddressString = SentAndResponse.ReadAddressString;
            this.ReadAddressBytes = SentAndResponse.ReadAddressBytes;

            this.Initalized = SentAndResponse.Initalized;
        }


        private ProtocolId GetProtocol(PassThruMsg Msg) { return Msg.protocolId; }
        private byte[] GetAddressBytes(PassThruMsg Msg)
        {
            if (Protocol == ProtocolId.ISO15765) return Msg.data.Skip(2).Take(2).ToArray();
            if (Protocol == ProtocolId.CAN) return Msg.data.Take(2).ToArray();

            // Default. But this is gonna change.
            return Msg.data.Skip(2).Take(2).ToArray();
        }
        private string GetAddressString(PassThruMsg Msg) { return ConvertDataToString(GetAddressBytes(Msg)); }
    }
}
