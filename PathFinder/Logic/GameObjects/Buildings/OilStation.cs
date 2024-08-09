using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinder.Logic.GameObjects.Buildings
{
    public class OilStation : Building
    {
        public OilStation(Point fieldPosition, string customName = "") : base(fieldPosition, customName)
        {
            string iconFile = "icons\\icon_oilStation.png";
            if (File.Exists(iconFile))
                Icon = new Bitmap(iconFile);
        }

        public override string Name { get; protected set; } = "Нефтяная Вышка";
        public override string Description { get; protected set; } = "Строение, добывающее нефть из недр океана";
        public override Bitmap Icon { get; protected set; }

        protected override string BuildingBitmapFile { get; set; } = "oil_station\\s_oilStation.png";
        public override Bitmap BuildingBitmap { get; protected set; }

        protected override void Draw(Graphics g)
        {
            SolidBrush brush = new SolidBrush(Color.DarkRed);

            if (BuildingBitmap != null)
                DrawImage(g, BuildingBitmap);
            else
                DrawDefault(g);

            void DrawImage(Graphics g, Image img)
            {
                try
                {
                    g.DrawImage(img, new Rectangle(new Point(RealPosition.X, RealPosition.Y), new Size(PFinder.CELLSIZE, PFinder.CELLSIZE)));
                }
                catch { }
            }

            void DrawDefault(Graphics g)
            {
                try
                {
                    g.FillEllipse(brush, new Rectangle(new Point(RealPosition.X, RealPosition.Y), RealSize));
                }
                catch { }
            }
        }
    }
}
