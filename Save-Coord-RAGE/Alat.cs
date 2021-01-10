using Rage;
using Rage.Native;
using RAGENativeUI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Save_Coord_RAGE
{
    internal static class Alat
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
            bool keyboarInputCheck = NativeFunction.CallByHash<int>(0x0CF2B696BBF945AE) == 0;
            if (!keyboarInputCheck)
            {
                if (Game.IsKeyDown(key) && modifierKey == Keys.None) return true;
                if (Game.IsKeyDownRightNow(modifierKey) && Game.IsKeyDown(key)) return true;
            }
            return false;
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
                if (File.Exists(path))
                {
                    using (var tw = new StreamWriter(path, true))
                    {
                        tw.WriteLine(text);
                    }
                }
                Game.DisplayNotification("DESKTOP_PC", "FOLDER", "Save Coord", "~g~Success", "Your coordinate has been saved ~g~successfully");
            });
        }
        internal static string GetKeyboardInput(string textTitle, string boxText, int length)
        {
            NativeFunction.Natives.DISABLE_ALL_CONTROL_ACTIONS(2);

            NativeFunction.Natives.DISPLAY_ONSCREEN_KEYBOARD(true, textTitle, 0, boxText, 0, 0, 0, length);
            Game.DisplayHelp($"Press {FormatKeyBinding(Keys.None, Keys.Enter)} to commit changes\nPress {FormatKeyBinding(Keys.None, Keys.Escape)} to back", true);
            Game.DisplaySubtitle(textTitle, 900000);
            while (NativeFunction.CallByHash<int>(0x0CF2B696BBF945AE) == 0)
            {
                GameFiber.Yield();
            }
            NativeFunction.Natives.ENABLE_ALL_CONTROL_ACTIONS(2);
            Game.DisplaySubtitle("");
            Game.HideHelp();

            return NativeFunction.Natives.GET_ONSCREEN_KEYBOARD_RESULT<string>();
        }
        internal static string FormatKeyBinding(Keys modifierKey, Keys key)
            => modifierKey == Keys.None ? $"~{key.GetInstructionalId()}~" :
                                          $"~{modifierKey.GetInstructionalId()}~ ~+~ ~{key.GetInstructionalId()}~";
        internal static void CheckDirectory()
        {
            if (!Directory.Exists(@"Plugins/Save Coord"))
            {
                Directory.CreateDirectory(@"Plugins/Save Coord");
                Game.LogTrivial("Creating \"Plugins/Save Coord\" directory");
            }
            if (!Directory.Exists(@"Plugins/Save Coord/XML Export"))
            {
                Directory.CreateDirectory(@"Plugins/Save Coord/XML Export");
                Game.LogTrivial("Creating \"Plugins/Save Coord/XML Export\" directory");
            }
            if (Directory.Exists(@"Plugins/Save Coord") && Directory.Exists(@"Plugins/Save Coord/XML Export")) Game.LogTrivial("Directory check passed");
            if (!File.Exists(@"Plugins/SaveCoordConfig.ini"))
            {
                Game.LogTrivial("INI configuration file not found, using default value");
            }
            else Game.LogTrivial("INI Check Passed");
        }
        internal static string GetZoneName(this Vector3 pos)
        {           
            string gameName = NativeFunction.Natives.GET_NAME_OF_ZONE<string>(pos.X, pos.Y, pos.Z);
            return Game.GetLocalizedString(gameName);
        }
        internal static bool IsNumeric(this string s) => int.TryParse(s, out _);
        internal static bool IsByteNumeric(this string s) => byte.TryParse(s, out _);
        internal static void ToLog(this string msg) => Game.LogTrivial(msg);
        internal static int CreateCheckPoint(Vector3 location) => NativeFunction.Natives.CREATE_CHECKPOINT<int>(47, location.X, location.Y, location.Z, location.X, location.Y, location.Z, 3f, 0, 255, 0, 255, 0);
        internal static int CreateCheckPoint(Vector3 location, System.Drawing.Color color)
        {
            int r = Convert.ToInt32(color.R);
            int g = Convert.ToInt32(color.G);
            int b = Convert.ToInt32(color.B);
            int a = Convert.ToInt32(color.A);
            return NativeFunction.Natives.CREATE_CHECKPOINT<int>(47, location.X, location.Y, location.Z, location.X, location.Y, location.Z, 3f, r, g, b, a, 0);
        }
        internal static string GetColorHexForHUD(this string msg, Color color) => $"<font color=\"{ColorTranslator.ToHtml(color)}\">{msg}</font>";
        internal static void DeleteCheckPoint(int checkpointHandle) => NativeFunction.Natives.DELETE_CHECKPOINT(checkpointHandle);
        internal static void DeleteCheckPoints(params int[] checkPointHandle) => checkPointHandle.ToList().ForEach(cp => DeleteCheckPoint(cp));
        internal static Random Random = new Random(DateTime.UtcNow.Ticks.GetHashCode()); 
        internal static T GetRandomElement<T>(this IEnumerable<T> list)
        {
            var han = list.ToList();
            return han[Random.Next(0, han.Count - 1)];
        }
        internal static string AddSpacesToSentence(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return "";
            StringBuilder newText = new StringBuilder(text.Length * 2);
            newText.Append(text[0]);
            for (int i = 1; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]) && text[i - 1] != ' ')
                    newText.Append(' ');
                newText.Append(text[i]);
            }
            return newText.ToString();
        }
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                int k = Random.Next(n--);
                T temp = list[n];
                list[n] = list[k];
                list[k] = temp;
            }
        }
    }
}
