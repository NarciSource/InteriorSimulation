using System;
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
