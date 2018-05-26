using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace selfInteriorSimulation
{
    class Tv : InteriorObject
    {
        public Tv(Point point) : base(point)
        {
            setImg("tv.PNG");
        }
    }
}
