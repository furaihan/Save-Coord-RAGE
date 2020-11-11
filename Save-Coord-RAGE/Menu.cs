using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Save_Coord_RAGE
{
    class Menu
    {
        internal static MenuPool _menuPool;
        internal static UIMenu mainMenu;

        internal static UIMenuItem fileName;
        internal static UIMenuCheckboxItem useHeading;
        internal static UIMenuListScrollerItem<string> entityCoordinate;
        internal static UIMenuItem confirmMenu;
        internal static UIMenuItem openLocationManager;
        internal static void CreateMenu()
        {
            _menuPool = new MenuPool();
            mainMenu = new UIMenu("Save Coords", "Press confirm to save current coordinates");
            _menuPool.Add(mainMenu);
            mainMenu.SetBannerType(Color.ForestGreen);

            mainMenu.MouseControlsEnabled = false;
            mainMenu.AllowCameraMovement = true;

            fileName = new UIMenuItem("Name: ");
            fileName.BackColor = Color.White;
            fileName.ForeColor = Color.Black;
            fileName.HighlightedBackColor = Color.LightGray;
            fileName.HighlightedForeColor = Color.Black;
            useHeading = new UIMenuCheckboxItem("Use Heading", true);
            entityCoordinate = new UIMenuListScrollerItem<string>("Coordinates", "Choose beetwen your coordinates or your vehicle coordinates",
                new[] { "Player", "Player Vehicle" });
            confirmMenu = new UIMenuItem("Confirm", "Confirm your choice");
            openLocationManager = new UIMenuItem("Coordinate Manager", "Choose this to manage your saved coordinates");
            openLocationManager.BackColor = Color.DarkRed;
            openLocationManager.ForeColor = Color.White;
            openLocationManager.HighlightedBackColor = Color.Plum;
            openLocationManager.HighlightedForeColor = Color.Black;
            CoordinateManager.ManagerMenu.CreateCoordsManagerMenu();
            mainMenu.AddItems(fileName, useHeading, entityCoordinate, confirmMenu, openLocationManager);

            mainMenu.RefreshIndex();
            mainMenu.OnItemSelect += MenuHandler.ItemSelectHandler;
        }
    }
}
