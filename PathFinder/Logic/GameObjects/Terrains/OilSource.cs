using PathFinder.Logic.GameObjects.Abstract;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinder.Logic.GameObjects.Terrains
{
    public class OilSource : Terrain
    {
        public OilSource(Point fieldPosition) : base(fieldPosition) { }

        public override string Name { get; protected set; } = "Нефть";
        public override Bitmap Icon { get; protected set; }

        public override float MoveDifficulty { get; protected set; } = 5f;
        public override Color TerrainColor { get; protected set; } = Color.Blue;

        public override Terrain CopyToNewCoords(Point coords) => new Water(coords);
    }
}
