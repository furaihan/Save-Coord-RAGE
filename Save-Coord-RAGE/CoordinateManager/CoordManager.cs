using Rage;
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
        private static bool calculating = false;
        private static int readFileCount = 0;
        internal static List<Vector3> GetVector3FromFile(string filename)
        {
            calculating = true;
            List<Vector3> ret = new List<Vector3>();
            string path = @"Plugins/Save Coord/";
            var lines = File.ReadLines(path + filename);
            List<string> vectorData = new List<string>();
            int readvector = 0;
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
        }
        internal static void GetNearestLocation(List<Vector3> listLocation, bool enableroute = false)
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
                Game.DisplaySubtitle($"Nearest location detected in ~g~{Zones.GetZoneName(nearestVec)}~w~ near ~g~{World.GetStreetName(nearestVec)}");
                calculating = false;
                if (enableroute)
                {
                    Blip blip = new Blip(nearestVec);
                    blip.Color = Color.LimeGreen;
                    blip.Scale = 0.75f;
                    blip.EnableRoute(Color.ForestGreen);
                    while (true)
                    {
                        GameFiber.Yield();
                        if (Game.LocalPlayer.Character.Position.DistanceTo(blip.Position) <= 8f) break;
                    }
                    if (blip.Exists())
                    {
                        blip.DisableRoute();
                        blip.Delete();
                    }
                }
                if (!enableroute)
                    Game.DisplaySubtitle($"~g~RAGESaveCoords~w~: Nearest location distance is {Math.Round(nearest)} meters", 6000);
                readFileCount = 0;
                GameFiber.Hibernate();
            });
        }
    }
}
