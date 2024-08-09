using PathFinder.Logic.GameObjects.Abstract;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinder.Logic.Commands
{
    public class CallOfVoid : Command
    {
        protected override string IconPath { get; set; } = "icons\\abilityIcon_callOfVoid.png";
        public override Bitmap Icon { get; protected set; }
        public override string Title { get; protected set; } = "Зов Пустоты";
        public override string Description { get; protected set; } = "Обращение местности в Пустоту";

        public override OutputParameters Run(InputParameters input)
        {
            if (!(input.receiver is Terrain))
            {
                return new OutputParameters() 
                { 
                    result = false,
                    resultText = "Целью должна быть поверхность"
                };
            }

            Terrain terrain = input.receiver as Terrain;
            Terrain newTerrain = new GameObjects.Terrains.Void(terrain.FieldPosition);
            int index = PFinder.Terrains.FindIndex(x => x == terrain);
            PFinder.Terrains.RemoveAt(index);
            PFinder.Terrains.Insert(index, newTerrain);
            PFinder.UpdateTerrain(PFinder.Terrains[index]);

            return new OutputParameters()
            {
                result = true
            };
        }
    }
}
