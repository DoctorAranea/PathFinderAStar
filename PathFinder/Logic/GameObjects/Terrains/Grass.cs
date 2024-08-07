using PathFinder.Logic.GameObjects.Abstract;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinder.Logic.GameObjects.Terrains
{
    public class Grass : Terrain
    {
        public Grass(Point fieldPosition) : base(fieldPosition) { }

        public override string Name { get; } = "Трава";

        public override float MoveDifficulty { get; protected set; } = 1f;
        public override Color TerrainColor { get; protected set; } = Color.Green;

        public override Terrain CopyToNewCoords(Point coords) => new Grass(coords);
    }
}
