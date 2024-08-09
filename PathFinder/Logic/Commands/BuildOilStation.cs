using PathFinder.Logic.GameObjects.Abstract;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinder.Logic.Commands
{
    public class BuildOilStation : Command
    {
        protected override string IconPath { get; set; } = "icons\\abilityIcon_buildOilStation.png";
        public override Bitmap Icon { get; protected set; }
        public override string Title { get; protected set; } = "Построить нефтяную платформу";
        public override string Description { get; protected set; } = "Строительство нефтяной вышки в океане для добычи нефти";

        public override OutputParameters Run(InputParameters input)
        {
            if (!(input.receiver is GameObjects.Terrains.OilSource))
            {
                return new OutputParameters()
                {
                    result = false,
                    resultText = "Здание может быть установлено на водном источнике нефти"
                };
            }

            PFinder.Buildings.Add(new GameObjects.Buildings.OilStation(input.receiver.FieldPosition));
            PFinder.DrawMap();

            return new OutputParameters()
            {
                result = true
            };
        }
    }
}
