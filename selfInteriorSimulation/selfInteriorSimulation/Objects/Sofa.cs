using System;
using System.Windows;

namespace selfInteriorSimulation.Objects
{
    class Sofa : InteriorObject
    {
        private Sofa(Point point) : base(point)
        {
            setImg("sofa.PNG");
        }
    }
}
