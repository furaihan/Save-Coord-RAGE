using Rage;
using Save_Coord_RAGE.CoordinateManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Save_Coord_RAGE.XmlDivision
{
    internal class Serializer
    {
        internal static void SerializeItems(List<Vector3> loc, List<float> heading, string filename)
        {
            if (loc == null || heading == null) return;
            if (loc.Count != heading.Count)
            {
                Game.LogTrivial($"Error while trying to export an XML file. List heading and vector3 are different");
                Game.DisplayNotification("CHAR_BLOCKED", "CHAR_BLOCKED", "Save Coord", "~r~Failed", "Error while trying to export an XML file");
                return;
            }
            try
            {
                GameFiber.StartNew(delegate
                {
                    List<Coordinate> Coords = new List<Coordinate>();
                    for (int i = 0; i < loc.Count; i++)
                    {
                        Coordinate coord = new Coordinate(); Vector3 v = loc[i]; float h = heading[i];
                        coord.AxisX = v.X;
                        coord.AxisY = v.Y;
                        coord.AxisZ = v.Z;
                        coord.Heading = h;
                        coord.NearestStreet = World.GetStreetName(v);
                        coord.Zone = Alat.GetZoneName(v);
                        Coords.Add(coord);
                        Game.DisplaySubtitle($"~g~Save Coord~w~: Exporting to {filename} ({i}/{loc.Count - 1})");
                        GameFiber.Yield();
                    }
                    Serialize(Coords, filename);
                    GameFiber.Sleep(10000);
                    GameFiber.Hibernate();
                });
            } catch (Exception e)
            {
                Game.LogTrivial($"Error while trying to export an XML file. {e.Message}");
                Game.DisplayNotification("CHAR_BLOCKED", "CHAR_BLOCKED", "Save Coord", "~r~Failed", "Error while trying to export an XML file");
                return;
            }
        }
        internal static void Serialize(List<Coordinate> list, string filename)
        {
            var path = @"Plugins/Save Coord/XML Export/" + filename;
            if (string.IsNullOrEmpty(filename) || !filename.ToLower().EndsWith(".xml"))
            {
                Game.LogTrivial("File name is empty or invalid, please provide a valid file name");
                Game.DisplayNotification("CHAR_BLOCKED", "CHAR_BLOCKED", "Save Coord", "~r~Failed", "File name is ~r~empty or invalid~w~, please provide a ~g~valid~w~ file name");
                if (!filename.EndsWith(".xml")) { Game.HideHelp(); Game.DisplayHelp("Filename must have ~y~\".xml\"~w~ extension", 5000); }
                return;
            }
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<Coordinate>));
                using (TextWriter textWriter = new StreamWriter(path, false))
                {
                    serializer.Serialize(textWriter, list);
                }
                Game.DisplayNotification("DESKTOP_PC", "FOLDER", "Save Coord", "~g~Success", "Exported to XML Successfully");
            } catch (Exception e)
            {
                Game.LogTrivial($"Error while trying to export an XML file. {e.Message}");
                Game.DisplayNotification("CHAR_BLOCKED", "CHAR_BLOCKED", "Save Coord", "~r~Failed", "Error while trying to export an XML file");
                return;
            }          
        }
    }
}
