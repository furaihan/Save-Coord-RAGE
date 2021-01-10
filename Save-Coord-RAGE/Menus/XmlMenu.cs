using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Save_Coord_RAGE.Menus
{
    internal class XmlMenu
    {
        internal static UIMenu xmlMenu;
        internal static UIMenuItem confirmExport;
        internal static UIMenuListScrollerItem<string> locationToExport;
        internal static UIMenuCheckboxItem allowOverwrite;
        internal static UIMenuCheckboxItem useZoneName;
        internal static UIMenuCheckboxItem useStreetName;
        internal static UIMenuCheckboxItem shuffle;
        internal static void CreateXmlMenu()
        {
            xmlMenu = new UIMenu("XML Export", "Export your saved coordinates to an xml file")
            {
                ParentMenu = ManagerMenu.locationManager,
                WidthOffset = 200,
                MouseControlsEnabled = false,
                AllowCameraMovement = true,
                SubtitleBackgroundColor = Color.DarkGray
            };
            xmlMenu.SetBannerType(Color.Indigo);
            MainMenu._menuPool.Add(xmlMenu);
            locationToExport = new UIMenuListScrollerItem<string>("Location File to Export", "Choose a location file that you want to export", Alat.GetLocationGroups());
            locationToExport.IndexChanged += (item, oldIndex, newIndex) =>
            {
                confirmExport.Description = $"run the export process for the {locationToExport.SelectedItem} file";
                Game.LogTrivial($"Confirm menu description changed to {confirmExport.Description}");
            };
            useZoneName = new UIMenuCheckboxItem("Use Zone Name", true, "Show the name of zone in your XML file");
            useStreetName = new UIMenuCheckboxItem("Use Street Name", true, "If checked, the nearest street name will be added in your XML file");
            shuffle = new UIMenuCheckboxItem("Shuffle", false, "Shuffle the order of the lines in the xml file to be exported");
            allowOverwrite = new UIMenuCheckboxItem("Allow Overwrite on Export", false, "Allow exported location file to overwrite the exsiting if have the same name");
            confirmExport = new UIMenuItem("Confirm Selection", $"run the export process for the {locationToExport.SelectedItem} file")
            {
                BackColor = HudColor.BlueDark.GetColor(),
                ForeColor = Color.White,
                HighlightedBackColor = HudColor.BlueLight.GetColor(),
                HighlightedForeColor = Color.Black,
                LeftBadge = UIMenuItem.BadgeStyle.Star
            };
            xmlMenu.AddItems(locationToExport,useStreetName, useStreetName,shuffle, allowOverwrite, confirmExport);
            ManagerMenu.locationManager.BindMenuToItem(xmlMenu, ManagerMenu.openXmlMenu);
            xmlMenu.RefreshIndex();
            xmlMenu.OnItemSelect += MenuHandler.ItemSelectHandler;
        }
    }
}
