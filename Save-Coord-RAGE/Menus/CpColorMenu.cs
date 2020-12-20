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
        internal static List<UIMenuItem> colourUiItems = new List<UIMenuItem>();
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
                if (x.ToLower() != "custom color")
                {
                    var suc = Enum.TryParse(x.Replace(" ", ""), out KnownColor clr);
                    if (!suc)
                    {
                        Game.DisplayNotification($"{x} is invalid KnownColor");
                        $"{x} is invalid KnownColor".ToLog();
                        x.Replace(" ", "").ToLog();
                    }
                    Color fore = Color.FromKnownColor(clr);
                    UIMenuItem menu = new UIMenuItem(x, 
                        $"Color {x} with RGB Value: [R: {fore.R}; G: {fore.G}; B: {fore.B}].~n~ Hue: {fore.GetHue()}, Brightness: {fore.GetBrightness()}, Saturation: {fore.GetSaturation()}")
                    {
                        ForeColor = Color.FromKnownColor(clr),
                    };
                    menu.BackColor = Color.FromArgb(105, Color.Snow);
                    menu.HighlightedForeColor = menu.ForeColor;
                    menu.HighlightedBackColor = Color.FromArgb(95, Color.Honeydew);
                    GameFiber.Sleep(1);
                    colourUiItems.Add(menu);
                    colorMenu.AddItem(menu);
                }                
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
            colourUiItems.ForEach(m => colorMenu.AddItem(m));
            colorMenu.RefreshIndex();
        }
    }
}
