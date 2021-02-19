using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minimal_J2534_0500
{
    internal class AppException : Exception
    {
        public AppException()
        {
        }

        public AppException(string message)
                : base(message)
        {
        }

        public AppException(string message, Exception inner)
                : base(message, inner)
        {
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public string SimpleDescription()
        {
            return Message + " (" + TargetSite.Name + ")";
        }
    }
}
