using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace selfInteriorSimulation
{
    partial class Room : BaseObject
    {
        public class Door : Gate
        {
            public Door(Room room):base(room)
            {
                length = 100;
                line.StrokeThickness = room.BorderThickness.Left * 5;
                line.Stroke = new SolidColorBrush(Colors.White);
            }
        }
        
    }

}
