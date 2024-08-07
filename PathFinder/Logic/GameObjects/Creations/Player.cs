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
        public class BitmapHaver
        {
            public Bitmap bmp;
        }

        public SolidBrush brush;

        private BitmapHaver sBoatLeft = new BitmapHaver();
        private BitmapHaver sBoatRight = new BitmapHaver();
        private BitmapHaver sBoatDown = new BitmapHaver();
        private BitmapHaver sBoatUp = new BitmapHaver();
        private BitmapHaver sCarLeft = new BitmapHaver();
        private BitmapHaver sCarRight = new BitmapHaver();
        private BitmapHaver sCarDown = new BitmapHaver();
        private BitmapHaver sCarUp = new BitmapHaver();

        public Player(Color color, Point fieldPosition) : base(fieldPosition)
        {
            brush = new SolidBrush(color);
            IEnumerable<(string name, BitmapHaver bmpHaver)> allBitmaps = GetAllBitmaps();
            for (int i = 0; i < allBitmaps.Count(); i++)
            {
                var item = allBitmaps.ElementAt(i);
                string file = $"player\\s_{item.name}.png";
                if (File.Exists(file)) item.bmpHaver.bmp = Image.FromFile(file) as Bitmap;
            }
        }

        public override bool CanUseWater { get; protected set; } = true;

        public IEnumerable<(string, BitmapHaver)> GetAllBitmaps()
        {
            yield return ("boatLeft", sBoatLeft);
            yield return ("boatRight", sBoatRight);
            yield return ("boatDown", sBoatDown);
            yield return ("boatUp", sBoatUp);
            yield return ("carLeft", sCarLeft);
            yield return ("carRight", sCarRight);
            yield return ("carDown", sCarDown);
            yield return ("carUp", sCarUp);
        }

        public Bitmap GetBitmap(string transport, string direction)
        {
            IEnumerable<(string name, BitmapHaver bmpHaver)> allBitmaps = GetAllBitmaps();
            return allBitmaps.FirstOrDefault(x =>
                x.name.ToLower().Contains(transport.ToLower()) &&
                x.name.ToLower().Contains(direction.ToLower())
            ).bmpHaver.bmp;
        }

        private string oldDirection = "right";
        protected override void Draw(Graphics g)
        {
            Terrain terra = PFinder.GetTerrain(FieldPosition);
            string transport = terra.GetType().Name.ToLower() == "water" ? "boat" : "car";
            string direction;

            if (OffsetX != 0)
                direction = OffsetX < 0 ? "left" : "right";
            else if (OffsetY != 0)
                direction = OffsetY < 0 ? "up" : "down";
            else
                direction = oldDirection;

            oldDirection = direction;
            Bitmap bitmap = GetBitmap(transport, direction);
            if (bitmap != null)
                DrawImage(g, bitmap);
            else
                DrawDefault(g);

            void DrawImage(Graphics g, Image img)
            { 
                try 
                { 
                    g.DrawImage(img, new Rectangle(new Point(RealPosition.X + OffsetX, RealPosition.Y + OffsetY), new Size(PFinder.CELLSIZE, PFinder.CELLSIZE))); 
                } 
                catch { } 
            }

            void DrawDefault(Graphics g)
            {
                try 
                { 
                    g.FillEllipse(brush, new Rectangle(new Point(RealPosition.X + OffsetX, RealPosition.Y + OffsetY), RealSize)); 
                } 
                catch { }
            }
        }
    }
}
