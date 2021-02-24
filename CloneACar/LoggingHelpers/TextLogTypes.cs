using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloneACar.LoggingHelpers
{
    public class TextLogTypes
    {
        public enum LogItemType
        {
            DEBUG,
            TRACE,
            WARNS,
            ERROR,
            EXEOK,
        }
    }

    public class MessageLogTypes
    {
        public enum MessageTypes
        {
            PT_WRITE,
            PT_READS,
            PT_MESSG,
            VIN_NUMB,
            CLNE_MSG,
            FAIL_MSG
        }
    }
}
