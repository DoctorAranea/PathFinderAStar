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
        public static int FIELD_WIDTH = 20;
        public static int FIELD_HEIGHT = 20;
        public static int CELLSIZE = 30;
        public static int PANEL_HEIGHT = 200;

        private static PictureBox pBox_Game;
        private static PictureBox pBox_Panel;

        private string goalBitmapPath = "goal.png";
        private static Bitmap goalBitmap;
        private static Bitmap backgroundBitmap;

        private static Rectangle selectedArea;

        public PFinder(int fieldWidth, int fieldHeight) : base()
        {
            DoubleBuffered = true;

            BrightnessMaps = new Dictionary<string, float[,]>();

            if (File.Exists(goalBitmapPath))
                goalBitmap = new Bitmap(goalBitmapPath);

            FIELD_WIDTH = fieldWidth;
            FIELD_HEIGHT = fieldHeight;
            GenerateMap();

            SelectedObjects = new List<GameObject>();

            Creations = new List<Creation>();
            Creations.Add(new Player(Color.Red, new Point(4, 3)));
            Creations.Add(new Player(Color.Red, new Point(11, 6)));

            pBox_Game = new PictureBox();
            pBox_Game.Parent = this;
            pBox_Game.Dock = DockStyle.Top;
            pBox_Game.SizeMode = PictureBoxSizeMode.AutoSize;
            pBox_Game.MouseClick += PBox_MouseClick;
            pBox_Game.MouseDown += PBox_MouseDown;
            pBox_Game.MouseUp += PBox_MouseUp;
            pBox_Game.MouseMove += PBox_MouseMove;

            pBox_Panel = new PictureBox();
            pBox_Panel.Parent = this;
            pBox_Panel.Size = new Size(pBox_Game.Size.Width, PANEL_HEIGHT);
            pBox_Panel.Dock = DockStyle.Bottom;
            pBox_Panel.SizeMode = PictureBoxSizeMode.AutoSize;
            pBox_Panel.Paint += PBox_Panel_Paint;

            DrawMap();
        }

        private void PBox_Panel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(Color.Black);

            if (SelectedObjects.Count > 0)
            {
                g.DrawString(SelectedObjects[0].Name, new Font("Comic Sans MS", 20, FontStyle.Bold), new SolidBrush(Color.White), 10, 20);
            }
        }

        public static Rectangle SelectedArea 
        { 
            get => selectedArea; 
            set
            {
                selectedArea = value;
                pBox_Panel.Refresh();
            }
        }

        public static bool DrawGizmosPath { get; set; } = true;
        public static bool DrawGizmosGoal { get; set; } = true;

        public static Dictionary<string, float[,]> BrightnessMaps { get; set; }

        public static List<GameObject> SelectedObjects { get; set; }
        public static List<Terrain> Terrains { get; set; }
        public static List<Creation> Creations { get; set; }
        public static Terrain[,] TerrainMap
        {
            get
            {
                Terrain[,] map = new Terrain[FIELD_WIDTH, FIELD_HEIGHT];
                for (int i = 0; i < Terrains.Count; i++)
                {
                    Terrain terra = Terrains[i];
                    map[terra.FieldPosition.X, terra.FieldPosition.Y] = terra;
                }
                return map;
            }
        }


        private void PBox_MouseClick(object sender, MouseEventArgs e) => MouseLogic.MouseClicked(this, sender, e);

        private void PBox_MouseMove(object sender, MouseEventArgs e) => MouseLogic.MouseMoved(this, sender, e);

        private void PBox_MouseUp(object sender, MouseEventArgs e) => MouseLogic.MouseUpped(this, sender, e);

        private void PBox_MouseDown(object sender, MouseEventArgs e) => MouseLogic.MouseDowned(this, sender, e);


        public static void DrawMap()
        {
            Bitmap world = new Bitmap(backgroundBitmap);
            Graphics g = Graphics.FromImage(world);

            for (int i = 0; i < Creations.Count; i++)
            {
                var gizmosPath = Creations[i].GizmosPath;

                for (int j = 0; DrawGizmosPath && j < gizmosPath.Count - 1; j++)
                {
                    Point realPathPosition = GetRealPosition(gizmosPath[j]);
                    realPathPosition.X += CELLSIZE / 2;
                    realPathPosition.Y += CELLSIZE / 2;
                    g.FillRectangle(new SolidBrush(Color.Lime), new Rectangle(realPathPosition.X - 3, realPathPosition.Y - 3, 6, 6));
                }

                if (DrawGizmosGoal && gizmosPath.Count > 0)
                {
                    Point gizmosGoal = gizmosPath.LastOrDefault();

                    if (goalBitmap != default)
                        g.DrawImage(goalBitmap, new Rectangle(GetRealPosition(gizmosGoal), new Size(CELLSIZE, CELLSIZE)));
                    else
                        g.DrawEllipse(new Pen(Color.Lime), new Rectangle(GetRealPosition(gizmosGoal), new Size(CELLSIZE - 1, CELLSIZE - 1)));
                }
            }

            for (int i = 0; i < Creations.Count; i++)
                Creations[i].SafeDraw(g);

            for (int i = 0; i < SelectedObjects.Count; i++)
            {
                if (SelectedObjects[i] is Creation)
                {
                    Creation sObj = SelectedObjects[i] as Creation;
                    Point offsetRealPosition = new Point
                        (
                            sObj.RealPosition.X + sObj.OffsetX,
                            sObj.RealPosition.Y + sObj.OffsetY
                        );
                    g.DrawEllipse(new Pen(Color.Lime, 2), new Rectangle(offsetRealPosition, new Size(CELLSIZE - 1, CELLSIZE - 1)));
                }
                else
                {
                    GameObject sObj = SelectedObjects[i];
                    g.DrawEllipse(new Pen(Color.Lime, 2), new Rectangle(sObj.RealPosition, new Size(CELLSIZE - 1, CELLSIZE - 1)));
                }
            }

            if (SelectedArea != default)
            {
                g.DrawRectangle(new Pen(Color.Lime), new Rectangle(SelectedArea.X, SelectedArea.Y, SelectedArea.Width, SelectedArea.Height));
            }

            pBox_Game.Image = world;
        }

        private void GenerateMap()
        {
            Terrains = new List<Terrain>();

            try
            {
                backgroundBitmap = new Bitmap(FIELD_WIDTH * CELLSIZE, FIELD_HEIGHT * CELLSIZE);
                Graphics g = Graphics.FromImage(backgroundBitmap);

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
                            case 'g': Terrains.Add(new Grass(new Point(x, y))); break;
                            case 'w': Terrains.Add(new Water(new Point(x, y))); break;
                            case 'r': Terrains.Add(new Rock(new Point(x, y))); break;
                            case 's': Terrains.Add(new Sand(new Point(x, y))); break;
                            case 'v': Terrains.Add(new GameObjects.Terrains.Void(new Point(x, y))); break;
                        }
                        Terrains.LastOrDefault().SafeDraw(g);
                        mapDataX++;
                    }
                    mapDataY++;
                }
            }
            catch { }
        }

        public static Point GetRealPosition(Point fieldPosition) => new Point(fieldPosition.X * CELLSIZE, fieldPosition.Y * CELLSIZE);

        public static Terrain GetTerrain(Point coords) => Terrains.FirstOrDefault(x => x.FieldPosition == coords);

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

                        var neighborTerra = Terrains.FirstOrDefault(t => t.FieldPosition == new Point(x, y));
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
            BrightnessMaps.Add(name, brightnessMap);
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
