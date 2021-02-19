using System;
using System.Text;

namespace Minimal_J2534_0500
{
    internal class J2534Exception : Exception
    {
        public J2534Err j2534ErrorCode;
        public string lastErrorString = "";

        public J2534Exception()
        {
        }

        public J2534Exception(J2534Err code)
            : base()
        {
            j2534ErrorCode = code;
        }

        public J2534Exception(J2534Err code, StringBuilder lastError) //, J2534Device device)
            : base()
        {
            j2534ErrorCode = code;
            lastErrorString = lastError.ToString();
        }

        public J2534Exception(string message, J2534Err code)
            : base(message)
        {
            j2534ErrorCode = code;
        }

        public J2534Exception(string message, Exception inner, J2534Err code)
            : base(message, inner)
        {
            j2534ErrorCode = code;
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public string SimpleDescription()
        {
            return TargetSite.Name + " " + j2534ErrorCode.ToString();
        }
    }
}