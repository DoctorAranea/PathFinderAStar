using ColorMine.ColorSpaces;
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
        public const int CELLSIZE = 25;

        public static bool isGameplayEnabled = true;
        public static Dictionary<string, float[,]> brightnessMaps;
        private static List<Terrain> terrains;
        private static List<Creation> creations;

        private Player player;

        private PictureBox pBox;
        private System.Windows.Forms.Timer timer;

        public PFinder() : base()
        {
            DoubleBuffered = true;
            brightnessMaps = new Dictionary<string, float[,]>();

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
            if (isGameplayEnabled)
                pBox.Invalidate();
        }

        private List<Point> gizmosPath = new List<Point>();
        private int fieldOfView = 7;
        private bool firstDraw = true;

        private void PBox_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(Color.Black);

            if (!firstDraw)
            {
                var changedCells = creations.Select(x => x.FieldPosition).ToList();
                var fieldOfViews = creations.Select(x => new Rectangle(x.FieldPosition.X - fieldOfView / 2, x.FieldPosition.Y - fieldOfView / 2, fieldOfView, fieldOfView)).ToList();
                for (int i = 0; i < terrains.Count; i++)
                {
                    var rect = fieldOfViews.FirstOrDefault(x => x.Contains(terrains[i].FieldPosition));
                    if (rect != default)
                        terrains[i].Draw(g);
                    else
                    {
                        Color terraColor = terrains[i].TerrainColor;
                        Color fogColor = Color.FromArgb(terraColor.R / 2, terraColor.G / 2, terraColor.B / 2);

                        g.FillRectangle(new SolidBrush(fogColor), new Rectangle(terrains[i].RealPosition, new Size(terrains[i].RealSize.Width + 1, terrains[i].RealSize.Height + 1)));
                    }
                }
            }
            else
            {
                for (int i = 0; i < terrains.Count; i++)
                    terrains[i].Draw(g);
                firstDraw = false;
            }

            //int linesBrightness = 25;
            //Pen linesPen = new Pen(Color.FromArgb(linesBrightness, linesBrightness, linesBrightness));

            //for (int y = 0; y <= FIELD_HEIGHT; y++)
            //{
            //    for (int x = 0; x <= FIELD_WIDTH; x++)
            //    {
            //        g.DrawLine(linesPen, CELLSIZE * x, CELLSIZE * y, CELLSIZE * x, FIELD_HEIGHT);
            //        g.DrawLine(linesPen, CELLSIZE * x, CELLSIZE * y, FIELD_WIDTH, CELLSIZE * y);
            //    }
            //}

            if (gizmosPath.Count > 0)
            {
                Point start = player.RealPosition;
                start.X += CELLSIZE / 2;
                start.Y += CELLSIZE / 2;
                for (int i = 0; i < gizmosPath.Count; i++)
                {
                    Point realPathPosition = new Point(gizmosPath[i].X * CELLSIZE, gizmosPath[i].Y * CELLSIZE);
                    realPathPosition.X += CELLSIZE / 2;
                    realPathPosition.Y += CELLSIZE / 2;
                    g.DrawLine(new Pen(Color.Red, (gizmosPath.Count - i)), start, realPathPosition);
                    start = realPathPosition;
                }
            }

            for (int i = 0; i < creations.Count; i++)
                creations[i].Draw(g);
        }

        private void PBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (!isGameplayEnabled)
                return;

            isGameplayEnabled = false;
            Point clickPosition = e.Location;
            Point fieldClickPosition = new Point(clickPosition.X / CELLSIZE, clickPosition.Y / CELLSIZE);

            if (gizmosPath.Count == 0 || gizmosPath.LastOrDefault() != fieldClickPosition)
            {
                gizmosPath = player.FindPath(player.FieldPosition, fieldClickPosition);
                pBox.Invalidate();
                isGameplayEnabled = true;
            }
            else
            {
                new Thread(() =>
                {
                    while (gizmosPath.Count > 0)
                    {
                        player.Move(gizmosPath[0]);
                        gizmosPath.RemoveAt(0);
                        Invoke(new Action(() =>
                        {
                            pBox.Invalidate();
                        }));
                        Thread.Sleep(100);
                    }
                    gizmosPath.Clear();
                    isGameplayEnabled = true;
                }).Start();
            }
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

        public static List<SimplifiedTerrain> GetNeighborsList(SimplifiedTerrain terrain)
        {
            List<SimplifiedTerrain> neighbors = new List<SimplifiedTerrain>();
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
                            SimplifiedTerrain neighbor = neighborTerra.ToSimplified();
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
