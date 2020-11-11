using Rage;
using Rage.Native;
using RAGENativeUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Save_Coord_RAGE
{
    internal class Alat
    {
        internal static List<string> GetLocationGroups()
        {
            List<string> ret = new List<string>();
            foreach (string s in Directory.GetFiles(@"Plugins/Save Coord/", "*.txt").Select(Path.GetFileName))
            {
                ret.Add(s);
            }
            return ret;
        }
        internal static bool CheckKey(Keys modifierKey, Keys key)
        {
            bool keyboarInputCheck = (NativeFunction.Natives.UPDATE_ONSCREEN_KEYBOARD<int>() == 0);
            if (!keyboarInputCheck)
            {
                if (Game.IsKeyDown(key) && modifierKey == Keys.None) return true;
                else if (Game.IsKeyDownRightNow(modifierKey) && Game.IsKeyDown(key)) return true;
                else return false;
            }
            else return false;
        }
        internal static void OutputFile(Vector3 position, string filename, float heading = 0)
        {
            GameFiber.StartNew(delegate
            {
                if (string.IsNullOrEmpty(filename) || !filename.EndsWith(".txt"))
                {
                    Game.LogTrivial("File name is empty or invalid, please provide a valid file name");
                    Game.DisplayNotification("CHAR_BLOCKED", "CHAR_BLOCKED", "Save Coord", "~r~Failed", "File name is ~r~empty or invalid~w~, please provide a ~g~valid~w~ file name");
                    if (!filename.EndsWith(".txt")) { Game.HideHelp(); Game.DisplayHelp("Filename must have ~b~\".txt\"~w~ extension", 5000); }
                    return;
                }
                string path = @"Plugins/Save Coord/" + filename;
                string xFloat = Convert.ToString(position.X);
                string yFloat = Convert.ToString(position.Y);
                string zFloat = Convert.ToString(position.Z);
                string text = $"({xFloat}, {yFloat}, {zFloat})";
                if (heading != 0) { text += $", {Convert.ToString(heading)}"; }
                if (!File.Exists(path))
                {
                    var file = File.Create(path);
                    file.Close();
                    Game.LogTrivial("After Create");
                    TextWriter tw = new StreamWriter(path);
                    tw.WriteLine("This file created in [" + DateTime.Now.ToString() + "]");
                    tw.Close();
                }
                else if (File.Exists(path))
                {
                    using (var tw = new StreamWriter(path, true))
                    {
                        tw.WriteLine(text);
                    }
                }
                Game.DisplayNotification("DESKTOP_PC", "FOLDER", "Save Coord", "~g~Success", "Your coordinate has been saved ~g~successfully");
            });
        }
        internal static class ClientKeyboardInput
        {
            public static string GetKeyboardInput(string textTitle, string boxText, int length)
            {
                NativeFunction.Natives.DISABLE_ALL_CONTROL_ACTIONS(2);

                NativeFunction.Natives.DISPLAY_ONSCREEN_KEYBOARD(true, textTitle, 0, boxText, 0, 0, 0, length);
                Game.DisplayHelp($"Press {FormatKeyBinding(Keys.None, Keys.Enter)} to commit changes\nPress {FormatKeyBinding(Keys.None, Keys.Escape)} to back", true);
                while (NativeFunction.Natives.UPDATE_ONSCREEN_KEYBOARD<int>() == 0)
                {
                    GameFiber.Yield();
                }
                NativeFunction.Natives.ENABLE_ALL_CONTROL_ACTIONS(2);
                Game.HideHelp();

                return NativeFunction.Natives.GET_ONSCREEN_KEYBOARD_RESULT<string>();
            }
            private static string FormatKeyBinding(Keys modifierKey, Keys key)
                => modifierKey == Keys.None ? $"~{key.GetInstructionalId()}~" :
                                              $"~{modifierKey.GetInstructionalId()}~ ~+~ ~{key.GetInstructionalId()}~";
        }
        internal static void CheckDirectory()
        {
            if (!Directory.Exists(@"Plugins/Save Coord"))
            {
                Directory.CreateDirectory(@"Plugins/Save Coord");
                Game.LogTrivial("Creating \"Plugins/Save Coord\" directory");
            }
            else if (!Directory.Exists(@"Plugins/Save Coord/XML Export"))
            {
                Directory.CreateDirectory(@"Plugins/Save Coord/XML Export");
                Game.LogTrivial("Creating \"Plugins/Save Coord/XML Export\" directory");
            }
            else Game.LogTrivial("Directory check passed");
            if (!File.Exists(@"Plugins/SaveCoordConfig.ini"))
            {
                string[] itu = new string[]
                {
                    "[Initialization]",
                    "OpenMenuKey=Q",
                    "OpenMenuModifier=LControlKey",
                    "CheckOtherPluginMenuVisibility=false",
                    "BoostPerformance=false"
                };
                var path = @"Plugins/SaveCoordConfig.ini";
                var file = File.Create(path);
                file.Close();
                TextWriter tw = new StreamWriter(path);
                foreach (string s in itu) tw.WriteLine(s);
                tw.Close();
            }
            else Game.LogTrivial("INI Check Passed");
        }
    }
}
