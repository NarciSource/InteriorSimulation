using System;
using System.Windows;

namespace selfInteriorSimulation
{
    class Sofa : InteriorObject
    {
        public Sofa(Point point) : base(point)
        {
            setImg("sofa.PNG");
        }
    }
}
