using System;
using System.Windows;
using static selfInteriorSimulation.BasicObject;

namespace selfInteriorSimulation
{
    class Chair : InteriorObject
    {
        public Chair(Point point) : base(point)
        {
            isType = IsType.Chair;
            setImg("chair2_dan.PNG");
        }
    }
}
