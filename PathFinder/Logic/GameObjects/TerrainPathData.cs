using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinder.Logic.GameObjects
{
    public class TerrainPathData
    {
        public class PathData
        {
            public float weight;
            public float pathLength;
            public float lengthToEnd;
            public TerrainPathData link;

            public PathData(float pathLength, float lengthToEnd, TerrainPathData link)
            {
                this.link = link;
                this.pathLength = pathLength;
                this.lengthToEnd = lengthToEnd;
                weight = pathLength + lengthToEnd;
            }
        }

        public Point fieldPosition;
        public float moveDifficulty;
        public PathData pathData;

        public TerrainPathData(Point fieldPosition, float moveDifficulty)
        {
            this.fieldPosition = fieldPosition;
            this.moveDifficulty = moveDifficulty;
        }
    }
}
