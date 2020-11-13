using RAGENativeUI;
using RAGENativeUI.Elements;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Save_Coord_RAGE.CoordinateManager
{
    internal class ManagerMenu
    {
        internal static UIMenu locationManager;
        internal static UIMenuListScrollerItem<string> locationGroupFile;
        internal static UIMenuItem refreshIndex;
        internal static UIMenuItem getNearestLocaionDistance;
        internal static UIMenuItem placeMarker;
        internal static UIMenuItem setRouteToNearest;
        internal static List<string> locationGroup;
        internal static UIMenuItem deleteAllBlips;
        internal static UIMenuItem openXmlMenu;
        internal static void CreateCoordsManagerMenu()
        {
            locationManager = new UIMenu("Coords Manager", "Manage your saved coordinates here")
            {
                ParentMenu = Menu.mainMenu,
                WidthOffset = 200,
                AllowCameraMovement = true,
                MouseControlsEnabled = false
            };
            locationManager.SetBannerType(Color.Maroon);
            Menu._menuPool.Add(locationManager);
            locationGroupFile = new UIMenuListScrollerItem<string>("Location Groups", "", locationGroup);
            refreshIndex = new UIMenuItem("Refresh", "Refresh current location group, useful if you have new location group but haven't show on list")
            {
                LeftBadge = UIMenuItem.BadgeStyle.Alert,
                BackColor = Color.Teal,
                ForeColor = Color.WhiteSmoke
            };
            getNearestLocaionDistance = new UIMenuItem("Get Nearest Location", "Get nearest location from selected location group");
            placeMarker = new UIMenuItem("Place Marker / Checkpoint", "Place a marker or a checkpoint on every location in selected location group (Coming Soon)")
            {
                Enabled = false
            };
            setRouteToNearest = new UIMenuItem("Enable Route on Nearest Location", "");
            openXmlMenu = new UIMenuItem("Export to XML", "Export your saved coordinates to an XML file")
            {
                LeftBadge = UIMenuItem.BadgeStyle.Heart
            };
            deleteAllBlips = new UIMenuItem("Delete Blips", "Delete all blips created by this plugin")
            {
                Enabled = false,
            };
            XmlDivision.XmlMenu.CreateXmlMenu();
            locationManager.AddItems(locationGroupFile, getNearestLocaionDistance, setRouteToNearest, placeMarker, refreshIndex, deleteAllBlips ,openXmlMenu);
            Menu.mainMenu.BindMenuToItem(locationManager, Menu.openLocationManager);
            locationManager.RefreshIndex();
            locationManager.OnItemSelect += MenuHandler.ItemSelectHandler;
        }
    }
}
