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
    

    class InteriorObject : BasicObject
    {
        public Image objectImg = new Image();
        private Point mpoint;

        public Point Point { get { return mpoint; } set { setPosition(value); mpoint = value; } }
        public Image Image { get { return objectImg; } set { objectImg = value; } }
        public double rotate {
            get { var rotate = this.objectImg.RenderTransform as RotateTransform; return rotate.Angle; }
            set { objectImg.RenderTransform = new RotateTransform(value); }
        }
        

        public InteriorObject()
        {
            rotate = 0;
            this.MouseDown += (o, e) => { active_notify(this); this.CaptureMouse();};
            this.MouseMove += (o, e) => {
                if (this.IsMouseCaptured)
                {
                    Point clickPoint = e.GetPosition(canvas);
                    setPosition(new Point(clickPoint.X , clickPoint.Y));
                }
            };
            this.MouseUp += Mouse_Up;

            Canvas.SetZIndex(this, 2);
            border.Child = objectImg;
        }

        private void setPosition(Point point)
        {
            mpoint = point;
            Canvas.SetTop(this, point.Y - this.Height / 2);
            Canvas.SetLeft(this, point.X - this.Width / 2);
        }

        public void setImg(string src)
        {
            objectImg.Source = new BitmapImage(new Uri(@"image\"+ src, UriKind.Relative));
        }




        public void Mouse_Up(object sender, MouseButtonEventArgs e)
        {
            if (this.IsMouseCaptured)
            {
                Point p = e.GetPosition(null);
                PointCollection object_points = new PointCollection();
                object_points.Add(new Point(p.X - Width / 2, p.Y - Height / 2));
                object_points.Add(new Point(p.X + Width / 2, p.Y - Height / 2));
                object_points.Add(new Point(p.X - Width / 2, p.Y + Height / 2));
                object_points.Add(new Point(p.X + Width / 2, p.Y + Height / 2));

                foreach (var each in BasicObject.rooms)
                {
                    if (!(isType == IsType.door && isType == IsType.window))
                    {
                        if (MainWindow.isCollesion(each.points, object_points) == true)
                        {
                            return;
                        }
                    }
                }
                change_notify("Moved", this.Name);
                this.ReleaseMouseCapture();
            }
        }
    }
}
