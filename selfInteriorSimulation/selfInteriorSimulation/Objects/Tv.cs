using System;
using System.Windows;

namespace selfInteriorSimulation
{
    class Tv : InteriorObject
    {
        public Tv() : base()
        {
            isType = IsType.Tv;
            setImg("tv_dan.PNG");
            
        }
    }
}
