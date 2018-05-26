using System;
using System.Windows;

namespace selfInteriorSimulation
{
    class Tv : InteriorObject
    {
        private Tv(Point point) : base(point)
        {
            setImg("tv.PNG");
        }
    }
}
