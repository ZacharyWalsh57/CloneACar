namespace Minimal_J2534_0404
{
    internal class MiscConstants
    {
        public static int debug = 0;

        public const uint INVALID_ID = 0xFFFFFFFF;
        public const uint MAXNUMRXMSG50 = 50;
        public const uint MAXNUMANALSAMPLES = 100;

        public const uint MAX_READ_MSGS = 40;

        public const string ObserverSubjectEmpty = "ObserverSubjectEmpty";

        public enum TEST_RESULT
        {
            NOT_TESTED,
            FAIL,
            PASS
        }

        public const uint SHORT_TO_GROUND = 0xFFFFFFFE;
        public const uint VOLTAGE_OFF = 0xFFFFFFFF;
    }
}