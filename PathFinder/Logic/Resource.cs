using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinder.Logic
{
    public abstract class Resource
    {
        protected Resource()
        {
            string name = this.GetType().Name;
            string bmName = $"resources\\s_{name}.png".ToLower();

            if (File.Exists(bmName))
                Icon = Image.FromFile(bmName) as Bitmap;
        }

        public Bitmap Icon { get; protected set; }

        public abstract string Name { get; protected set; }
        public abstract int Value { get; set; }
    }
}
