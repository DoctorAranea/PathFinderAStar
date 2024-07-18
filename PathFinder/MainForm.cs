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

namespace PathFinder
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            Load += MainForm_Load;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.Size = new Size(PFinder.CELLSIZE * PFinder.FIELD_WIDTH + 16, PFinder.CELLSIZE * PFinder.FIELD_HEIGHT + 39);
        }
    }
}
