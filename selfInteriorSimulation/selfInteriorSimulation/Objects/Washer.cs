using System;
using System.Windows;

namespace selfInteriorSimulation
{
    class Washer : InteriorObject
    {
        public IsType isType = IsType.Washer;
        public Washer(Point point) : base(point)
        {
            setImg("washer.PNG");
        }
    }
}
