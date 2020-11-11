﻿using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Save_Coord_RAGE.CoordinateManager;

namespace Save_Coord_RAGE
{
    internal class MenuHandler
    {
        public static string filename = null;
        internal static void ItemSelectHandler(UIMenu sender, UIMenuItem selectedItem, int index)
        {
            if (sender == Menu.mainMenu)
            {
                if (selectedItem == Menu.confirmMenu)
                {
                    Entity toSave;
                    sender.Close();
                    try
                    {
                        toSave = Game.LocalPlayer.Character;
                        if (Menu.entityCoordinate.Index == 1 && Game.LocalPlayer.Character.CurrentVehicle.Exists())
                        {
                            toSave = Game.LocalPlayer.Character.CurrentVehicle;
                        }
                        if (Menu.useHeading.Checked)
                        {
                            Alat.OutputFile(toSave.Position, filename, toSave.Heading);
                        }
                        else
                        {
                            Alat.OutputFile(toSave.Position, filename);
                        }
                    }
                    catch (Exception e)
                    {
                        Game.LogTrivial("Export Coordinates Error: " + e.Message);
                        Game.DisplayNotification("~r~Export Coordinates Error: ~w~" + e.Message);
                    }
                }
                else if (selectedItem == Menu.fileName)
                {
                    if (string.IsNullOrWhiteSpace(filename)) { filename = Alat.ClientKeyboardInput.GetKeyboardInput("File Name:", "MyCoords.txt", 32); }
                    else { filename = Alat.ClientKeyboardInput.GetKeyboardInput("File Name:", filename, 32); }
                    Menu.fileName.Text = "Name: " + filename;
                    Menu.mainMenu.RefreshIndex();
                }
            }
            if (sender == ManagerMenu.locationManager)
            {
                if (selectedItem == ManagerMenu.refreshIndex)
                {
                    sender.Close();
                    ManagerMenu.locationGroup = Alat.GetLocationGroups();
                    ManagerMenu.locationGroupFile.Items = ManagerMenu.locationGroup;
                }
                else if (selectedItem == ManagerMenu.getNearestLocaionDistance)
                {
                    if (!CoordManager.calculating)
                    {
                        Menu._menuPool.CloseAllMenus();
                        CoordManager.calculating = true;
                        Game.DisplayNotification("CHAR_MP_DETONATEPHONE", "CHAR_MP_DETONATEPHONE", "Save Coord", "~y~Calculating...", "Calculating is in ~g~progress");
                        var toCount = CoordManager.GetVector3FromFile(ManagerMenu.locationGroupFile.SelectedItem);
                        CoordManager.GetNearestLocation(toCount, false);
                    }
                    else
                    {
                        Game.LogTrivial("Another calculating process is running");
                        Game.DisplayNotification("CHAR_BLOCKED", "CHAR_BLOCKED", "Save Coord", "~r~Failed", "Another calculation process is running");
                    }
                }
                else if (selectedItem == ManagerMenu.setRouteToNearest)
                {
                    if (!CoordManager.calculating)
                    {
                        Menu._menuPool.CloseAllMenus();
                        CoordManager.calculating = true;
                        Game.DisplayNotification("CHAR_MP_DETONATEPHONE", "CHAR_MP_DETONATEPHONE", "Save Coord", "~y~Calculating...", "Calculating is in ~g~progress");
                        var toCount = CoordManager.GetVector3FromFile(ManagerMenu.locationGroupFile.SelectedItem);
                        CoordManager.GetNearestLocation(toCount, true);
                    }
                    else
                    {
                        Game.LogTrivial("Another calculating process is running");
                        Game.DisplayNotification("CHAR_BLOCKED", "CHAR_BLOCKED", "Save Coord", "~r~Failed", "Another calculation process is running");
                    }
                }
            }
        }
        internal static void MenuLoop()
        {
            while (true)
            {
                GameFiber.Yield();
                Menu._menuPool.ProcessMenus();
                if (Alat.CheckKey(Initialize.modifier, Initialize.menuKey))
                {
                    if (Menu._menuPool.IsAnyMenuOpen()) Menu._menuPool.CloseAllMenus();
                    else if (Initialize.checkVisibility)
                    {
                        if (!UIMenu.IsAnyMenuVisible) Menu.mainMenu.Visible = true;
                        else if (UIMenu.IsAnyMenuVisible) Menu.mainMenu.Visible = false; Game.LogTrivial("Other plugin menu is already opened");
                    }
                    else if (!Initialize.checkVisibility) Menu.mainMenu.Visible = true;
                }
            }
        }
    }
}
