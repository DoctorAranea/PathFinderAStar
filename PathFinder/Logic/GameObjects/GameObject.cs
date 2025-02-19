﻿using PathFinder.Logic.Commands;
using System.Collections.Generic;
using System.Drawing;
using static PathFinder.Logic.PFinder;

namespace PathFinder.Logic
{
    public abstract class GameObject
    {
        public class BitmapHaver
        {
            public Bitmap bmp;
        }

        protected GameObject(Point fieldPosition, Size fieldSize = default)
        {
            if (fieldSize == default)
                fieldSize = new Size(1, 1);

            FieldSize = fieldSize;
            FieldPosition = fieldPosition;
        }

        public abstract string Name { get; protected set; }
        public abstract string Description { get; protected set; }
        public abstract Bitmap Icon { get; protected set; }

        public abstract List<Command> Commands { get; set; }

        public Size FieldSize { get; protected set; }
        public Size RealSize { get => new Size(FieldSize.Width * CELLSIZE, FieldSize.Height * CELLSIZE); }

        public Point FieldPosition { get; protected set; }
        public Point RealPosition { get => new Point(FieldPosition.X * CELLSIZE, FieldPosition.Y * CELLSIZE); }


        public void SafeDraw(Graphics g)
        {
            try
            {
                Draw(g);
            }
            catch { }
        }

        protected abstract void Draw(Graphics g);
    }
}