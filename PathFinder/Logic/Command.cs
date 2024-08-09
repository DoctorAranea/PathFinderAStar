using PathFinder.Logic.GameObjects.Abstract;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

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
        public abstract string Title { get; protected set; } 
        public abstract string Description { get; protected set; } 
        protected abstract List<(Type, int)> Costs { get; set; } 

        public OutputParameters Use(InputParameters input)
        {
            bool enough = true;
            for (int i = 0; i < Costs.Count; i++)
            {
                (Type res, int value) = Costs[i];
                Resource resource = PFinder.Resources.FirstOrDefault(x => x.GetType() == res);

                if (value > resource.Value)
                {
                    enough = false;
                    break;
                }
            }

            if (!enough)
            {
                return new OutputParameters()
                {
                    result = false,
                    resultText = "Недостаточно ресурсов!"
                };
            }

            var output = Run(input);
            if (output.result)
            {
                for (int i = 0; i < Costs.Count; i++)
                {
                    (Type res, int value) = Costs[i];
                    Resource resource = PFinder.Resources.FirstOrDefault(x => x.GetType() == res);
                    resource.Value -= value;
                }
                PFinder.DrawPanelRes();
            }

            return output;
        }

        protected abstract OutputParameters Run(InputParameters input);
    }
}
