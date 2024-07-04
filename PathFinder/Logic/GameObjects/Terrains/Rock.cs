using PathFinder.Logic.GameObjects.Abstract;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinder.Logic.GameObjects.Terrains
{
    public class Rock : Terrain
    {
        public Rock(Point fieldPosition) : base(fieldPosition) { }

        public override float MoveDifficulty { get; protected set; } = 999f;
        public override Color TerrainColor { get; protected set; } = Color.Gray;

        public override Terrain CopyToNewCoords(Point coords) => new Rock(coords);

        public override void Draw(Graphics g)
        {
            Size offsetSize = new Size(RealSize.Width + 1, RealSize.Height + 1);
            if (PFinder.brightnessMaps.ContainsKey(name))
            {
                Grass grass = new Grass(FieldPosition);
                grass.Draw(g);

                Bitmap skin = PFinder.GetSkin(PFinder.brightnessMaps[name], TerrainColor);
                g.DrawImage(skin, new Rectangle(RealPosition, new Size(PFinder.CELLSIZE, PFinder.CELLSIZE)));
            }
            else
                g.FillRectangle(new SolidBrush(TerrainColor), new Rectangle(RealPosition, offsetSize));
        }
    }
}
