using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace selfInteriorSimulation
{
    class WindowObject : AttachObject
    {
        public WindowObject() : base()
        {
            isType = IsType.window;
            setImg("window2.jpg");
            
        }
    }
}
