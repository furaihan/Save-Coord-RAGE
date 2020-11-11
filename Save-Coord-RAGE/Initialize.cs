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

        public static readonly Keys menuKey = ini.ReadEnum<Keys>("Initializaton", "OpenMenuKey", Keys.Q);
        public static readonly Keys modifier = ini.ReadEnum<Keys>("Initializaton", "OpenMenuModifier", Keys.LControlKey);
        public static readonly bool checkVisibility = ini.ReadBoolean("Initializaton", "CheckOtherPluginMenuVisibility", false);
        public static readonly bool boostPerformance = ini.ReadBoolean("Initializaton", "BoostPerformance", false);
    }
}
