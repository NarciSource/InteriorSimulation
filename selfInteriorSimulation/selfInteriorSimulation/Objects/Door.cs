using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace selfInteriorSimulation
{
    class Door : AttachObject
    {
        public Door() : base()
        {
            isType = IsType.door;
            setImg("door.PNG");
            
        }
    }
}
