using System;
using System.Windows;

namespace selfInteriorSimulation
{
    class Washer : InteriorObject
    {
        public Washer() : base()
        {
            isType = IsType.Washer;
            setImg("washer.PNG");
            
        }
    }
}
