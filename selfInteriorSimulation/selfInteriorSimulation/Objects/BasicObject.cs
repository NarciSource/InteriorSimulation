using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace selfInteriorSimulation
{
    class BasicObject : UIElement
    {
        private string name = string.Empty;
        public string Name { get { return name; } set { ObjectName.Content = value; name = value; } }
        private Label ObjectName = new Label();

        public BasicObject(Canvas canvas){
            canvas.Children.Add(ObjectName);
        }
    }
}
