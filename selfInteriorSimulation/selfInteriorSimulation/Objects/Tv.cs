using System;
using System.Windows;

namespace selfInteriorSimulation
{
    class Tv : InteriorObject
    {
        public Tv(Point point) : base(point)
        {
            isType = IsType.Tv;
            setImg("tv_dan.PNG");
        }
    }
}
