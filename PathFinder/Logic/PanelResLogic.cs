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
    public static class PanelResLogic
    {
        internal static void PBox_PanelRes_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(Color.Black);

            g.DrawImage(PanelResBitmap, new Rectangle(0, 0, (sender as PictureBox).Width, (sender as PictureBox).Height));

            for (int i = 0; i < PFinder.Resources.Count; i++)
            {
                g.DrawImage(PFinder.Resources[i].Icon, new Rectangle(100 * i + 2, 2, 26, 26));
                g.DrawString(PFinder.Resources[i].Value.ToString(), new Font("Comic Sans MS", 14, FontStyle.Bold), new SolidBrush(Color.White), new Rectangle(100 * i + 30, 0, 100, PANELRES_HEIGHT));
            }

        }
    }
}
