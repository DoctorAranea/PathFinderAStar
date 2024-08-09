using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PathFinder.Logic.GameObjects
{
    public abstract class Building : GameObject
    {
        protected Building(Point fieldPosition, string customName = "") : base(fieldPosition)
        {
            if (customName != "")
                Name = customName;

            if (File.Exists(BuildingBitmapFile)) 
                BuildingBitmap = Image.FromFile(BuildingBitmapFile) as Bitmap;
        }

        protected abstract string BuildingBitmapFile { get; set; }
        public abstract Bitmap BuildingBitmap { get; protected set; }

        public override List<Command> Commands { get; set; } = new List<Command>();

        public abstract List<(Type, int)> MinedResources { get; set; }
    }
}
