using System;
using System.Windows;

namespace selfInteriorSimulation.Objects
{
    class Chair : InteriorObject
    {
        private Chair(Point point) : base(point)
        {
            setImg("chair.PNG");
        }
    }
}
