using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PathFinder.Logic.GameObjects.Abstract
{
    public abstract class Creation : GameObject
    {
        protected Creation(Point fieldPosition) : base(fieldPosition)
        {
            OldFieldPosition = fieldPosition;
        }

        public abstract bool CanUseWater { get; protected set; }

        public int OffsetX { get; set; }
        public int OffsetY { get; set; }
        public Point OldFieldPosition { get; private set; }
        public Point OldRealPosition { get => new Point(OldFieldPosition.X * PFinder.CELLSIZE, OldFieldPosition.Y * PFinder.CELLSIZE); }

        public bool Move(Point end)
        {
            var terrain = PFinder.GetTerrain(end);
            if (terrain.MoveDifficulty < 150)
            {
                OldFieldPosition = FieldPosition;
                FieldPosition = end;
                return true;
            }
            else
                return false;
        }

        public List<Point> FindPath(Point start, Point end)
        {
            Terrain[,] terrains = PFinder.TerrainMap;
            var openList = new List<TerrainPathData>();
            var closedList = new List<TerrainPathData>();

            TerrainPathData activeTerra = terrains[start.X, start.Y].ToSimplified();

            while (activeTerra.fieldPosition.X != end.X || activeTerra.fieldPosition.Y != end.Y)
            {
                AppendOpenList(openList, closedList, activeTerra, end);
                if (openList.Count > 0)
                {
                    var minWeight = openList.Min(x => x.pathData.weight);
                    activeTerra = openList.FirstOrDefault(x => x.pathData.weight == minWeight);
                }
                else
                    break;
            }

            if (openList.Count == 0)
                return new List<Point>();

            closedList.Add(activeTerra);

            List<Point> path = new List<Point>();
            Point pathBuilder = end;
            while (pathBuilder != start)
            {
                var terrain = closedList.FirstOrDefault(x => x.fieldPosition == pathBuilder);
                path.Add(terrain.fieldPosition);
                pathBuilder = terrain.pathData.link.fieldPosition;
            }
            path.Reverse();

            return path;
        }

        private void AppendOpenList
            (
                List<TerrainPathData> openList,
                List<TerrainPathData> closedList,
                TerrainPathData activeTerra,
                Point end
            )
        {
            if (activeTerra.pathData == null)
            {
                float lengthToEndX = Math.Abs(end.X - activeTerra.fieldPosition.X);
                float lengthToEndY = Math.Abs(end.Y - activeTerra.fieldPosition.Y);
                float lengthToEnd = (lengthToEndX + lengthToEndY) * 10;

                activeTerra.pathData = new TerrainPathData.PathData(1, lengthToEnd, null);
            }

            List<TerrainPathData> neighbors = PFinder.GetNeighborsList(activeTerra).Select(x => x).ToList();
            for (int i = 0; i < neighbors.Count; i++)
            {
                var neighbor = neighbors[i];

                if (neighbor.moveDifficulty >= 150)
                    continue;

                int nX = neighbor.fieldPosition.X;
                int nY = neighbor.fieldPosition.Y;

                TerrainPathData fromOpenList = openList.FirstOrDefault(x => x.fieldPosition.X == nX && x.fieldPosition.Y == nY);

                if (closedList.FirstOrDefault(x => x.fieldPosition == neighbor.fieldPosition) == null)
                {
                    Terrain neighborTerrain = PFinder.GetTerrain(neighbor.fieldPosition);
                    if (neighborTerrain is Terrains.Water && !CanUseWater) continue;

                    bool diagonal = nX != activeTerra.fieldPosition.X && nY != activeTerra.fieldPosition.Y;
                    float pathLength = ((diagonal ? 14 : 10) + activeTerra.pathData.pathLength) * neighbor.moveDifficulty;

                    if (fromOpenList != null)
                    {
                        if (pathLength > fromOpenList.pathData.pathLength)
                            continue;
                    }

                    float lengthToEndX = Math.Abs(end.X - nX);
                    float lengthToEndY = Math.Abs(end.Y - nY);
                    float lengthToEnd = (lengthToEndX + lengthToEndY) * 10;

                    var pathData = new TerrainPathData.PathData(pathLength, lengthToEnd, activeTerra);

                    if (fromOpenList != null)
                    {
                        fromOpenList.pathData = pathData;
                    }
                    else
                    {
                        neighbor.pathData = pathData;
                        openList.Add(neighbor);
                    }
                }
            }

            closedList.Add(activeTerra);
            openList.Remove(activeTerra);
        }
    }
}
