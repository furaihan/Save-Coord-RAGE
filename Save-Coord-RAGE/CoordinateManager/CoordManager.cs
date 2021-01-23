using Rage;
using Rage.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Save_Coord_RAGE.Menus;

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
                MainMenu._menuPool.CloseAllMenus();
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
                readFileCount = 0;
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
            try
            {
                GameFiber.StartNew(delegate
                {
                    Vector3 nearestVec = Vector3.Zero;
                    Vector3 playerPos = Game.LocalPlayer.Character.Position;
                    nearestVec = (from x in listLocation orderby x.DistanceTo(playerPos) select x).FirstOrDefault();                 
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
                        Game.DisplaySubtitle($"~g~Save Coord~w~: Nearest location distance is {Math.Round(nearestVec.DistanceTo(Game.LocalPlayer.Character))} meters. " +
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
                string path = @"Plugins/Save Coord/" + filename;
                var lines = File.ReadLines(path);
                List<string> vectorData = new List<string>();
                int readvector = 0;
                MainMenu._menuPool.CloseAllMenus();
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
                    if (Initialize.boostPerformance)
                        GameFiber.Yield();
                }
                var playerPos = Game.LocalPlayer.Character.Position;
                Vector3 nearestVec = Vector3.Zero;
                nearestVec = (from x in ret orderby x.DistanceTo(playerPos) select x).FirstOrDefault();
                Game.LogTrivial($"Orderby Distance {Math.Round(nearestVec.DistanceTo(playerPos))}");
                calculating = false;
                return nearestVec;
            }
            catch (Exception e)
            {
                Game.LogTrivial("Get nearest location error");
                calculating = false;
                //Game.LogTrivial(e.Message);
                Game.LogTrivial(e.ToString());
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
                    if (float.TryParse(result, NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out float resultf))
                        headingList.Add(resultf);
                    else
                    {
                        Game.LogTrivial($"Float parse error Line:{lines.ToList().IndexOf(line)}");
                        Game.LogTrivial(line + " ==> " + result);
                    }
                }
            }
            return headingList;
        }
        internal enum DeleteConfirmation { Confirmed, Rejected, Unopened}
        internal static DeleteConfirmation DelConfirm = DeleteConfirmation.Unopened;
        internal static void DeleteNearestLocation(string filename)
        {
            GameFiber.StartNew(() =>
            {
                try
                {
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    StringBuilder output = new StringBuilder();
                    Vector3 nearest = GetNearestLocation(filename);
                    if (nearest == Vector3.Zero)
                    {
                        calculating = false;
                        DelConfirm = DeleteConfirmation.Unopened;
                        return;
                    }
                    if (nearest.DistanceTo(Game.LocalPlayer.Character) > 50f)
                    {
                        Game.DisplaySubtitle("~r~No nearest location found, ~g~make sure your distance to nearest location must be below 50 meters");
                        calculating = false;
                        DelConfirm = DeleteConfirmation.Unopened;
                        return;
                    }
                    Game.DisplaySubtitle("~y~The nearest location is marked with a ~g~checkpoint~y~, please ~g~confirm~y~ to ~r~delete~y~ that location", 80000);
                    var nCp = Alat.CreateCheckPoint(nearest, Color.DeepPink);
                    ConfirmationMenu.DeleteLocation.Visible = true;
                    while (true)
                    {
                        GameFiber.Yield();
                        if (DelConfirm == DeleteConfirmation.Rejected) { GameFiber.Sleep(300); break; }
                        if (DelConfirm == DeleteConfirmation.Confirmed) { GameFiber.Sleep(300); break; }
                        if (!ConfirmationMenu.DeleteLocation.Visible) { DelConfirm = DeleteConfirmation.Rejected; break; }
                        if (stopwatch.ElapsedMilliseconds > 120000)
                        {
                            DelConfirm = DeleteConfirmation.Rejected;
                            GameFiber.Sleep(300);
                            break;
                        }
                    }
                    if (DelConfirm == DeleteConfirmation.Rejected)
                    {
                        Game.DisplaySubtitle("~b~Okay, Cancelled", 8000);
                        DelConfirm = DeleteConfirmation.Unopened;
                        Alat.DeleteCheckPoint(nCp);
                        calculating = false;
                        return;
                    }
                    int count = 0;
                    string path = @"Plugins/Save Coord/" + filename;
                    foreach (string line in File.ReadLines(path))
                    {
                        count++;
                        if (line.Contains(nearest.X.ToString()) && line.Contains(nearest.Y.ToString()) && line.Contains(nearest.Z.ToString()))
                        {
                            Game.LogTrivial($"Nearest location detected in line number {count}");
                            continue;
                        }
                        output.Append(line + Environment.NewLine);
                    }
                    string result = output.ToString();
                    using (TextWriter tw = new StreamWriter(path, false))
                        tw.Write(result);
                    Game.DisplayNotification("DESKTOP_PC", "FOLDER", "Save Coord", "~g~Success", "Your ~y~Nearest~s~ coordinate has been ~o~deleted~s~ ~g~successfully");
                    Game.DisplayNotification($"~y~X~s~: {nearest.X}~n~~y~Y~s~: {nearest.Y}~n~~y~Z~s~: {nearest.Z}~n~~y~Zone Name~s~: {Alat.GetZoneName(nearest)}");
                    Alat.DeleteCheckPoint(nCp);
                }
                catch (Exception e)
                {
                    Game.DisplayNotification("CHAR_BLOCKED", "CHAR_BLOCKED", "Save Coord", "~r~Failed", "An error occured");
                    Game.LogTrivial("Delete nearest location failed, please check your log file");
                    Game.LogTrivial(e.Message);
                }
                finally
                {
                    DelConfirm = DeleteConfirmation.Unopened;
                    calculating = false;
                }
            });         
        }
        internal static List<int> listCP = new List<int>();
        internal static bool checkPointActive = false;       
        internal static void CreateCheckPoint(string filename, Color color, float radius, float height, int type, int number)
        {
            GameFiber.StartNew(delegate
            {
                try
                {
                    Game.LogTrivial("Placing marker / checkpoint started");
                    checkPointActive = true;
                    int checkpoint = 0;
                    List<Vector3> locations = GetVector3FromFile(filename);
                    int actualNumber = locations.Count > number ? number : locations.Count;
                    locations = (from x in locations orderby x.DistanceTo(Game.LocalPlayer.Character.Position) select x).Take(actualNumber).ToList();
                    foreach (Vector3 locationo in locations)
                    {
                        GameFiber.Yield();
                        Game.DisplaySubtitle($"Placing marker / checkpoint on {locations.Count} nearby locations ({locations.IndexOf(locationo) + 1}/{locations.Count})~n~" +
                            $"Checkpoint Color: {CheckPointMenu.color.SelectedItem.GetColorHexForHUD(color)}");
                        var location = locationo;
                        if (CheckPointMenu.placeOnGround.Checked)
                        {
                            float? zi = World.GetGroundZ(location, true, true);
                            if (zi.HasValue) location.Z = zi.Value;
                        }
                        checkpoint = NativeFunction.Natives.CREATE_CHECKPOINT<int>(type, location.X, location.Y, location.Z, location.X, location.Y, location.Z, radius, color.R, color.G, color.B, color.A, 0);
                        NativeFunction.Natives.SET_CHECKPOINT_CYLINDER_HEIGHT(checkpoint, height, height, radius);
                        listCP.Add(checkpoint);
                    }
                    calculating = false;
                    GameFiber.SleepUntil(() => !checkPointActive || Alat.CheckKey(Initialize.deleteCPModf, Initialize.deleteCPKey), Initialize.cpTimeout * 60000);
                    calculating = true;
                    foreach (int cp in listCP)
                    {
                        GameFiber.Yield();
                        Game.DisplaySubtitle($"Deleting marker / checkpoint ({listCP.IndexOf(cp) + 1}/{listCP.Count})");
                        NativeFunction.Natives.DELETE_CHECKPOINT(cp);
                    }
                    checkPointActive = false;
                    calculating = false;
                    listCP = new List<int>();
                }
                catch (ArgumentNullException e)
                {
                    e.ToString().ToLog();
                }
                catch (Exception e)
                {
                    if (listCP.Count > 0)
                    {
                        try
                        {
                            calculating = true;
                            foreach (int cp in listCP)
                            {
                                GameFiber.Yield();
                                Game.DisplaySubtitle($"Deleting marker / checkpoint ({listCP.IndexOf(cp) + 1}/{listCP.Count})");
                                NativeFunction.Natives.DELETE_CHECKPOINT(cp);
                            }
                        }
                        catch (Exception w)
                        {
                            Game.LogTrivial("Failed to delete all checkpoint");
                            Game.LogTrivial(w.Message);
                            Game.LogTrivial(w.ToString());
                        }
                    }
                    calculating = false;
                    Game.DisplayNotification("CHAR_BLOCKED", "CHAR_BLOCKED", "Save Coord", "~r~Failed", "There's an error while creating checkpoint");
                    Game.LogTrivial(e.Message);
                    Game.LogTrivial(e.ToString());
                }
                finally
                {
                    listCP = new List<int>();
                    calculating = false;
                    CheckPointMenu.confirm.Enabled = true;
                    CheckPointMenu.deleteCheckpoint.Enabled = checkPointActive;
                }
            });
        }
        internal static bool CarSpawned = false;
        internal static void SpawnCarOnNearestLocation(string filename)
        {
            GameFiber.StartNew(() =>
            {
                CarSpawned = true;
                var locs = GetVector3FromFile(filename);
                var headings = GetHeadingFromFile(filename);
                if (locs.Count != headings.Count)
                {
                    Game.DisplayNotification("Count is not same");
                    return;
                }
                Vector3 loc = (from x in locs orderby x.DistanceTo(Game.LocalPlayer.Character) select x).FirstOrDefault();
                float heading = headings[locs.IndexOf(loc)];
                var veh = new Vehicle(m => m.IsCar, loc, heading);
                var cp = Alat.CreateCheckPoint(veh.Position, Color.DarkMagenta);
                calculating = false;
                GameFiber.WaitUntil(() => (!veh.Exists() || Game.IsKeyDown(System.Windows.Forms.Keys.Subtract)), 1000000);
                if (veh.Exists()) veh.Dismiss();
                Alat.DeleteCheckPoint(cp);
                CarSpawned = false;
            });         
        }
    }
}
