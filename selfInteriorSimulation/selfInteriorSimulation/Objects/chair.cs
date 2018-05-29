using System;
using System.Windows;
using static selfInteriorSimulation.BasicObject;

namespace selfInteriorSimulation
{
    class Chair : InteriorObject
    {
        public Chair() : base()
        {
            isType = IsType.Chair;
            setImg("chair2_dan.PNG");
            
        }
    }
}
