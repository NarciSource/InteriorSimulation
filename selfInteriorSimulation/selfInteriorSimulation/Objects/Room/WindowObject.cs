using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace selfInteriorSimulation
{
    partial class Room : BaseObject
    {
        public class WindowObject : Gate
        {
            public WindowObject(Room room) : base(room)
            {
                length = 80;
                line.StrokeThickness = room.BorderThickness.Left * 7;
                line.Stroke = new SolidColorBrush(Colors.CadetBlue);
            }
        }
    }
}
