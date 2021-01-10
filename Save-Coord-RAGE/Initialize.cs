using Rage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Save_Coord_RAGE
{
    internal static class Initialize
    {
        public static readonly InitializationFile ini = new InitializationFile(@"Plugins/SaveCoordConfig.ini");

        public static readonly Keys menuKey = ini.ReadEnum("Initialization", "OpenMenuKey", Keys.Q);
        public static readonly Keys modifier = ini.ReadEnum("Initialization", "OpenMenuModifier", Keys.LControlKey);
        public static readonly bool checkVisibility = ini.ReadBoolean("Initialization", "CheckOtherPluginMenuVisibility", false);
        public static readonly bool boostPerformance = ini.ReadBoolean("Initialization", "BoostPerformance", false);
        public static readonly Keys deleteCPKey = ini.ReadEnum("Initialization", "DeleteCheckPointKey", Keys.Delete);
        public static readonly Keys deleteCPModf = ini.ReadEnum("Initialization", "DeleteCheckPointModifier", Keys.RShiftKey);
        public static readonly int cpTimeout = ini.ReadInt32("Initialization", "CheckPointTimeout", 10);
        internal static void IniValue()
        {
            Game.LogTrivial($"Open Menu Key : {menuKey}");
            Game.LogTrivial($"Modifier Key : {modifier}");
            Game.LogTrivial($"Check other plugin menu visibility {checkVisibility}");
            Game.LogTrivial($"Boost Performance : {boostPerformance}");
            $"DeleteCheckPointKey : {deleteCPKey}".ToLog();
            $"DeleteCheckPointModifier : {deleteCPModf}".ToLog();
            $"CheckPointTimeout : {cpTimeout}".ToLog();
        }
    }
}
