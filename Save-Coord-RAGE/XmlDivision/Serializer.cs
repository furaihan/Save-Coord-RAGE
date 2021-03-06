﻿using Rage;
using Save_Coord_RAGE.CoordinateManager;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Save_Coord_RAGE.Menus;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Linq;

namespace Save_Coord_RAGE.XmlDivision
{
    internal class Serializer
    {
        internal static void SerializeItems(List<Vector3> locations, List<float> heading, string filename, string rootName)
        {
            if (string.IsNullOrEmpty(filename) || !filename.ToLower().EndsWith(".xml"))
            {
                Game.LogTrivial("File name is empty or invalid, please provide a valid file name");
                Game.DisplayNotification("CHAR_BLOCKED", "CHAR_BLOCKED", "Save Coord", "~r~Failed", "File name is ~r~empty or invalid~w~, please provide a ~g~valid~w~ file name");
                if (!filename.EndsWith(".xml")) { Game.HideHelp(); Game.DisplayHelp("Filename must have ~y~\".xml\"~w~ extension", 5000); }
                CoordManager.calculating = false;
                return;
            }
            if (locations.Count == 0 || heading.Count == 0) return;
            if (locations.Count != heading.Count)
            {
                Game.LogTrivial($"Error while trying to export an XML file. List heading and vector3 counts are different");
                Game.DisplayNotification("CHAR_BLOCKED", "CHAR_BLOCKED", "Save Coord", "~r~Failed", "Error while trying to export an XML file");
                CoordManager.calculating = false;
                return;
            }
            try
            {
                List<Coordinate> Coords = new List<Coordinate>();
                CoordManager.calculating = true;
                for (int i = 0; i < locations.Count; i++)
                {
                    Coordinate coord = new Coordinate(); Vector3 v = locations[i]; float h = heading[i];
                    coord.AxisX = (float)Math.Round(v.X, 2);
                    coord.AxisY = (float)Math.Round(v.Y, 2);
                    coord.AxisZ = (float)Math.Round(v.Z, 2);
                    coord.Heading = (float)Math.Round(h, 3);
                    if (XmlMenu.useStreetName.Checked) coord.NearestStreet = World.GetStreetName(v);
                    if (XmlMenu.useZoneName.Checked) coord.Zone = Alat.GetZoneName(v);
                    Coords.Add(coord);
                    if (i % 75 == 0) GameFiber.Yield();
                    //Game.DisplaySubtitle($"~g~Save Coord~w~: Exporting to {filename} ({i}/{loc.Count - 1})");
                }
                if (XmlMenu.shuffle.Checked) Coords.Shuffle();
                LinqSerializer(Coords, filename, rootName);
            }
            catch (Exception e)
            {
                Game.LogTrivial($"Error while trying to export an XML file. {e.Message}");
                Game.DisplayNotification("CHAR_BLOCKED", "CHAR_BLOCKED", "Save Coord", "~r~Failed", "Error while trying to export an XML file");
            }
            finally
            {
                CoordManager.calculating = false;
            }
        }
        internal static void Serialize(List<Coordinate> list, string filename)
        {
            var path = Path.Combine(@"Plugins/Save Coord/XML Export/", filename);
            if (string.IsNullOrEmpty(filename) || !filename.ToLower().EndsWith(".xml"))
            {
                Game.LogTrivial("File name is empty or invalid, please provide a valid file name");
                Game.DisplayNotification("CHAR_BLOCKED", "CHAR_BLOCKED", "Save Coord", "~r~Failed", "File name is ~r~empty or invalid~w~, please provide a ~g~valid~w~ file name");
                if (!filename.EndsWith(".xml")) { Game.HideHelp(); Game.DisplayHelp("Filename must have ~y~\".xml\"~w~ extension", 5000); }
                return;
            }
            try
            {
                CoordManager.calculating = true;
                XmlSerializer serializer = new XmlSerializer(typeof(List<Coordinate>));
                using (TextWriter textWriter = new StreamWriter(path, false))
                {
                    serializer.Serialize(textWriter, list);
                }
                Game.DisplayNotification("DESKTOP_PC", "FOLDER", "Save Coord", "~g~Success", $"Exported to XML Successfully~n~~y~Coordinate Count~s~: {list.Count}~n~~y~File Name~s~: {filename}");
            }
            catch (Exception e)
            {
                Game.LogTrivial($"Error while trying to export an XML file. {e.Message}");
                Game.DisplayNotification("CHAR_BLOCKED", "CHAR_BLOCKED", "Save Coord", "~r~Failed", "Error while trying to export an XML file");
            }
            finally
            {
                CoordManager.calculating = false;
                XmlMenu.xmlMenu.Close(false);
            }
        }
        internal static void LinqSerializer(IEnumerable<Coordinate> coordinates, string filename, string rootName)
        {
            try
            {
                var path = Path.Combine("Plugins", "Save Coord", "XML Export", filename);
                XDocument xDocument = new XDocument(new XDeclaration("1.0", "utf-8", "yes"),
                                                    new XElement(rootName,
                                                        from coord in coordinates
                                                        select new XElement("Coordinate",
                                                            new XAttribute("X", coord.AxisX),
                                                            new XAttribute("Y", coord.AxisY),
                                                            new XAttribute("Z", coord.AxisZ),
                                                            new XElement("Heading", coord.Heading),
                                                            coord.Zone == null ? null : new XElement("Zone", coord.Zone),
                                                            coord.NearestStreet == null ? null : new XElement("NearestStreet", coord.NearestStreet))));
                xDocument.Save(path);
                Game.DisplayNotification("DESKTOP_PC", "FOLDER", "Save Coord", "~g~Success", $"Exported to XML Successfully~n~~y~Coordinate Count~s~: {coordinates.ToList().Count}~n~~y~File Name~s~: {filename}");
            }
            catch (Exception e)
            {
                Game.LogTrivial($"Error while trying to export an XML file. {e.Message}");
                Game.DisplayNotification("CHAR_BLOCKED", "CHAR_BLOCKED", "Save Coord", "~r~Failed", "Error while trying to export an XML file");
                e.ToString().ToLog();
            }
            finally
            {
                CoordManager.calculating = false;
            }
        }
    }
}
