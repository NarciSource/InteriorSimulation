using System;
using System.Windows;

namespace selfInteriorSimulation.Objects
{
    class Washer : InteriorObject
    {
        private  Washer(Point point) : base(point)
        {
            setImg("washer.PNG");
        }
    }
}
