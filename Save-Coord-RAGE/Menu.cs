using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
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
            mainMenu = new UIMenu("Save Coord", "Press confirm to save current coordinates")
            {
                MouseControlsEnabled = false,
                AllowCameraMovement = true
            };
            mainMenu.SetBannerType(Color.ForestGreen);
            _menuPool.Add(mainMenu);

            fileName = new UIMenuItem("Name: ")
            {
                BackColor = Color.White,
                ForeColor = Color.Black,
                HighlightedBackColor = Color.LightGray,
                HighlightedForeColor = Color.Black
            };
            useHeading = new UIMenuCheckboxItem("Use Heading", true)
            {
                Enabled = false
            };
            entityCoordinate = new UIMenuListScrollerItem<string>("Coordinates", "Choose beetwen your coordinates or your vehicle coordinates",
                new[] { "Player", "Player Vehicle" });
            confirmMenu = new UIMenuItem("Confirm", "Confirm your choice")
            {
                BackColor = Color.MidnightBlue,
                ForeColor = Color.White,
                HighlightedBackColor = Color.LightCyan,
                HighlightedForeColor = Color.Black,
                LeftBadge = UIMenuItem.BadgeStyle.Star
            };
            openLocationManager = new UIMenuItem("Coordinate Manager", "Choose this to manage your saved coordinates")
            {
                BackColor = Color.DarkRed,
                ForeColor = Color.White,
                HighlightedBackColor = Color.Plum,
                HighlightedForeColor = Color.Black,
                LeftBadge = UIMenuItem.BadgeStyle.SilverMedal
            };
            CoordinateManager.ManagerMenu.CreateCoordsManagerMenu();
            mainMenu.AddItems(fileName, useHeading, entityCoordinate, confirmMenu, openLocationManager);

            mainMenu.RefreshIndex();
            mainMenu.OnItemSelect += MenuHandler.ItemSelectHandler;
        }
    }
}
