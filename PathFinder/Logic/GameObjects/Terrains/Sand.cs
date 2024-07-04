using PathFinder.Logic.GameObjects.Abstract;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinder.Logic.GameObjects.Terrains
{
    public class Sand : Terrain
    {
        public Sand(Point fieldPosition) : base(fieldPosition) { }

        public override float MoveDifficulty { get; protected set; } = 1.5f;
        public override Color TerrainColor { get; protected set; } = Color.FromArgb(239, 210, 96);

        public override Terrain CopyToNewCoords(Point coords) => new Sand(coords);
    }
}
