using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace selfInteriorSimulation
{
    class Refrigerator : InteriorObject
    {
        public Refrigerator(Point point) : base(point)
        {
            setImg("Refrigerator.jpg");
        }
    }
}
