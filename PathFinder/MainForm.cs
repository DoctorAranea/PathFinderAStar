using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PathFinder.Logic;
using PathFinder;
using System.IO;

namespace PathFinder
{
    public partial class MainForm : Form
    {
        private PFinder pFinder;
        public MainForm()
        {
            InitializeComponent();

            string[] mapDataLines = File.ReadAllLines("map.dat");
            int fieldWidth = mapDataLines[0].Length;
            int fieldHeight = mapDataLines.Length;

            pFinder = new PFinder(fieldWidth, fieldHeight);
            pFinder.Dock = DockStyle.Fill;
            pFinder.Parent = this;
            pFinder.Show();

            Load += MainForm_Load;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.Size = new Size(PFinder.CELLSIZE * PFinder.FIELD_WIDTH + 16, PFinder.CELLSIZE * PFinder.FIELD_HEIGHT + 39 + PFinder.PANEL_HEIGHT + PFinder.PANELRES_HEIGHT);
        }
    }
}
