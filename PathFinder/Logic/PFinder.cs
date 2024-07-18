﻿using ColorMine.ColorSpaces;
using PathFinder.Logic.GameObjects;
using PathFinder.Logic.GameObjects.Abstract;
using PathFinder.Logic.GameObjects.Creations;
using PathFinder.Logic.GameObjects.Terrains;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PathFinder.Logic
{
    public class PFinder : Control
    {
        public const int FIELD_WIDTH = 20;
        public const int FIELD_HEIGHT = 20;
        public const int CELLSIZE = 40;

        public static bool isGameplayEnabled = true;
        public static bool drawGizmosPath = true;
        public static bool drawGizmosGoal = true;
        public static Dictionary<string, float[,]> brightnessMaps;
        private static List<Terrain> terrains;
        private static List<Creation> creations;

        private Player player;

        private string goalBitmapPath = "goal.png";
        private Bitmap goalBitmap;

        private Rectangle fieldRect;
        private PictureBox pBox;
        private System.Windows.Forms.Timer timer;

        public PFinder() : base()
        {
            DoubleBuffered = true;
            brightnessMaps = new Dictionary<string, float[,]>();

            if (File.Exists(goalBitmapPath))
                goalBitmap = new Bitmap(goalBitmapPath);

            fieldRect = new Rectangle(0, 0, FIELD_WIDTH, FIELD_HEIGHT);
            GenerateMap();

            player = new Player(Color.Red, new Point(4, 3));

            creations = new List<Creation>();
            creations.Add(player);

            pBox = new PictureBox();
            pBox.Parent = this;
            pBox.Dock = DockStyle.Fill;
            pBox.Paint += PBox_Paint;
            pBox.MouseClick += PBox_MouseClick;

            timer = new System.Windows.Forms.Timer();
            timer.Interval = 250;
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        public static Terrain[,] TerrainMap
        {
            get
            {
                Terrain[,] map = new Terrain[FIELD_WIDTH, FIELD_HEIGHT];
                for (int i = 0; i < terrains.Count; i++)
                {
                    Terrain terra = terrains[i];
                    map[terra.FieldPosition.X, terra.FieldPosition.Y] = terra;
                }
                return map;
            }
        }

        private void GenerateMap()
        {
            terrains = new List<Terrain>();

            try
            {
                string[] mapDataLines = File.ReadAllLines("map.dat");

                int mapDataX;
                int mapDataY = 0;
                for (int y = 0; y < FIELD_HEIGHT; y++)
                {
                    mapDataX = 0;
                    for (int x = 0; x < FIELD_WIDTH; x++)
                    {
                        char terrainChar = 'v';
                        try { terrainChar = mapDataLines[mapDataY][mapDataX]; } catch { }
                        switch (terrainChar)
                        {
                            case 'g': terrains.Add(new Grass(new Point(x, y))); break;
                            case 'w': terrains.Add(new Water(new Point(x, y))); break;
                            case 'r': terrains.Add(new Rock(new Point(x, y))); break;
                            case 's': terrains.Add(new Sand(new Point(x, y))); break;
                            case 'v': terrains.Add(new GameObjects.Terrains.Void(new Point(x, y))); break;
                        }
                        mapDataX++;
                    }
                    mapDataY++;
                }
            }
            catch { }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            //pBox.Invalidate();
            timer.Stop();
        }

        private Graphics g;
        private List<Point> changedCells = new List<Point>();
        private List<Point> gizmosPath = new List<Point>();
        private int fieldOfView = 3;
        private bool firstDraw = true;
        private bool gizmosDrawn = false;

        private void PBox_Paint(object sender, PaintEventArgs e)
        {
            g = e.Graphics;
            //g.Clear(Color.Black);

            if (!firstDraw)
            {
                changedCells.AddRange(creations.Select(x => x.FieldPosition));
                var fieldOfViews = creations.Select(x => new Rectangle(x.FieldPosition.X - fieldOfView / 2, x.FieldPosition.Y - fieldOfView / 2, fieldOfView, fieldOfView)).ToList();

                for (int i = 0; i < fieldOfViews.Count; i++)
                {
                    var fov = fieldOfViews[i];
                    for (int y = fov.Y; y < fov.Y + fov.Height; y++)
                    {
                        for (int x = fov.X; x < fov.X + fov.Width; x++)
                        {
                            if (fieldRect.Contains(new Point(x, y)))
                                changedCells.Add(new Point(x, y));
                        }
                    }
                }

                while (changedCells.Count > 0)
                {
                    Point changedCell = changedCells[0];

                    var rect = fieldOfViews.FirstOrDefault(x => x.Contains(changedCell));
                    GetTerrain(changedCell).Draw(g);

                    changedCells.RemoveAt(0);
                }
            }
            else
            {
                for (int i = 0; i < terrains.Count; i++)
                    terrains[i].Draw(g);
                firstDraw = false;
            }

            if (drawGizmosPath && !gizmosDrawn && gizmosPath.Count > 0)
            {
                Point start = player.RealPosition;
                start.X += CELLSIZE / 2;
                start.Y += CELLSIZE / 2;
                for (int i = 0; i < gizmosPath.Count; i++)
                {
                    Point realPathPosition = new Point(gizmosPath[i].X * CELLSIZE, gizmosPath[i].Y * CELLSIZE);
                    realPathPosition.X += CELLSIZE / 2;
                    realPathPosition.Y += CELLSIZE / 2;
                    GetTerrain(gizmosPath[i]).Draw(g);
                    g.DrawLine(new Pen(Color.Yellow, (gizmosPath.Count - i)), start, realPathPosition);
                    start = realPathPosition;
                }

            }

            if (drawGizmosGoal && !gizmosDrawn && gizmosPath.Count > 0)
            {
                Point goal = gizmosPath.LastOrDefault();
                Point realGoalPosition = new Point(goal.X * CELLSIZE, goal.Y * CELLSIZE);
                GetTerrain(goal).Draw(g);
                if (goalBitmap != default)
                    g.DrawImage(goalBitmap, new Rectangle(realGoalPosition, new Size(CELLSIZE, CELLSIZE)));
                else
                    g.DrawEllipse(new Pen(Color.Yellow), new Rectangle(realGoalPosition, new Size(CELLSIZE - 1, CELLSIZE - 1)));
            }

            for (int i = 0; i < creations.Count; i++)
                creations[i].Draw(g);

            gizmosDrawn = true;
        }

        private void PBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (!isGameplayEnabled)
                return;

            isGameplayEnabled = false;
            Point clickPosition = e.Location;
            Point fieldClickPosition = new Point(clickPosition.X / CELLSIZE, clickPosition.Y / CELLSIZE);

            gizmosDrawn = false;
            gizmosPath = player.FindPath(player.FieldPosition, fieldClickPosition);
            UpdateGizmosGraphics(clear: false);
            UpdateGoalGraphics();

            new Thread(() =>
            {
                while (gizmosPath.Count > 0)
                {
                    bool changedX = player.FieldPosition.X != gizmosPath[0].X;
                    bool offsetIsNatural = (changedX ? player.FieldPosition.X : player.FieldPosition.Y) < (changedX ? gizmosPath[0].X : gizmosPath[0].Y);
                    int offsetsCount = 10;
                    int offset = CELLSIZE / offsetsCount;

                    for (int i = 1; i < offsetsCount; i++)
                    {
                        int totalOffset = offsetIsNatural ? offset : -offset;
                        player.OffsetX = changedX ? totalOffset * i : 0;
                        player.OffsetY = changedX ? 0 : totalOffset * i;
                        DrawCreationFromAnotherThread(player);
                        Thread.Sleep(25);
                    }

                    player.Move(gizmosPath[0]);
                    gizmosPath.RemoveAt(0);

                    player.OffsetX = 0;
                    player.OffsetY = 0;
                    DrawCreationFromAnotherThread(player);

                    void DrawCreationFromAnotherThread(Creation creation)
                    {
                        Invoke(new Action(() =>
                        {
                            pBox.Invalidate(new Rectangle(
                                new Point(creation.RealPosition.X - CELLSIZE, creation.RealPosition.Y - CELLSIZE),
                                new Size(creation.RealSize.Width + CELLSIZE * 2, creation.RealSize.Height + CELLSIZE * 2))
                            );
                        }));
                    }
                }
                isGameplayEnabled = true;
            }).Start();
        }

        private void UpdateGizmosGraphics(bool clear)
        {
            if (!drawGizmosPath)
                return;

            for (int i = gizmosPath.Count - 1; i >= 0; i--)
            {
                var gizmos = gizmosPath[i];
                if (clear)
                {
                    changedCells.Add(gizmos);
                    gizmosPath.Remove(gizmos);
                }
                pBox.Invalidate(new Rectangle(
                    new Point(gizmos.X * CELLSIZE, gizmos.Y * CELLSIZE),
                    new Size(CELLSIZE, CELLSIZE))
                );
            }
        }

        private void UpdateGoalGraphics()
        {
            if (!drawGizmosGoal)
                return;

            Point goal = gizmosPath.LastOrDefault();
            changedCells.Add(goal);
            pBox.Invalidate(new Rectangle(
                new Point(goal.X * CELLSIZE, goal.Y * CELLSIZE),
                new Size(CELLSIZE, CELLSIZE))
            );
        }

        public static Terrain GetTerrain(Point coords) => terrains.FirstOrDefault(x => x.FieldPosition == coords);

        public static Terrain[,] GetNeighbors(Terrain terrain)
        {
            Terrain[,] neighbors = new Terrain[3, 3];
            int nX;
            int nY = 0;
            for (int y = terrain.FieldPosition.Y - 1; y < terrain.FieldPosition.Y + 2; y++)
            {
                nX = 0;
                for (int x = terrain.FieldPosition.X - 1; x < terrain.FieldPosition.X + 2; x++)
                {
                    if (x != terrain.FieldPosition.X || y != terrain.FieldPosition.Y)
                    {
                        Terrain neighbor = terrains.FirstOrDefault(t => t.FieldPosition == new Point(x, y));
                        neighbors[nX, nY] = neighbor ?? terrain.CopyToNewCoords(new Point(x, y));
                    }
                    nX++;
                }
                nY++;
            }
            return neighbors;
        }

        public static List<TerrainPathData> GetNeighborsList(TerrainPathData terrain)
        {
            List<TerrainPathData> neighbors = new List<TerrainPathData>();
            int nX;
            int nY = 0;
            for (int y = terrain.fieldPosition.Y - 1; y < terrain.fieldPosition.Y + 2; y++)
            {
                nX = 0;
                for (int x = terrain.fieldPosition.X - 1; x < terrain.fieldPosition.X + 2; x++)
                {
                    if (x != terrain.fieldPosition.X || y != terrain.fieldPosition.Y)
                    {
                        if (x != terrain.fieldPosition.X && y != terrain.fieldPosition.Y)
                            continue;

                        var neighborTerra = terrains.FirstOrDefault(t => t.FieldPosition == new Point(x, y));
                        if (neighborTerra != null)
                        {
                            TerrainPathData neighbor = neighborTerra.ToSimplified();
                            neighbors.Add(neighbor);
                        }
                    }
                    nX++;
                }
                nY++;
            }
            return neighbors;
        }

        public static void AddBrightnessMap(string name, float[,] brightnessMap)
        {
            brightnessMaps.Add(name, brightnessMap);
        }

        public static float[,] GetBrightnessMap(string filename)
        {
            try
            {
                Bitmap skin = new Bitmap(filename);
                float[,] newBrightnessMap = new float[skin.Width, skin.Height];
                for (int y = 0; y < skin.Height; y++)
                {
                    for (int x = 0; x < skin.Width; x++)
                    {
                        Color pixel = skin.GetPixel(x, y);
                        float brightness = pixel.GetBrightness();
                        newBrightnessMap[x, y] = brightness;
                    }
                }

                return newBrightnessMap;
            }
            catch { return null; }
        }

        public static Bitmap GetSkin(float[,] brightnessMap, Color color)
        {
            Bitmap skin = new Bitmap(brightnessMap.GetLength(0), brightnessMap.GetLength(1));
            for (int y = 0; y < brightnessMap.GetLength(1); y++)
            {
                for (int x = 0; x < brightnessMap.GetLength(0); x++)
                {
                    Rgb rgb = new Rgb() { R = color.R, G = color.G, B = color.B };
                    Hsb hsb = rgb.To<Hsb>();
                    //hsb.S = .5;
                    hsb.B = brightnessMap[x, y];
                    Color pixelColor = hsb.ToSystemColor();
                    if (brightnessMap[x, y] == 0)
                        pixelColor = Color.FromArgb(0, pixelColor.R, pixelColor.G, pixelColor.B);
                    skin.SetPixel(x, y, pixelColor);
                }
            }

            return skin;
        }
    }
}
