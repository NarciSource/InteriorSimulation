using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace selfInteriorSimulation
{
    class Refrigeraot : InteriorObject
    {
        public Refrigeraot(Point point) : base(point)
        {
            setImg("Refrigerator.jpg");
        }
    }
}
