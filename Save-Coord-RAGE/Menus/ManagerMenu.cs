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
    internal class ManagerMenu
    {
        internal static UIMenu locationManager;
        internal static UIMenuListScrollerItem<string> locationGroupFile;
        internal static UIMenuItem refreshIndex;
        internal static UIMenuItem getNearestLocaionDistance;
        internal static UIMenuItem placeMarker;
        internal static UIMenuItem setRouteToNearest;
        internal static UIMenuItem shuffle;
        internal static UIMenuListScrollerItem<string> deleteLocation;
        internal static List<string> locationGroup;
        internal static UIMenuItem deleteAllBlips;
        internal static UIMenuItem openXmlMenu;
        internal static void CreateCoordsManagerMenu()
        {
            locationManager = new UIMenu("Coords Manager", "Manage your saved coordinates here")
            {
                ParentMenu = MainMenu.mainMenu,
                WidthOffset = 200,
                AllowCameraMovement = true,
                MouseControlsEnabled = false
            };
            locationManager.SetBannerType(Color.Maroon);
            MainMenu._menuPool.Add(locationManager);
            locationGroupFile = new UIMenuListScrollerItem<string>("Location Groups", "", locationGroup);
            locationGroupFile.IndexChanged += (item, oldindex, newindex) =>
            {
                getNearestLocaionDistance.Description = $"Get nearest location from {locationGroupFile.SelectedItem}";
                placeMarker.Description = $"Place or delete a marker or a checkpoint on every location in {locationGroupFile.SelectedItem}. This will open a new menu to fully customize your checkpoint";
                deleteLocation.Description = $"delete a location from {locationGroupFile.SelectedItem}";
            };
            refreshIndex = new UIMenuItem("Refresh", "Refresh current location group, select this if you have new location group but haven't show on list")
            {
                LeftBadge = UIMenuItem.BadgeStyle.Alert,
                BackColor = Color.Teal,
                ForeColor = Color.WhiteSmoke
            };
            getNearestLocaionDistance = new UIMenuItem("Get Nearest Location", $"Get nearest location from {locationGroupFile.SelectedItem}");
            placeMarker = new UIMenuItem("Place Marker / Checkpoint", $"Place or delete a marker or a checkpoint on every location in {locationGroupFile.SelectedItem}")
            {
                Enabled = true,
            };
            setRouteToNearest = new UIMenuItem("Enable Route on Nearest Location", "");
            shuffle = new UIMenuItem("Shuffle", "Shuffle selected location file lines");
            deleteLocation = new UIMenuListScrollerItem<string>("Delete Location", $"delete a location from {locationGroupFile.SelectedItem}", new[] { "Nearest", "Latest"});
            openXmlMenu = new UIMenuItem("Export to XML", "Export your saved coordinates to an XML file")
            {
                LeftBadge = UIMenuItem.BadgeStyle.Heart
            };
            deleteAllBlips = new UIMenuItem("Delete Blips", "Delete all blips created by this plugin")
            {
                Enabled = false,
            };
            XmlMenu.CreateXmlMenu();
            CheckPointMenu.CreateMenu();
            locationManager.AddItems(locationGroupFile, getNearestLocaionDistance, setRouteToNearest,shuffle, placeMarker, deleteLocation, refreshIndex, deleteAllBlips ,openXmlMenu);
            MainMenu.mainMenu.BindMenuToItem(locationManager, MainMenu.openLocationManager);
            locationManager.RefreshIndex();
            locationManager.OnItemSelect += MenuHandler.ItemSelectHandler;
        }
    }
}
