using System;
using System.Windows;

namespace selfInteriorSimulation
{
    class Tv : InteriorObject
    {
        public IsType isType = IsType.Tv;
        public Tv(Point point) : base(point)
        {
            setImg("tv.PNG");
        }
    }
}
