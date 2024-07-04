using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinder.Logic.GameObjects.Abstract
{
    public abstract class Creation : GameObject
    {
        protected Creation(Point fieldPosition) : base(fieldPosition) { }

        public bool Move(Point end)
        {
            var terrain = PFinder.GetTerrain(end);
            if (terrain.MoveDifficulty < 150)
            {
                FieldPosition = end;
                return true;
            }
            else
                return false;
        }

        public List<Point> FindPath(Point start, Point end)
        {
            Terrain[,] terrains = PFinder.TerrainMap;
            var openList = new List<SimplifiedTerrain>();
            var closedList = new List<SimplifiedTerrain>();

            SimplifiedTerrain activeTerra = terrains[start.X, start.Y].ToSimplified();

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
                List<SimplifiedTerrain> openList,
                List<SimplifiedTerrain> closedList,
                SimplifiedTerrain activeTerra,
                Point end
            )
        {
            if (activeTerra.pathData == null)
            {
                float lengthToEndX = Math.Abs(end.X - activeTerra.fieldPosition.X);
                float lengthToEndY = Math.Abs(end.Y - activeTerra.fieldPosition.Y);
                float lengthToEnd = (lengthToEndX + lengthToEndY) * 10;

                activeTerra.pathData = new SimplifiedTerrain.PathData(1, lengthToEnd, null);
            }

            List<SimplifiedTerrain> neighbors = PFinder.GetNeighborsList(activeTerra).Select(x => x).ToList();
            for (int i = 0; i < neighbors.Count; i++)
            {
                var neighbor = neighbors[i];

                if (neighbor.moveDifficulty >= 150)
                    continue;

                int nX = neighbor.fieldPosition.X;
                int nY = neighbor.fieldPosition.Y;

                SimplifiedTerrain fromOpenList = openList.FirstOrDefault(x => x.fieldPosition.X == nX && x.fieldPosition.Y == nY);

                if (closedList.FirstOrDefault(x => x.fieldPosition == neighbor.fieldPosition) == null)
                {
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

                    var pathData = new SimplifiedTerrain.PathData(pathLength, lengthToEnd, activeTerra);

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
