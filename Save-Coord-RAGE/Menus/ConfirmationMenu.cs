using System.Drawing;
using RAGENativeUI;
using RAGENativeUI.Elements;

namespace Save_Coord_RAGE.Menus
{
    class ConfirmationMenu
    {
        internal static UIMenu DeleteLocation;
        internal static UIMenuItem yesDL;
        internal static UIMenuItem noDL;
        internal static void DeleteLocationConfirmation()
        {
            DeleteLocation = new UIMenu("", "ARE YOU SURE")
            {
                AllowCameraMovement = true,
                MouseControlsEnabled = false,
                SubtitleStyle = new TextStyle(TextFont.Monospace, Color.Black, 0.50f, TextJustification.Left),
                SubtitleBackgroundColor = Color.Azure,
            };
            DeleteLocation.RemoveBanner();
            yesDL = new UIMenuItem("YES, ABSOLUTELY")
            {
                BackColor = Color.FromArgb(120, HudColor.GreenDark.GetColor()),
                ForeColor = Color.WhiteSmoke,
                HighlightedBackColor = Color.FromArgb(125, HudColor.GreenLight.GetColor()),
                HighlightedForeColor = Color.Black
            };
            yesDL.Activated += (m, s) =>
            {
                CoordinateManager.CoordManager.DelConfirm = CoordinateManager.CoordManager.DeleteConfirmation.Confirmed;
                m.Close(false);
            };
            noDL = new UIMenuItem("NO, CANCEL")
            {
                BackColor = Color.FromArgb(120, HudColor.RedDark.GetColor()),
                ForeColor = Color.WhiteSmoke,
                HighlightedBackColor = Color.FromArgb(125, HudColor.RedLight.GetColor()),
                HighlightedForeColor = Color.Black
            };
            noDL.Activated += (m, s) =>
            {
                CoordinateManager.CoordManager.DelConfirm = CoordinateManager.CoordManager.DeleteConfirmation.Rejected;
                m.Close(false);
            };
            DeleteLocation.AddItems(yesDL, noDL);
            DeleteLocation.RefreshIndex();
            DeleteLocation.OnMenuOpen += DeleteLocation_OnMenuOpen;
            MainMenu._menuPool.Add(DeleteLocation);
        }

        private static void DeleteLocation_OnMenuOpen(UIMenu sender)
        {
            yesDL.Text = yesText.GetRandomElement();
            noDL.Text = noText.GetRandomElement();
        }

        private static readonly string[] yesText = {"YES", "YES, ABSOLUTELY", "YES, WHY DO YOU ASK?", "JUST DO IT", "YES, OF COURSE", "ABSOLUTELY YES", "JUST DELETE IT" };
        private static readonly string[] noText = { "NO", "NO CANCEL IT", "NO, CANCEL", "NO, CANCEL PLEASE", "NO, I ACCIDENTALLY PRESS THAT", "NO, CANCEL THAT PLEASE" };
    }
}
