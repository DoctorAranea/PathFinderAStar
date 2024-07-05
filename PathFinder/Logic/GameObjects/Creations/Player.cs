using PathFinder.Logic.GameObjects.Abstract;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinder.Logic.GameObjects.Creations
{
    public class Player : Creation
    {
        public SolidBrush brush;

        private string sBoatFilePath = "player\\s_boat.png";
        private Bitmap sBoat;

        private string sCarFilePath = "player\\s_car.png";
        private Bitmap sCar;

        public Player(Color color, Point fieldPosition) : base(fieldPosition)
        {
            brush = new SolidBrush(color);
            if (File.Exists(sBoatFilePath)) sBoat = Image.FromFile(sBoatFilePath) as Bitmap;
            if (File.Exists(sCarFilePath)) sCar = Image.FromFile(sCarFilePath) as Bitmap;
        }

        public override void Draw(Graphics g)
        {
            Terrain terra = PFinder.GetTerrain(FieldPosition);
            switch (terra.GetType().Name.ToLower())
            {
                case "water":
                    {
                        if (sBoat != default)
                            g.DrawImage(sBoat, new Rectangle(RealPosition, new Size(PFinder.CELLSIZE, PFinder.CELLSIZE)));
                        else
                            g.FillRectangle(brush, new Rectangle(RealPosition, RealSize));
                    }
                    break;
                default:
                    {
                        {
                            if (sCar != default)
                                g.DrawImage(sCar, new Rectangle(RealPosition, new Size(PFinder.CELLSIZE, PFinder.CELLSIZE)));
                            else
                                g.FillEllipse(brush, new Rectangle(RealPosition, RealSize));
                        }
                    }
                    break;
            }
        }
    }
}
