using PathFinder.Logic.GameObjects.Terrains;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinder.Logic.GameObjects.Abstract
{
    public abstract class Terrain : GameObject
    {
        protected string name;

        protected Terrain(Point fieldPosition) : base(fieldPosition) 
        {
            name = this.GetType().Name;
            string bmName = $"bm_{name}.png".ToLower();
            if (!PFinder.BrightnessMaps.ContainsKey(name) && File.Exists(bmName))
            {
                var brightnessMap = PFinder.GetBrightnessMap(bmName);
                PFinder.AddBrightnessMap(name, brightnessMap);
            }
        }

        public override string Description { get; protected set; } = "Местность";
        public override List<Command> Commands { get; set; } = new List<Command>();
        public abstract float MoveDifficulty { get; protected set; }
        public abstract Color TerrainColor { get; protected set; }

        public abstract Terrain CopyToNewCoords(Point coords);

        protected override void Draw(Graphics g)
        {
            Size offsetSize = new Size(RealSize.Width + 1, RealSize.Height + 1);
            if (PFinder.BrightnessMaps.ContainsKey(name))
            {
                Bitmap skin = PFinder.GetSkin(PFinder.BrightnessMaps[name], TerrainColor);
                g.DrawImage(skin, new Rectangle(RealPosition, new Size(PFinder.CELLSIZE, PFinder.CELLSIZE)));
            }
            else
                g.FillRectangle(new SolidBrush(TerrainColor), new Rectangle(RealPosition, offsetSize));
        }

        public TerrainPathData ToSimplified() => new TerrainPathData(FieldPosition, MoveDifficulty);
    }
}
