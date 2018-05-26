using System;
using System.Windows;

namespace selfInteriorSimulation
{
    class Refrigerator : InteriorObject
    {
        private Refrigerator(Point point) : base(point)
        {
            setImg("Refrigerator.jpg");
        }
    }
}
