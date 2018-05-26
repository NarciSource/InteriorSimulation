using System;
using System.Windows;

namespace selfInteriorSimulation
{
    class Refrigerator : InteriorObject
    {
      
        public Refrigerator(Point point) : base(point)
        {
            isType = IsType.Refrigeraot;
            setImg("refrigerator_dan.PNG");
        }
    }
}
