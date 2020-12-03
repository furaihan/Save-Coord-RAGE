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
                            Alat.OutputFile(toSave.Position, filename);
                        }
                        ManagerMenu.locationGroup = Alat.GetLocationGroups();
                        XmlMenu.locationToExport.Items = ManagerMenu.locationGroup;
                        Game.LogTrivial("Coordinate manager menu changed");
                        ManagerMenu.locationGroupFile.Items = ManagerMenu.locationGroup;
                        Game.LogTrivial("XML Export menu changed");
                        MainMenu.fileName.Items = ManagerMenu.locationGroup;
                    }
                    catch (Exception e)
                    {
                        Game.LogTrivial("Export Coordinates Error: " + e.Message);
                        Game.DisplayNotification("~r~Export Coordinates Error: ~w~" + e.Message);
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
                    MainMenu._menuPool.CloseAllMenus();
                    ManagerMenu.locationGroup = Alat.GetLocationGroups();
                    ManagerMenu.locationGroupFile.Items = ManagerMenu.locationGroup;
                }
                else if (selectedItem == ManagerMenu.getNearestLocaionDistance)
                {
                    if (!CoordManager.calculating)
                    {
                        MainMenu._menuPool.CloseAllMenus();
                        CoordManager.calculating = true;
                        //Game.DisplayNotification("CHAR_MP_DETONATEPHONE", "CHAR_MP_DETONATEPHONE", "Save Coord", "~y~Calculating...", "Calculating is in ~g~progress");
                        var toCount = CoordManager.GetVector3FromFile(ManagerMenu.locationGroupFile.SelectedItem);
                        CoordManager.GetNearestLocation(toCount, false);
                    }
                    else
                    {
                        MainMenu._menuPool.CloseAllMenus();
                        Game.LogTrivial("Another calculating process is running");
                        Game.DisplayNotification("CHAR_BLOCKED", "CHAR_BLOCKED", "Save Coord", "~r~Failed", "Another calculation process is running");
                    }
                }
                else if (selectedItem == ManagerMenu.setRouteToNearest)
                {
                    if (!CoordManager.calculating)
                    {
                        MainMenu._menuPool.CloseAllMenus();
                        CoordManager.calculating = true;
                        //Game.DisplayNotification("CHAR_MP_DETONATEPHONE", "CHAR_MP_DETONATEPHONE", "Save Coord", "~y~Calculating...", "Calculating is in ~g~progress");
                        var toCount = CoordManager.GetVector3FromFile(ManagerMenu.locationGroupFile.SelectedItem);
                        CoordManager.GetNearestLocation(toCount, true);
                    }
                    else
                    {
                        MainMenu._menuPool.CloseAllMenus();
                        Game.LogTrivial("Another calculating process is running");
                        Game.DisplayNotification("CHAR_BLOCKED", "CHAR_BLOCKED", "Save Coord", "~r~Failed", "Another calculation process is running");
                    }
                } else if (selectedItem == ManagerMenu.deleteAllBlips)
                {
                    blipExist = false;
                    MainMenu._menuPool.CloseAllMenus();
                } else if (selectedItem == ManagerMenu.deleteLocation)
                {
                    GameFiber.StartNew(delegate
                    {
                        filename = ManagerMenu.locationGroupFile.SelectedItem;
                        CoordManager.DeleteNearestLocation(filename);
                    });                    
                } else if (selectedItem == ManagerMenu.placeMarker)
                {
                    if (!CoordManager.calculating)
                    {
                        MainMenu._menuPool.CloseAllMenus();
                        filename = ManagerMenu.locationGroupFile.SelectedItem;
                        if (ManagerMenu.placeMarker.Text.ToLower().Contains("place"))
                        {
                            ManagerMenu.placeMarker.Text = "Delete Marker / Checkpoint";
                            CoordManager.CreateCheckPoint(filename);
                        }
                        else if (ManagerMenu.placeMarker.Text.ToLower().Contains("delete"))
                        {
                            ManagerMenu.placeMarker.Text = "Place Marker / Checkpoint";
                            CoordManager.checkPointActive = false;
                        }
                    }
                    else
                    {
                        MainMenu._menuPool.CloseAllMenus();
                        Game.LogTrivial("Another calculating process is running");
                        Game.DisplayNotification("CHAR_BLOCKED", "CHAR_BLOCKED", "Save Coord", "~r~Failed", "Another calculation process is running");
                    }
                }
            }
            else if (sender == XmlMenu.xmlMenu)
            {
                if (selectedItem == XmlMenu.confirmExport)
                {
                    MainMenu._menuPool.CloseAllMenus();
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
            }
        }
    }
}
