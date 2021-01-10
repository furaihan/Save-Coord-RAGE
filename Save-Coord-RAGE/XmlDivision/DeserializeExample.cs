using Rage;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Save_Coord_RAGE.XmlDivision
{
    class DeserializeExample
    {
        /// <summary>
        /// Deserialize an xml file. Before deserialization you must create a new class called coordinate
        /// see the example at Coordinate.cs file
        /// </summary>
        /// <param name="filename">filename to deserialize</param>
        /// <returns>Return a list of <see cref="Coordinate"></returns>
        public static List<Coordinate> Deserialize(string filename)
        {
            XmlSerializer xml = new XmlSerializer(typeof(List<Coordinate>));
            TextReader reader = new StreamReader(filename);
            object obj = xml.Deserialize(reader);
            List<Coordinate> XmlData = (List<Coordinate>)obj;
            reader.Close();
            return XmlData;
        }
        public static void GetDataFromXml(string filename, out List<Vector3> locations, out List<float> headings, out List<string> zones, out List<string> streets)
        {
            locations = new List<Vector3>();
            headings = new List<float>();
            zones = new List<string>();
            streets = new List<string>();
            var outVar = Deserialize(@"Plugins/Save Coord/XML Export" + filename);
            foreach (Coordinate c in outVar)
            {
                Vector3 location = new Vector3(c.AxisX, c.AxisY, c.AxisZ);
                float heading = c.Heading;
                string zone = c.Zone;
                string street = c.NearestStreet;
                locations.Add(location);
                headings.Add(heading);
                zones.Add(zone);
                streets.Add(street);
            }
        }
        public static void GetDataFromXml(string filename, out List<Vector3> locations, out List<float> headings)
        {
            locations = new List<Vector3>();
            headings = new List<float>();         
            var outVar = Deserialize(@"Plugins/Save Coord/XML Export" + filename);
            foreach (Coordinate c in outVar)
            {
                Vector3 location = new Vector3(c.AxisX, c.AxisY, c.AxisZ);
                float heading = c.Heading;
                locations.Add(location);
                headings.Add(heading);
            }
        }
    }
}
