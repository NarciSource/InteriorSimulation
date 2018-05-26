using System;
using System.Windows;

namespace selfInteriorSimulation
{
    class Washer : InteriorObject
    {
        public Washer(Point point) : base(point)
        {
            isType = IsType.Washer;
            setImg("washer.PNG");
        }
    }
}
