using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rage;

namespace Save_Coord_RAGE
{
    public class EntryPoint
    {
        public static void Main()
        {
            GameFiber.StartNew(delegate
            {
                Game.LogTrivial("Save Coords Plugin loaded successfully");
                Game.DisplayNotification("CHAR_SOCIAL_CLUB", "CHAR_SOCIAL_CLUB", "Save Coord", "~g~Success", "Save Coord Plugin loaded ~g~successfully");
                Menu.CreateMenu();
            });
        }
    }
}
