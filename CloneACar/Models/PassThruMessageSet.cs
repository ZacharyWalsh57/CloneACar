using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Using call for the Minimal 0404 DLL
using Minimal_J2534_0404;
using Newtonsoft.Json;

// Globals
using static CloneACar.GlobalObjects;
using static CloneACar.LogicalHelpers.DataByteHelpers;
using CloneACar.Models;

namespace CloneACar.Models
{
    public class PassThruMessageSet
    {
        public ProtocolId Protocol;

        public byte[] AddressBytes;
        public string AddressString;

        public List<PassThruMsg> Messages = new List<PassThruMsg>();
        public int NumberOfMessages = 0;

        public PassThruMessageSet(ProtocolId Proc, byte[] Address, List<PassThruMsg> Messages)
        {
            Protocol = Proc;
            AddressBytes = Address;
            this.Messages = Messages;
            NumberOfMessages = this.Messages.Count;

            // 0x07 0xDF
            AddressString = ConvertDataToString(Address, true);
        }
    }
}
