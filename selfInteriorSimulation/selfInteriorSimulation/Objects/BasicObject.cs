using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace selfInteriorSimulation
{
    interface CL : ICloneable
    {

    }


    abstract class BasicObject : Border, CL
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
            Washer
        };
        public IsType isType;
        public static List<Wall> walls = new List<Wall>();
        public BasicObject(){
            objects.Add(this);
        }

        public abstract void setColor(Color color);
        public abstract void setBorderThickness(double thickness);

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
