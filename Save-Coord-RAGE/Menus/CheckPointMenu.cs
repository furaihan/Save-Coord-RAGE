using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Reflection;
using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;
using Save_Coord_RAGE.CoordinateManager;

namespace Save_Coord_RAGE.Menus
{
    internal class CheckPointMenu
    {
        internal static UIMenu checkPointMenu;
        internal static UIMenuListScrollerItem<string> locationGroup;
        internal static UIMenuNumericScrollerItem<int> cpNumber;
        internal static UIMenuNumericScrollerItem<float> cpHeight;
        internal static UIMenuNumericScrollerItem<float> cpRadius;
        internal static UIMenuListScrollerItem<string> type;
        internal static UIMenuListScrollerItem<string> color;
        internal static UIMenuNumericScrollerItem<byte> rColor;
        internal static UIMenuNumericScrollerItem<byte> gColor;
        internal static UIMenuNumericScrollerItem<byte> bColor;
        internal static UIMenuNumericScrollerItem<byte> aColor;
        internal static UIMenuItem deleteCheckpoint;
        internal static UIMenuItem confirm;
        internal static List<UIMenuItem> argb = new List<UIMenuItem>()
        {
            rColor, gColor, bColor, aColor
        };

        internal static int cpAmount = 50;
        internal static float height = 35;
        internal static float radius = 2.5f;
        internal static CheckPoint.CheckPointType pointType = CheckPoint.CheckPointType.Cylinder3;
        internal static Color CpColor = Color.Green;
        internal static void CreateMenu()
        {            
            PropertyInfo[] colour = typeof(Color).GetProperties(BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.Public);
            List<string> colors = new List<string>();
            colour.ToList().ForEach(c =>
            {
                colors.Add(Alat.AddSpacesToSentence(c.Name));
            });
            colors.Add("Custom Color");

            List<string> cpTypes = new List<string>();
            Enum.GetNames(typeof(CheckPoint.CheckPointType)).ToList().ForEach(ty => cpTypes.Add(Alat.AddSpacesToSentence(ty)));

            checkPointMenu = new UIMenu("Place CheckPoint", "Place some Checkpoint on nearby locations")
            {
                WidthOffset = 200,
                AllowCameraMovement = true,
                MouseControlsEnabled = false,
                ParentMenu = ManagerMenu.locationManager,
                ParentItem = ManagerMenu.placeMarker,
                TitleStyle = new TextStyle(TextFont.HouseScript, Color.Aquamarine, 1.025f, TextJustification.Center)
        };
            checkPointMenu.SetBannerType(Color.MidnightBlue);

            locationGroup = new UIMenuListScrollerItem<string>("Location Group", "Select which location group to place checkpoint nearby", Alat.GetLocationGroups());

            cpNumber = new UIMenuNumericScrollerItem<int>("CheckPoint Amount", "Place checkpionts on 50 nearby location from player", 5, 60, 5)
            {
                Value = 50,
            };
            cpNumber.IndexChanged += (item, oldindex, newIndex) =>
            {
                cpNumber.Description = $"Place checkpionts on {cpNumber.Value} nearby location from player";
                cpAmount = cpNumber.Value;
            };

            cpHeight = new UIMenuNumericScrollerItem<float>("CheckPoint Height", "Adjust the height of the check point to be placed", 5, 100, 5);
            cpHeight.Value = 35;
            cpHeight.IndexChanged += (item, oldIOndex, newIndex) => height = cpHeight.Value;

            cpRadius = new UIMenuNumericScrollerItem<float>("CheckPoint Radius", "Adjust the radius of the check point to be placed", 1.5f, 10f, 0.5f);
            cpRadius.Value = 2.5f;
            cpRadius.IndexChanged += (item, oldIOndex, newIndex) => radius = cpRadius.Value;

            type = new UIMenuListScrollerItem<string>("CheckPoint Type", "Set the checkpoint icon", cpTypes);
            if (type.Items.Contains("Cylinder3")) type.SelectedItem = type.Items[type.Items.IndexOf("Cylinder3")];
            type.IndexChanged += (item, oldIOndex, newIndex) =>
            {
                if (Enum.TryParse(type.SelectedItem.Replace(" ", ""), out CheckPoint.CheckPointType selectedType)) pointType = selectedType;
                else { Game.DisplayNotification($"~r~CheckPoint Type parse error ==> {type.SelectedItem}"); Game.LogTrivial($"CheckPoint Type parsing error {type.SelectedItem}"); }
            };

            rColor = new UIMenuNumericScrollerItem<byte>("Red Value", "Adjust the red value of the color", 0, 255, 1)
            {
                Value = 0,
                BackColor = Color.FromArgb(120, Color.Red)
            }; 
            gColor = new UIMenuNumericScrollerItem<byte>("Green Value", "Adjust the green value of the color", 0, 255, 1)
            {
                Value = 0,
                BackColor = Color.FromArgb(120, Color.Green)
            }; 
            bColor = new UIMenuNumericScrollerItem<byte>("Blue Value", "Adjust the blue value of the color", 0, 255, 1)
            {
                Value = 0,
                BackColor = Color.FromArgb(120, Color.Blue)
            }; 
            aColor = new UIMenuNumericScrollerItem<byte>("Alpha Value", "Adjust the alpha value of the color", 0, 255, 1)
            {
                Value = 255,
                ForeColor = Color.Snow
            };
            color = new UIMenuListScrollerItem<string>("CheckPoint Color", "Set the color of the checkpoint that will be placed" +
                $". Press ~y~RControlKey~s~ + ~y~NumPad9~s~ to use custom color", colors);
            if (color.Items.Contains("Green")) color.SelectedItem = color.Items[color.Items.IndexOf("Green")];
            color.IndexChanged += (item, oldIOndex, newIndex) =>
            {
                bool refresh = false;
                if (color.SelectedItem == "Custom Color")
                {
                    if (checkPointMenu.MenuItems.Contains(confirm)) { checkPointMenu.RemoveItemAt(checkPointMenu.MenuItems.IndexOf(confirm)); }
                    if (checkPointMenu.MenuItems.Contains(deleteCheckpoint)) { checkPointMenu.RemoveItemAt(checkPointMenu.MenuItems.IndexOf(deleteCheckpoint)); }
                    checkPointMenu.RefreshIndex();
                    if (!checkPointMenu.MenuItems.Contains(rColor)) { checkPointMenu.AddItem(rColor); }
                    if (!checkPointMenu.MenuItems.Contains(gColor)) { checkPointMenu.AddItem(gColor); }
                    if (!checkPointMenu.MenuItems.Contains(bColor)) { checkPointMenu.AddItem(bColor); }
                    if (!checkPointMenu.MenuItems.Contains(aColor)) { checkPointMenu.AddItem(aColor); }
                    if (!checkPointMenu.MenuItems.Contains(deleteCheckpoint)) { checkPointMenu.AddItem(deleteCheckpoint); }
                    if (!checkPointMenu.MenuItems.Contains(confirm)) { checkPointMenu.AddItem(confirm); }
                    checkPointMenu.RefreshIndex();
                    checkPointMenu.CurrentSelection = checkPointMenu.MenuItems.IndexOf(color);
                    color.Description = "Customize your own checkpoint color, change the RGBA value below ~g~(Valid values are 0 - 255)~s~";                   
                }
                else if (color.SelectedItem != "Custom Color")
                {
                    if (checkPointMenu.MenuItems.Contains(rColor)) { checkPointMenu.RemoveItemAt(checkPointMenu.MenuItems.IndexOf(rColor)); refresh = true; }
                    if (checkPointMenu.MenuItems.Contains(gColor)) { checkPointMenu.RemoveItemAt(checkPointMenu.MenuItems.IndexOf(gColor)); refresh = true; }
                    if (checkPointMenu.MenuItems.Contains(bColor)) { checkPointMenu.RemoveItemAt(checkPointMenu.MenuItems.IndexOf(bColor)); refresh = true; }
                    if (checkPointMenu.MenuItems.Contains(aColor)) { checkPointMenu.RemoveItemAt(checkPointMenu.MenuItems.IndexOf(aColor)); refresh = true; }
                    if (refresh)
                    {
                        checkPointMenu.RefreshIndex();
                        checkPointMenu.CurrentSelection = checkPointMenu.MenuItems.IndexOf(color);
                        color.Description = "Set the color of the checkpoint that will be placed" +
                        $". Press ~y~RControlKey~s~ + ~y~NumPad9~s~ to use custom color";
                    }
                    if (Enum.TryParse(color.SelectedItem.Replace(" ", ""), true, out KnownColor outcolor))
                    {
                        CpColor = Color.FromKnownColor(outcolor);
                    }
                    else { Game.DisplayNotification($"~r~Color Parsing Error ==> {color.SelectedItem}"); Game.LogTrivial($"Color parsing error {color.SelectedItem}"); }
                }
            };

            deleteCheckpoint = new UIMenuItem("Delete All Available CheckPoint")
            {
                Enabled = false
            };

            confirm = new UIMenuItem("Confirm", "Confirm your selection and start placing checkpoint")
            {
                BackColor = Color.MidnightBlue,
                ForeColor = Color.Honeydew,
                LeftBadge = UIMenuItem.BadgeStyle.Star
            };
            CpColorMenu.Main();
            checkPointMenu.BindMenuToItem(CpColorMenu.colorMenu, color);
            checkPointMenu.AddItems(locationGroup, cpNumber, cpHeight, cpRadius, type, color, deleteCheckpoint , confirm);
            checkPointMenu.RefreshIndex();

            MainMenu._menuPool.Add(checkPointMenu);
            ManagerMenu.locationManager.BindMenuToItem(checkPointMenu, ManagerMenu.placeMarker);
            checkPointMenu.OnItemSelect += MenuHandler.ItemSelectHandler;
            checkPointMenu.OnMenuOpen += CheckPointMenu_OnMenuOpen;
            //checkPointMenu.MenuItems.ForEach(a => a.Text.ToLog());
        }

        private static void CheckPointMenu_OnMenuOpen(UIMenu sender)
        {
            GameFiber.StartNew(() =>
            {
                while(checkPointMenu.Visible)
                {
                    GameFiber.Yield();
                    if (Alat.CheckKey(System.Windows.Forms.Keys.RControlKey, System.Windows.Forms.Keys.NumPad9))
                    {
                        "Custom color keys press detected".ToLog();
                        if (color.Items.Contains("Custom Color"))
                        {
                            if (color.SelectedItem != "Custom Color")
                                color.SelectedItem = "Custom Color";
                        }
                        else
                        {
                            Game.LogTrivial("Custom Color doesn't exist");
                        }
                        GameFiber.Sleep(2500);
                    }
                }
            });
        }
    }
}
