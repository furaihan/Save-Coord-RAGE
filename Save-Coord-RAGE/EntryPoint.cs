using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rage;
using Rage.Attributes;

[assembly: Plugin("Save Coord", Description ="Save your current coordinates to a txt file", Author ="Furaihan")]
namespace Save_Coord_RAGE
{
    public class EntryPoint
    {
        public static void Main()
        {
            GameFiber.StartNew(delegate
            {
                Game.LogTrivial("Save Coord Plugin loaded successfully");
                Game.DisplayNotification("CHAR_SOCIAL_CLUB", "CHAR_SOCIAL_CLUB", "Save Coord", "~g~Success", "Save Coord Plugin loaded ~g~successfully");
                Alat.CheckDirectory();
                Menu.CreateMenu();
                MenuHandler.MenuLoop();
            });
        }
    }
}
