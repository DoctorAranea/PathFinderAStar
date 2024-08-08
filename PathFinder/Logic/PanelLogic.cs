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

        private static List<PanelIcon> icons;
        private static Size iconSize = new Size(68, 68);
        private static Point iconOffset = new Point(CELLSIZE * FIELD_WIDTH / 2 - 3 * iconSize.Width, PANEL_HEIGHT / 2 - iconSize.Height);

        public static void InitializeIcons()
        {
            icons = new List<PanelIcon>();

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
                        iconSize.Width * column + iconOffset.X,
                        iconSize.Height * row + iconOffset.Y
                    ), iconSize);

                icons.Add(icon);
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

            for (int i = 0; i < icons.Count; i++)
            {
                PanelIcon icon = icons[i];
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
                Bitmap icon = SelectedObjects[SelectedObjectInPanel].Icon;
                if (icon == default)
                    icon = IconUnknown;

                g.DrawString(SelectedObjects[SelectedObjectInPanel].Name, new Font("Comic Sans MS", 20, FontStyle.Bold), new SolidBrush(Color.White), 20, 20);
                g.DrawImage(icon, new Rectangle(new Point
                    (
                        iconSize.Width * icons[SelectedObjectInPanel].column + iconOffset.X - selectingIconSize.Width,
                        iconSize.Height * icons[SelectedObjectInPanel].row + iconOffset.Y - selectingIconSize.Height
                    ), doneSelectedIconSize));
            }
        }

        internal static void PBox_MouseClick(object sender, MouseEventArgs e)
        {
            int selectedItemIndex = icons.FindIndex(x => x.rect.Contains(e.Location));
            if (selectedItemIndex >= 0 && selectedItemIndex < SelectedObjects.Count)
            {
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
            }
        }
    }
}
