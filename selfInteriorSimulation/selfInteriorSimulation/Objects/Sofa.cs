using System;
using System.Windows;

namespace selfInteriorSimulation
{
    class Sofa : InteriorObject
    {
        public Sofa() : base()
        {
            isType = IsType.Sofa;
            setImg("sofa_dan.PNG");
            
        }
    }
}
