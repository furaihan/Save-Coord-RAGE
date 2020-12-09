using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using RAGENativeUI;
using RAGENativeUI.Elements;
using Rage;

namespace Save_Coord_RAGE.Menus
{
    internal class CpColorMenu
    {
        internal static UIMenu colorMenu;
        internal static UIMenuItem searchItem;
        internal static void Main()
        {
            colorMenu = new UIMenu("Select Color", "Choose your color")
            {
                AllowCameraMovement = true,
                MouseControlsEnabled = false,
                WidthOffset = 200,
                ParentMenu = CheckPointMenu.checkPointMenu,
                ParentItem = CheckPointMenu.color,
                TitleStyle = new TextStyle(TextFont.HouseScript, Color.DarkSlateGray, 1.035f, TextJustification.Center)
            };
            colorMenu.SetBannerType(Color.MintCream);
            MainMenu._menuPool.Add(colorMenu);
            colorMenu.AddItem(searchItem = new UIMenuItem("SEARCH: "));
            CheckPointMenu.color.Items.ToList().ForEach(x =>
            {
                colorMenu.AddItem(new UIMenuItem(x));
            });
            colorMenu.RefreshIndex();
            colorMenu.OnItemSelect += MenuHandler.ItemSelectHandler;
            colorMenu.OnMenuClose += ColorMenu_OnMenuClose;
        }

        private static void ColorMenu_OnMenuClose(UIMenu sender)
        {
            var resetItems = colorMenu.MenuItems.ToList().Where(c => c != searchItem).ToList();
            resetItems.ForEach(i => colorMenu.RemoveItemAt(colorMenu.MenuItems.IndexOf(i)));
            colorMenu.RefreshIndex();
            searchItem.RightLabel = "";
            searchItem.Enabled = true;
            CheckPointMenu.color.Items.ToList().ForEach(x =>
            {
                colorMenu.AddItem(new UIMenuItem(x));
            });
            colorMenu.RefreshIndex();
        }
    }
}
