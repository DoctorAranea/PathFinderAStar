using PathFinder.Logic.GameObjects.Abstract;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinder.Logic.GameObjects.Terrains
{
    public class Void : Terrain
    {
        public Void(Point fieldPosition) : base(fieldPosition) { }

        public override string Name { get; protected set; } = "Terra Incognita";
        public override Bitmap Icon { get; protected set; }

        public override float MoveDifficulty { get; protected set; } = 999f;
        public override Color TerrainColor { get; protected set; } = Color.Black;

        public override Terrain CopyToNewCoords(Point coords) => new Void(coords);
    }
}
