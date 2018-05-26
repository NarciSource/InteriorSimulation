using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace selfInteriorSimulation
{
    class CustomObject : InteriorObject
    {
        public CustomObject(Point point) : base(point)
        {
            isType = IsType.Custom;

        }
    }
}
