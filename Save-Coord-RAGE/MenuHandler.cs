using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Save_Coord_RAGE.CoordinateManager;
using Save_Coord_RAGE.XmlDivision;
using Save_Coord_RAGE.Menus;
using System.IO;
using System.Drawing;

namespace Save_Coord_RAGE
{
    internal class MenuHandler
    {
        public static string filename = null;
        public static string xmlFileName = null;
        public static bool blipExist = false;
        internal static void ItemSelectHandler(UIMenu sender, UIMenuItem selectedItem, int index)
        {
            if (sender == MainMenu.mainMenu)
            {
                if (selectedItem == MainMenu.confirmMenu)
                {
                    filename = MainMenu.fileName.SelectedItem;
                    Entity toSave;
                    sender.Close();
                    try
                    {
                        toSave = Game.LocalPlayer.Character;
                        if (MainMenu.entityCoordinate.Index == 1 && Game.LocalPlayer.Character.CurrentVehicle.Exists())
                        {
                            toSave = Game.LocalPlayer.Character.CurrentVehicle;
                        }
                        if (MainMenu.useHeading.Checked)
                        {
                            Alat.OutputFile(toSave.Position, filename, toSave.Heading);
                        }
                        else
                        {
                            Alat.OutputFile(toSave.Position, filename, toSave.Heading);
                        }
                        List<string> before = new List<string>()
                        { MainMenu.fileName.SelectedItem, ManagerMenu.locationGroupFile.SelectedItem,XmlMenu.locationToExport.SelectedItem, CheckPointMenu.locationGroup.SelectedItem };
                        MainMenu.fileName.Items = Alat.GetLocationGroups();
                        ManagerMenu.locationGroupFile.Items = Alat.GetLocationGroups();
                        XmlMenu.locationToExport.Items = Alat.GetLocationGroups();
                        CheckPointMenu.locationGroup.Items = Alat.GetLocationGroups();
                        if (MainMenu.fileName.Items.Contains(before[0])) MainMenu.fileName.SelectedItem = before[0];
                        if (ManagerMenu.locationGroupFile.Items.Contains(before[1])) ManagerMenu.locationGroupFile.SelectedItem = before[1];
                        if (XmlMenu.locationToExport.Items.Contains(before[2])) XmlMenu.locationToExport.SelectedItem = before[2];
                        if (CheckPointMenu.locationGroup.Items.Contains(before[3])) CheckPointMenu.locationGroup.SelectedItem = before[3];
                    }
                    catch (Exception e)
                    {
                        Game.LogTrivial("Export Coordinates Error: " + e.Message);
                        Game.DisplayNotification("~r~Export Coordinates Error: ~w~" + e.Message);
                        CoordManager.calculating = false;
                    }
                }
                else if (selectedItem == MainMenu.fileName)
                {
                    filename = Alat.GetKeyboardInput("File Name:", "", 32);
                    MainMenu.fileName.Items.Add(filename);
                    MainMenu.fileName.SelectedItem = MainMenu.fileName.Items[MainMenu.fileName.Items.IndexOf(filename)];

                    MainMenu.mainMenu.RefreshIndex();
                }
            }
            else if (sender == ManagerMenu.locationManager)
            {
                if (selectedItem == ManagerMenu.refreshIndex)
                {
                    sender.Close(false);
                    ManagerMenu.locationGroup = Alat.GetLocationGroups();
                    ManagerMenu.locationGroupFile.Items = ManagerMenu.locationGroup;
                }
                else if (selectedItem == ManagerMenu.getNearestLocaionDistance)
                {
                    if (!CoordManager.calculating)
                    {
                        sender.Close(false);
                        CoordManager.calculating = true;
                        //Game.DisplayNotification("CHAR_MP_DETONATEPHONE", "CHAR_MP_DETONATEPHONE", "Save Coord", "~y~Calculating...", "Calculating is in ~g~progress");
                        var toCount = CoordManager.GetVector3FromFile(ManagerMenu.locationGroupFile.SelectedItem);
                        CoordManager.GetNearestLocation(toCount, false);
                    }
                    else
                    {
                        sender.Close(false);
                        Game.LogTrivial("Another calculating process is running");
                        Game.DisplayNotification("CHAR_BLOCKED", "CHAR_BLOCKED", "Save Coord", "~r~Failed", "Another calculation process is running");
                    }
                }
                else if (selectedItem == ManagerMenu.setRouteToNearest)
                {
                    if (!CoordManager.calculating)
                    {
                        sender.Close(false);
                        CoordManager.calculating = true;
                        //Game.DisplayNotification("CHAR_MP_DETONATEPHONE", "CHAR_MP_DETONATEPHONE", "Save Coord", "~y~Calculating...", "Calculating is in ~g~progress");
                        var toCount = CoordManager.GetVector3FromFile(ManagerMenu.locationGroupFile.SelectedItem);
                        CoordManager.GetNearestLocation(toCount, true);
                    }
                    else
                    {
                        sender.Close(false);
                        Game.LogTrivial("Another calculating process is running");
                        Game.DisplayNotification("CHAR_BLOCKED", "CHAR_BLOCKED", "Save Coord", "~r~Failed", "Another calculation process is running");
                    }
                } else if (selectedItem == ManagerMenu.deleteAllBlips)
                {
                    blipExist = false;
                    sender.Close(false);
                } else if (selectedItem == ManagerMenu.deleteLocation)
                {
                    GameFiber.StartNew(delegate
                    {
                        filename = ManagerMenu.locationGroupFile.SelectedItem;
                        CoordManager.DeleteNearestLocation(filename);
                    });                    
                } else if (selectedItem == ManagerMenu.placeMarker)
                {
                    //if (!CoordManager.calculating)
                    //{
                    //    sender.Close(false);
                    //    filename = ManagerMenu.locationGroupFile.SelectedItem;
                    //    if (ManagerMenu.placeMarker.Text.ToLower().Contains("place"))
                    //    {
                    //        ManagerMenu.placeMarker.Text = "Delete Marker / Checkpoint";
                    //        CoordManager.CreateCheckPoint(filename);
                    //    }
                    //    else if (ManagerMenu.placeMarker.Text.ToLower().Contains("delete"))
                    //    {
                    //        ManagerMenu.placeMarker.Text = "Place Marker / Checkpoint";
                    //        CoordManager.checkPointActive = false;
                    //    }
                    //}
                    //else
                    //{
                    //    sender.Close(false);
                    //    Game.LogTrivial("Another calculating process is running");
                    //    Game.DisplayNotification("CHAR_BLOCKED", "CHAR_BLOCKED", "Save Coord", "~r~Failed", "Another calculation process is running");
                    //}
                }
            }
            else if (sender == XmlMenu.xmlMenu)
            {
                if (selectedItem == XmlMenu.confirmExport)
                {
                    sender.Close(false);
                    xmlFileName = Alat.GetKeyboardInput("File Name (must ends with .xml)", "", 32);
                    if (File.Exists(@"Plugins/Save Coord/XML Export/" + xmlFileName) && !XmlMenu.allowOverwrite.Checked)
                    {
                        Game.DisplayNotification($"Export is ~r~aborted~w~, ~y~{xmlFileName}~w~ already exist");
                        return;
                    }
                    List<Vector3> vectorToExport = CoordManager.GetVector3FromFile(XmlMenu.locationToExport.SelectedItem);
                    List<float> headingToExport = CoordManager.GetHeadingFromFile(XmlMenu.locationToExport.SelectedItem);
                    Serializer.SerializeItems(vectorToExport, headingToExport, xmlFileName);
                }
            }
            else if (sender == CheckPointMenu.checkPointMenu)
            {
                if (selectedItem == CheckPointMenu.confirm)
                {
                    sender.Close(false);
                    $"Location Group = {CheckPointMenu.locationGroup.SelectedItem}".ToLog();
                    $"CheckPoint Amount = {CheckPointMenu.cpAmount}".ToLog();
                    $"CheckPoint Height = {CheckPointMenu.height}".ToLog();
                    $"CheckPoint Radius = {CheckPointMenu.radius}".ToLog();
                    $"CheckPoint Type = {CheckPointMenu.pointType} -> {(int)CheckPointMenu.pointType}".ToLog();
                    if (CheckPointMenu.color.SelectedItem == "Custom Color")
                    {
                        CheckPointMenu.CpColor = Color.FromArgb(CheckPointMenu.aColor.Value, CheckPointMenu.rColor.Value, CheckPointMenu.gColor.Value, CheckPointMenu.bColor.Value);
                    }
                    $"CheckPoint Color = {CheckPointMenu.CpColor}".ToLog();
                    var pos = Game.LocalPlayer.Character.Position + Game.LocalPlayer.Character.ForwardVector * 15f;
                    if (CoordManager.calculating)
                    {
                        Game.LogTrivial("Another calculating process is running");
                        Game.DisplayNotification("CHAR_BLOCKED", "CHAR_BLOCKED", "Save Coord", "~r~Failed", "Another calculation process is running");
                        return;
                    }
                    CoordManager.CreateCheckPoint(CheckPointMenu.locationGroup.SelectedItem, CheckPointMenu.CpColor, CheckPointMenu.radius, CheckPointMenu.height, (int)CheckPointMenu.pointType, CheckPointMenu.cpAmount);
                    CheckPointMenu.confirm.Enabled = false;
                    CheckPointMenu.confirm.Description = "~r~You must delete all available checkpoint before create another one";
                    CheckPointMenu.deleteCheckpoint.Enabled = true;
                }
                else if (selectedItem == CheckPointMenu.deleteCheckpoint)
                {
                    CoordManager.checkPointActive = false;
                    CheckPointMenu.confirm.Enabled = true;
                    CheckPointMenu.confirm.Description = "Confirm your selection and start placing checkpoint";
                    CheckPointMenu.deleteCheckpoint.Enabled = false;
                }
                else if (selectedItem == CheckPointMenu.rColor)
                {
                    var rstring = Alat.GetKeyboardInput("Type a number between 0 - 255", CheckPointMenu.rColor.Value.ToString(), 5);
                    if (int.TryParse(rstring, out int rValue))
                    {
                        if (rValue < 256 && rValue >= 0) { CheckPointMenu.rColor.Value = (byte)rValue; }
                        else { Game.DisplayNotification("~r~ERROR: The value must be between 0 - 255"); }
                    }
                    else { Game.DisplayNotification("~r~ERROR: You must type a number between 0 - 255 otherwise it will fails"); }
                }
                else if (selectedItem == CheckPointMenu.gColor)
                {
                    var gstring = Alat.GetKeyboardInput("Type a number between 0 - 255", CheckPointMenu.gColor.Value.ToString(), 5);
                    if (int.TryParse(gstring, out int gValue))
                    {
                        if (gValue < 256 && gValue >= 0) { CheckPointMenu.gColor.Value = (byte)gValue; }
                        else { Game.DisplayNotification("~r~ERROR: The value must be between 0 - 255"); }
                    }
                    else { Game.DisplayNotification("~r~ERROR: You must type a number between 0 - 255 otherwise it will fails"); }
                }
                else if (selectedItem == CheckPointMenu.bColor)
                {
                    var bstring = Alat.GetKeyboardInput("Type a number between 0 - 255", CheckPointMenu.bColor.Value.ToString(), 5);
                    if (int.TryParse(bstring, out int bValue))
                    {
                        if (bValue < 256 && bValue >= 0) { CheckPointMenu.bColor.Value = (byte)bValue; }
                        else { Game.DisplayNotification("~r~ERROR: The value must be between 0 - 255"); }
                    }
                    else { Game.DisplayNotification("~r~ERROR: You must type a number between 0 - 255 otherwise it will fails"); }
                }
                else if (selectedItem == CheckPointMenu.aColor)
                {
                    var astring = Alat.GetKeyboardInput("Type a number between 0 - 255", CheckPointMenu.aColor.Value.ToString(), 5);
                    if (int.TryParse(astring, out int aValue))
                    {
                        if (aValue < 256 && aValue >= 0) { CheckPointMenu.aColor.Value = (byte)aValue; }
                        else { Game.DisplayNotification("~r~ERROR: The value must be between 0 - 255"); }
                    }
                    else { Game.DisplayNotification("~r~ERROR: You must type a number between 0 - 255 otherwise it will fails"); }
                }
            }
            else if (sender == CpColorMenu.colorMenu)
            {
                if (selectedItem == CpColorMenu.searchItem)
                {
                    string query = Alat.GetKeyboardInput("Search a color", "", 30);
                    if (string.IsNullOrWhiteSpace(query))
                    {
                        return;
                    }
                    CpColorMenu.searchItem.RightLabel = query;
                    CpColorMenu.searchItem.Enabled = false;
                    var queryItems = CpColorMenu.colorMenu.MenuItems.ToList().Where(i => !i.Text.ToLower().Contains(query.ToLower()) && i != CpColorMenu.searchItem).ToList();
                    queryItems.ForEach(t =>
                    {
                        CpColorMenu.colorMenu.RemoveItemAt(CpColorMenu.colorMenu.MenuItems.IndexOf(t));
                    });
                    CpColorMenu.colorMenu.RefreshIndex();
                }
                else
                {
                    sender.Close(true);
                    if (CheckPointMenu.color.Items.Contains(selectedItem.Text)) CheckPointMenu.color.SelectedItem = selectedItem.Text;
                    else { "Selected color is not available in CheckPointMenu".ToLog(); $"Selected: {selectedItem.Text}".ToLog(); }
                }
            }
        }
        internal static void MenuLoop()
        {
            while (true)
            {
                GameFiber.Yield();
                MainMenu._menuPool.ProcessMenus();
                if (Alat.CheckKey(Initialize.modifier, Initialize.menuKey))
                {
                    if (MainMenu._menuPool.IsAnyMenuOpen()) MainMenu._menuPool.CloseAllMenus();
                    else if (Initialize.checkVisibility)
                    {
                        if (!UIMenu.IsAnyMenuVisible) MainMenu.mainMenu.Visible = true;
                        else if (UIMenu.IsAnyMenuVisible)
                        {
                            MainMenu.mainMenu.Visible = false;
                            Game.LogTrivial("Other plugin menu is already opened");
                        }
                    }
                    else if (!Initialize.checkVisibility) MainMenu.mainMenu.Visible = true;
                }
                if (Alat.CheckKey(System.Windows.Forms.Keys.NumPad7, System.Windows.Forms.Keys.NumPad4) && !CoordManager.CarSpawned)
                {
                    CoordManager.SpawnCarOnNearestLocation(ManagerMenu.locationGroupFile.SelectedItem);
                }
            }
        }
    }
}
