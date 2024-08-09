using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinder.Logic.Resources
{
    public class Oil : Resource
    {
        public override string Name { get; protected set; } = "Нефть";
        public override int Value { get; set; }
    }
}
