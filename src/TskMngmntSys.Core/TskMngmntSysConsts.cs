using TskMngmntSys.Debugging;

namespace TskMngmntSys
{
    public class TskMngmntSysConsts
    {
        public const string LocalizationSourceName = "TskMngmntSys";

        public const string ConnectionStringName = "Default";

        public const bool MultiTenancyEnabled = true;


        /// <summary>
        /// Default pass phrase for SimpleStringCipher decrypt/encrypt operations
        /// </summary>
        public static readonly string DefaultPassPhrase =
            DebugHelper.IsDebug ? "gsKxGZ012HLL3MI5" : "a71bdfd583f040819c705a9b8d455f24";
    }
}
