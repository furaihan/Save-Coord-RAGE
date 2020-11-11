using Rage;
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
            if (loc == null) return;
            List<Coordinate> Coords = new List<Coordinate>();
            foreach (Vector3 v in loc)
            {
                Coordinate coord = new Coordinate();
            }
        }
        internal static void Serialize(List<Coordinate> list, string filename)
        {
            var path = @"Plugins/Save Coord/XML Export/" + filename;
            if (string.IsNullOrEmpty(filename) || !filename.EndsWith(".xml"))
            {
                Game.LogTrivial("File name is empty or invalid, please provide a valid file name");
                Game.DisplayNotification("CHAR_BLOCKED", "CHAR_BLOCKED", "Save Coord", "~r~Failed", "File name is ~r~empty or invalid~w~, please provide a ~g~valid~w~ file name");
                if (!filename.EndsWith(".xml")) { Game.HideHelp(); Game.DisplayHelp("Filename must have ~y~\".xml\"~w~ extension", 5000); }
                return;
            }
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<Coordinate>));
                using (TextWriter textWriter = new StreamWriter(path))
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
