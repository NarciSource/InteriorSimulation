using System;
using System.Windows;

namespace selfInteriorSimulation
{
    class Refrigerator : InteriorObject
    {
      
        public Refrigerator() : base()
        {
            isType = IsType.Refrigerator;
            setImg("refrigerator_dan.PNG");
            
        }
    }
}
