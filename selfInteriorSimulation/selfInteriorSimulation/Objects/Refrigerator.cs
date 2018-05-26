using System;
using System.Windows;

namespace selfInteriorSimulation
{
    class Refrigerator : InteriorObject
    {

        public IsType isType = IsType.Refrigeraot;
        public Refrigerator(Point point) : base(point)
        {
            setImg("Refrigerator.jpg");
        }
    }
}
