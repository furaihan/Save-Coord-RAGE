using Rage;
using Rage.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Save_Coord_RAGE.CoordinateManager
{
    internal class CoordManager
    {
        internal static bool calculating = false;
        private static int readFileCount = 0;
        internal static List<Vector3> GetVector3FromFile(string filename)
        {
            try
            {
                calculating = true;
                List<Vector3> ret = new List<Vector3>();
                string path = @"Plugins/Save Coord/";
                var lines = File.ReadLines(path + filename);
                List<string> vectorData = new List<string>();
                int readvector = 0;
                Menu._menuPool.CloseAllMenus();
                foreach (string line in lines)
                {
                    readFileCount++;
                    if (line.StartsWith("("))
                    {
                        int start = line.IndexOf("(") + 1;
                        int end = line.IndexOf(")") - 1;
                        string result = line.Substring(start, end);
                        vectorData.Add(result);
                    }
                    Game.DisplaySubtitle($"Reading File {filename} ({readFileCount})");
                    if (Initialize.boostPerformance)
                        GameFiber.Yield();
                }
                foreach (string s in vectorData)
                {
                    readvector++;
                    List<float> vec = new List<float>(3);
                    string[] ploat = s.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string f in ploat)
                    {
                        float result = float.Parse(f, CultureInfo.InvariantCulture);
                        vec.Add(result);
                    }
                    ret.Add(new Vector3(vec[0], vec[1], vec[2]));
                    Game.DisplaySubtitle($"Converting to Vector3 Variable ({readvector}/{readFileCount})");
                    if (Initialize.boostPerformance)
                        GameFiber.Yield();
                }
                return ret;
            } catch (Exception e)
            {
                calculating = false;
                Game.LogTrivial($"Error reading a file {e.Message}");
                Game.DisplayNotification("CHAR_BLOCKED", "CHAR_BLOCKED", "Save Coord", "~r~Failed", "Error while reading a file");
                return null;
            }
        }
        internal static void GetNearestLocation(List<Vector3> listLocation, bool enableroute = false)
        {
            if (listLocation == null) return;
            Menu._menuPool.CloseAllMenus();
            try
            {
                GameFiber.StartNew(delegate
                {
                    float nearest = 0f;
                    Vector3 nearestVec = Vector3.Zero;
                    int count = 0;
                    Vector3 playerPos = Game.LocalPlayer.Character.Position;
                    foreach (Vector3 v in listLocation)
                    {
                        count++;
                        float dis = v.DistanceTo(playerPos);

                        if (count == 1)
                        {
                            nearest = dis;
                            nearestVec = v;
                        }
                        else
                        {
                            if (dis < nearest)
                            {
                                nearest = dis;
                                nearestVec = v;
                            }
                        }
                        Game.DisplaySubtitle($"Calculating Distance ({count}/{readFileCount})");
                        if (Initialize.boostPerformance)
                            GameFiber.Yield();
                    }
                    Game.LogTrivial("Done Calculating");
                    Game.DisplaySubtitle($"Nearest location detected in ~g~{Alat.GetZoneName(nearestVec)}~w~ near ~g~{World.GetStreetName(nearestVec)}", 8500);
                    calculating = false;
                    if (enableroute)
                    {
                        MenuHandler.blipExist = true;
                        ManagerMenu.deleteAllBlips.Enabled = true;
                        Blip blip = new Blip(nearestVec);
                        blip.Color = Color.LimeGreen;
                        blip.Scale = 0.75f;
                        blip.EnableRoute(Color.ForestGreen);
                        while (true)
                        {
                            GameFiber.Yield();
                            if (Game.LocalPlayer.Character.Position.DistanceTo(blip.Position) <= 8f || !MenuHandler.blipExist) break;
                        }
                        if (blip.Exists())
                        {
                            blip.DisableRoute();
                            blip.Delete();
                        }
                        MenuHandler.blipExist = false;
                        ManagerMenu.deleteAllBlips.Enabled = false;
                    }
                    if (!enableroute)
                        Game.DisplaySubtitle($"~g~Save Coord~w~: Nearest location distance is {Math.Round(nearest)} meters. " +
                            $"detected in ~g~{Alat.GetZoneName(nearestVec)}~w~ near ~g~{World.GetStreetName(nearestVec)}", 8500);
                    readFileCount = 0;
                    GameFiber.Hibernate();
                });
            }
            catch (Exception e)
            {
                calculating = false;
                Game.LogTrivial($"Error while calculating a distance {e.Message}");
                Game.DisplayNotification("CHAR_BLOCKED", "CHAR_BLOCKED", "Save Coord", "~r~Failed", "Error while calculating a distance");
            }
        }
        internal static Vector3 GetNearestLocation(string filename)
        {
            try
            {
                calculating = true;
                List<Vector3> ret = new List<Vector3>();
                string path = @"Plugins/Save Coord/";
                var lines = File.ReadLines(path + filename);
                List<string> vectorData = new List<string>();
                int readvector = 0;
                Menu._menuPool.CloseAllMenus();
                foreach (string line in lines)
                {
                    readFileCount++;
                    if (line.StartsWith("("))
                    {
                        int start = line.IndexOf("(") + 1;
                        int end = line.IndexOf(")") - 1;
                        string result = line.Substring(start, end);
                        vectorData.Add(result);
                    }
                    Game.DisplaySubtitle($"Reading File {filename} ({readFileCount})");
                    if (Initialize.boostPerformance)
                        GameFiber.Yield();
                }
                foreach (string s in vectorData)
                {
                    readvector++;
                    List<float> vec = new List<float>(3);
                    string[] ploat = s.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string f in ploat)
                    {
                        float result = float.Parse(f, CultureInfo.InvariantCulture);
                        vec.Add(result);
                    }
                    ret.Add(new Vector3(vec[0], vec[1], vec[2]));
                    Game.DisplaySubtitle($"Converting to Vector3 Variable ({readvector}/{readFileCount})");
                    if (Initialize.boostPerformance)
                        GameFiber.Yield();
                }
                int count = 0;
                var playerPos = Game.LocalPlayer.Character.Position;
                float nearest = 0f;
                Vector3 nearestVec = Vector3.Zero;
                nearestVec = (from x in ret orderby x.DistanceTo(playerPos) select x).FirstOrDefault();
                Game.LogTrivial($"Orderby Distance {Math.Round(nearestVec.DistanceTo(playerPos))}");
                foreach (Vector3 nearent in ret)
                {
                    count++;
                    float dis = nearent.DistanceTo(playerPos);

                    if (count == 1)
                    {
                        nearest = dis;
                        nearestVec = nearent;
                    }
                    else
                    {
                        if (dis < nearest)
                        {
                            nearest = dis;
                            nearestVec = nearent;
                        }
                    }
                }
                return nearestVec;
            }
            catch (Exception e)
            {
                Game.LogTrivial(e.Message);
                return Vector3.Zero;
            }
        }
        internal static List<float> GetHeadingFromFile(string filename)
        {
            string path = @"Plugins/Save Coord/"+ filename;
            var lines = File.ReadLines(path);
            List<float> headingList = new List<float>();
            foreach (string line in lines)
            {
                if (line.StartsWith("("))
                {
                    var result = line.Substring(line.IndexOf("),") + 2);
                    float.TryParse(result, NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out float resultf);
                    headingList.Add(resultf);
                }
            }
            return headingList;
        }
        internal static void DeleteNearestLocation(string filename)
        {
            try
            {
                int count = 0;
                Vector3 nearest = GetNearestLocation(filename);
                string path = @"Plugins/Save Coord/";
                var lines = File.ReadLines(path + filename);
                foreach (string line in lines)
                {
                    if (line.Contains(nearest.X.ToString()) && line.Contains(nearest.Y.ToString()) && line.Contains(nearest.Z.ToString()))
                    {
                        count++;
                        Game.DisplaySubtitle($"Nearest location found in {nearest.GetZoneName()} Line number: {lines.ToList().IndexOf(line)}", 10000);
                    }
                }
                Game.LogTrivial($"Count is {count}");
                calculating = false;
            }
            catch (Exception e)
            {
                calculating = false;
                Game.DisplayNotification("CHAR_BLOCKED", "CHAR_BLOCKED", "Save Coord", "~r~Failed", "An error occured");
                Game.LogTrivial("Delete nearest location failed, please check your log file");
                Game.LogTrivial(e.Message);
            }
        }
        internal static List<int> listCP = new List<int>();
        internal static bool checkPointActive = false;
        internal static void CreateCheckPoint(string filename)
        {
            ulong DOES_ENTITY_EXIST = 0x7239B21A38F536BA;
            GameFiber.StartNew(delegate
            {
                try
                {
                    Game.LogTrivial("Placing marker / checkpoint started");
                    checkPointActive = true;
                    int checkpoint = 0;
                    List<Vector3> locations = GetVector3FromFile(filename);
                    foreach (Vector3 location in locations)
                    {
                        GameFiber.Yield();
                        Game.DisplaySubtitle($"Placing marker / checkpoint ({locations.IndexOf(location) + 1}/{locations.Count})");
                        checkpoint = NativeFunction.Natives.CREATE_CHECKPOINT<int>(46, location.X, location.Y, location.Z, location.X, location.Y, location.Z, 3f, 0, 255, 0, 255, 0);
                        NativeFunction.Natives.SET_CHECKPOINT_CYLINDER_HEIGHT(checkpoint, 25f, 25f, 2f);
                        listCP.Add(checkpoint);
                    }
                    calculating = false;
                    var count = 0;
                    while (checkPointActive)
                    {
                        GameFiber.Yield();
                        count++;
                        if (count > 100000)
                        {
                            ManagerMenu.placeMarker.Text = "Place Marker / Checkpoint";
                            Game.LogTrivial("Force delete all checkpoint because too long");
                            break;
                        }
                    }
                    calculating = true;
                    foreach (int cp in listCP)
                    {
                        GameFiber.Yield();
                        Game.DisplaySubtitle($"Deleting marker / checkpoint ({listCP.IndexOf(cp) + 1}/{listCP.Count})");
                        if (NativeFunction.CallByHash<bool>(DOES_ENTITY_EXIST, cp)) Game.LogTrivial($"Yes, {cp} exist");
                        NativeFunction.Natives.DELETE_CHECKPOINT(cp);
                    }
                    calculating = false;
                    listCP = new List<int>();
                } catch (Exception e)
                {
                    if (listCP.Count > 0)
                    {
                        calculating = true;
                        foreach (int cp in listCP)
                        {
                            GameFiber.Yield();
                            Game.DisplaySubtitle($"Deleting marker / checkpoint ({listCP.IndexOf(cp) + 1}/{listCP.Count})");
                            if (NativeFunction.CallByHash<bool>(DOES_ENTITY_EXIST, cp)) Game.LogTrivial($"Yes, {cp} exist");
                            NativeFunction.Natives.DELETE_CHECKPOINT(cp);
                        }
                        calculating = false;
                    }
                    listCP = new List<int>();
                    Game.DisplayNotification("CHAR_BLOCKED", "CHAR_BLOCKED", "Save Coord", "~r~Failed", "An error occured while creating checkpoint");
                    Game.LogTrivial(e.Message);
                }
            });            
        }
    }
}
