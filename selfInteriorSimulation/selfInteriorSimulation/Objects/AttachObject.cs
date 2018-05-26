using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace selfInteriorSimulation
{
    class AttachObject : InteriorObject
    {
        public AttachObject(Point point) : base(point)
        {
            setImg("sofa.PNG");
        }

        PointCollection points = new PointCollection();
    }
}
