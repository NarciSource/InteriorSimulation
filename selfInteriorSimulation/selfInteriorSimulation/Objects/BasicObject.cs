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
        public static Canvas canvas { get; set; }
        private string name = string.Empty;
        public string Name { get { return name; } set { ObjectName.Content = value; name = value; } }
        private Label ObjectName = new Label();

        public static List<BasicObject> objects = new List<BasicObject>();
            
        public enum IsType {
            Wall,
            Chair,
            Refrigeraot,
            Sofa,
            Table,
            Tv,
            Washer,
            AttachObject
        };
        public IsType isType;
        public static List<Wall> walls = new List<Wall>();
        public BasicObject(){
            objects.Add(this);
        }
    }
}
