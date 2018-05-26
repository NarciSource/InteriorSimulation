using System;
using System.Windows;

namespace selfInteriorSimulation
{
    class Chair : InteriorObject
    {
        public IsType isType = IsType.Chair;
        public Chair(Point point) : base(point)
        {
            setImg("chair.PNG");
        }
    }
}
