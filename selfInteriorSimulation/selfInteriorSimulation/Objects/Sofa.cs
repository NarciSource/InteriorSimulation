using System;
using System.Windows;

namespace selfInteriorSimulation
{
    class Sofa : InteriorObject
    {
        public IsType isType = IsType.Sofa;
        public Sofa(Point point) : base(point)
        {
            setImg("sofa.PNG");
        }
    }
}
