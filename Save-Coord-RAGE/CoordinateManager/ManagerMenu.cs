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
        internal static void CreateCoordsManagerMenu()
        {
            locationManager = new UIMenu("Coords Manager", "Manage your saved coordinates here")
            {
                ParentMenu = Menu.mainMenu,
                WidthOffset = 250,
                AllowCameraMovement = true,
                MouseControlsEnabled = false
            };
            Menu._menuPool.Add(locationManager);
            locationGroup = Alat.GetLocationGroups();
            locationGroupFile = new UIMenuListScrollerItem<string>("Location Groups", "", locationGroup);
            refreshIndex = new UIMenuItem("Refresh", "Refresh current location group, useful if you have new location group but haven't show on list")
            {
                LeftBadge = UIMenuItem.BadgeStyle.Alert,
                BackColor = Color.LightGray,
                ForeColor = Color.Black
            };
            getNearestLocaionDistance = new UIMenuItem("Get Nearest Location", "Get nearest location from selected location group");
            placeMarker = new UIMenuItem("Place Marker / Checkpoint", "Place a marker or a checkpoint on every location in selected location group (Coming Soon)")
            {
                Enabled = false
            };
            setRouteToNearest = new UIMenuItem("Enable Route on Nearest Location", "");
            locationManager.AddItems(locationGroupFile, getNearestLocaionDistance, setRouteToNearest, placeMarker, refreshIndex);
            Menu.mainMenu.BindMenuToItem(locationManager, Menu.openLocationManager);
            locationManager.RefreshIndex();
            locationManager.OnItemSelect += MenuHandler.ItemSelectHandler;
        }
    }
}
