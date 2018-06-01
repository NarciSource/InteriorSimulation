using Newtonsoft.Json;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace selfInteriorSimulation
{
    

    class InteriorObject : BaseObject
    {
        public Point Center
        {
            get { return new Point(Margin.Left + Width / 2, Margin.Top + Height / 2); }
            set { Margin = new Thickness(value.X - Width / 2, value.Y - Height / 2, 0, 0); }
        }
        public Image Image { get; set; }
        public double Rotate {
            get { var rotate = this.Image.RenderTransform as RotateTransform; return rotate.Angle; }
            set { this.Image.RenderTransform = new RotateTransform(value); }
        }
        

        public InteriorObject()
        {
            this.Image = new Image() { Stretch = System.Windows.Media.Stretch.Fill };
            Rotate = 0;
            border.Child = this.Image;
        }

        public void Build()
        {
            this.MouseDown += (o, e) => { active_notify(this); this.CaptureMouse(); };
            this.MouseMove += (o, e) => {
                if (this.IsMouseCaptured)
                {
                    Point clickPoint = e.GetPosition(canvas);
                    clickPoint = Algorithm.Adjust_to_fit_std(clickPoint);

                    this.Center = new Point(clickPoint.X, clickPoint.Y);
                }
            };
            this.MouseUp += Mouse_Up;

            Canvas.SetZIndex(this, 3);
        }



        public void Mouse_Up(object sender, MouseButtonEventArgs e)
        {
            if (this.IsMouseCaptured)
            {
                foreach (var each in BaseObject.rooms)
                {
                    if (Algorithm.Is_collesion(each, this) == true) return;
                }
                change_notify("Moved", this.Name);
                this.ReleaseMouseCapture();
            }
        }
    }
}
