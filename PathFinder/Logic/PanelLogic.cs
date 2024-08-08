using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static PathFinder.Logic.PFinder;

namespace PathFinder.Logic
{
    public static class PanelLogic
    {
        public class PanelIcon
        {
            public Rectangle rect;
            public int row;
            public int column;
        }

        private static List<PanelIcon> selectionIcons;
        private static List<PanelIcon> abilitiesIcons;
        private static Size iconSize = new Size(68, 68);
        private static Point selectionIconsOffset = new Point(CELLSIZE * FIELD_WIDTH / 2 - 3 * iconSize.Width, PANEL_HEIGHT / 2 - iconSize.Height);
        private static Point abilitiesIconsOffset = new Point(CELLSIZE * FIELD_WIDTH / 2 + (int)(4.65 * iconSize.Width), PANEL_HEIGHT / 2 - iconSize.Height);

        public static void InitializeIcons()
        {
            selectionIcons = new List<PanelIcon>();

            int row = 0;
            int column = 0;
            for (int i = 0; i < 12; i++)
            {
                PanelIcon icon = new PanelIcon();

                if (i == 6)
                {
                    row++;
                    column = 0;
                }

                icon.row = row;
                icon.column = column;
                icon.rect = new Rectangle(new Point
                    (
                        iconSize.Width * column + selectionIconsOffset.X,
                        iconSize.Height * row + selectionIconsOffset.Y
                    ), iconSize);

                selectionIcons.Add(icon);
                column++;
            }

            abilitiesIcons = new List<PanelIcon>();

            row = 0;
            column = 0;
            for (int i = 0; i < 6; i++)
            {
                PanelIcon icon = new PanelIcon();

                if (i == 3)
                {
                    row++;
                    column = 0;
                }

                icon.row = row;
                icon.column = column;
                icon.rect = new Rectangle(new Point
                    (
                        iconSize.Width * column + abilitiesIconsOffset.X,
                        iconSize.Height * row + abilitiesIconsOffset.Y
                    ), iconSize);

                abilitiesIcons.Add(icon);
                column++;
            }
        }

        public static void PBox_Panel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(Color.Black);

            if (IconEmpty == null)
                return;

            g.DrawImage(PanelBitmap, new Rectangle(0, 0, (sender as PictureBox).Width, (sender as PictureBox).Height));

            Size selectingIconSize = new Size(5, 5);
            Size doneSelectedIconSize = new Size(iconSize.Width + selectingIconSize.Width * 2, iconSize.Height + selectingIconSize.Height * 2);

            for (int i = 0; i < selectionIcons.Count; i++)
            {
                PanelIcon icon = selectionIcons[i];
                Bitmap iconBitmap = IconEmpty;

                if (i < SelectedObjects.Count)
                {
                    iconBitmap = SelectedObjects[i].Icon;
                    if (iconBitmap == default)
                        iconBitmap = IconUnknown;
                }

                g.DrawImage(iconBitmap, icon.rect);
            }

            if (SelectedObjects.Count > 0)
            {
                Bitmap iconBitmap = SelectedObjects[SelectedObjectInPanel].Icon;
                if (iconBitmap == default)
                    iconBitmap = IconUnknown;

                g.DrawString(SelectedObjects[SelectedObjectInPanel].Name, new Font("Comic Sans MS", 20, FontStyle.Bold), new SolidBrush(Color.White), 20, 20);
                g.DrawImage(iconBitmap, new Rectangle(new Point
                    (
                        iconSize.Width * selectionIcons[SelectedObjectInPanel].column + selectionIconsOffset.X - selectingIconSize.Width,
                        iconSize.Height * selectionIcons[SelectedObjectInPanel].row + selectionIconsOffset.Y - selectingIconSize.Height
                    ), doneSelectedIconSize));

                for (int i = 0; i < abilitiesIcons.Count; i++)
                {
                    PanelIcon icon = abilitiesIcons[i];
                    iconBitmap = IconEmpty;

                    if (i < SelectedObjects[SelectedObjectInPanel].Commands.Count)
                    {
                        iconBitmap = SelectedObjects[SelectedObjectInPanel].Commands[i].Icon;
                        if (iconBitmap == default)
                            iconBitmap = IconUnknown;
                    }

                    g.DrawImage(iconBitmap, icon.rect);
                }

                if (SelectedAbilityInPanel >= 0)
                {
                    iconBitmap = SelectedObjects[SelectedObjectInPanel].Commands[SelectedAbilityInPanel].Icon;
                    if (iconBitmap == default)
                        iconBitmap = IconUnknown;

                    g.DrawImage(iconBitmap, new Rectangle(new Point
                        (
                            iconSize.Width * abilitiesIcons[SelectedAbilityInPanel].column + abilitiesIconsOffset.X - selectingIconSize.Width,
                            iconSize.Height * abilitiesIcons[SelectedAbilityInPanel].row + abilitiesIconsOffset.Y - selectingIconSize.Height
                        ), doneSelectedIconSize));
                }
            }
        }

        internal static void PBox_MouseClick(object sender, MouseEventArgs e)
        {
            int selectedItemIndex = selectionIcons.FindIndex(x => x.rect.Contains(e.Location));
            if (selectedItemIndex >= 0 && selectedItemIndex < SelectedObjects.Count)
            {
                SelectedAbilityInPanel = -1;
                if (SelectedObjectInPanel == selectedItemIndex)
                {
                    var selectedObject = SelectedObjects[selectedItemIndex];
                    SelectedObjectInPanel = 0;
                    SelectedObjects.Clear();
                    SelectedObjects.Add(selectedObject);
                    DrawPanel();
                    DrawMap();
                }
                else
                {
                    SelectedObjectInPanel = selectedItemIndex;
                    DrawPanel();
                }

                return;
            }

            selectedItemIndex = abilitiesIcons.FindIndex(x => x.rect.Contains(e.Location));
            if (selectedItemIndex >= 0 && selectedItemIndex < SelectedObjects[SelectedObjectInPanel].Commands.Count)
            {
                if (SelectedAbilityInPanel == selectedItemIndex)
                {
                    var selectedObject = SelectedObjects[SelectedObjectInPanel].Commands[selectedItemIndex];
                    SelectedAbilityInPanel = -1;
                    DrawPanel();
                }
                else
                {
                    SelectedAbilityInPanel = selectedItemIndex;
                    DrawPanel();
                }

                return;
            }
        }
    }
}
