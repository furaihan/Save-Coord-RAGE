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
            locationManager = new UIMenu("Coords Manager", "Manage your saved coordinates here");
            Menu._menuPool.Add(locationManager);
            locationGroup = Alat.GetLocationGroups();
            locationGroupFile = new UIMenuListScrollerItem<string>("Location Groups", "", locationGroup));
            locationManager.ParentMenu = Menu.mainMenu;
            locationManager.WidthOffset = 250;
            refreshIndex = new UIMenuItem("Refresh", "Refresh current location group, useful if you have new location group but haven't show on list");
            getNearestLocaionDistance = new UIMenuItem("Get Nearest Location", "Get nearest location from selected location group");
            placeMarker = new UIMenuItem("Place Marker / Checkpoint", "Place a marker or a checkpoint on every location in selected location group");
            setRouteToNearest = new UIMenuItem("Enable Route on Nearest Location", "");
            locationManager.AddItems(locationGroupFile, getNearestLocaionDistance, setRouteToNearest, placeMarker, refreshIndex);
            refreshIndex.LeftBadge = UIMenuItem.BadgeStyle.Alert;
            refreshIndex.BackColor = Color.LightGray;
            refreshIndex.ForeColor = Color.Black;

            locationManager.AllowCameraMovement = true;
            locationManager.MouseControlsEnabled = false;
            Menu.mainMenu.BindMenuToItem(locationManager, Menu.openLocationManager);
            locationManager.ParentMenu = Menu.mainMenu;
            locationManager.RefreshIndex();
            locationManager.OnItemSelect += MenuHandler.ItemSelectHandler;
        }
    }
}
