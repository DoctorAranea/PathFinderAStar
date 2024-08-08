using PathFinder.Logic.GameObjects.Abstract;
using System.Drawing;
using System.IO;

namespace PathFinder.Logic
{
    public abstract class Command
    {
        public class InputParameters
        {
            public GameObject sender;
            public GameObject receiver;
        }

        public class OutputParameters
        {
            public bool result;
            public string resultText;
        }

        protected Command()
        {
            if (File.Exists(IconPath))
                Icon = new Bitmap(IconPath);
        }

        protected abstract string IconPath { get; set; } 
        public abstract Bitmap Icon { get; protected set; } 

        public abstract OutputParameters Run(InputParameters input);
    }
}
