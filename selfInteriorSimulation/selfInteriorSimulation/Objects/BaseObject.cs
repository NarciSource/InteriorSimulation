using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace selfInteriorSimulation
{
    public delegate void del(object sender);
    public delegate void del2(string command, string name);

    abstract class Base : DockPanel, ICloneable
    {
        static public del active_notify;
        static public del2 change_notify;

        /* global */
        public static List<Room> allRooms = new List<Room>();
        public static Canvas canvas { get; set; }


        protected Border border = new Border();
        protected TextBlock name = new TextBlock()
        {
            Height = 15,
            HorizontalAlignment = HorizontalAlignment.Center
        };

        public String Type { get; set; }
        public new string Name { get { return name.Text; } set { name.Text = value;} }
        public Thickness BorderThickness { get { return border.BorderThickness; } set { border.BorderThickness = value; } }
        public Brush BorderBrush { get { return border.BorderBrush; } set { border.BorderBrush = value; } }
        public new double Width { get { return border.Width; } set { border.Width = value; } }
        public new double Height { get { return border.Height; } set { border.Height = value; } }
                

        
        public Base(){
            DockPanel.SetDock(border, Dock.Top);
            this.Children.Add(border);

            DockPanel.SetDock(name, Dock.Bottom);
            this.Children.Add(name);
        }

        public virtual object Clone()
        {
            throw new NotImplementedException();
        }
    }

    class NullBasicObject : Base { }
}
