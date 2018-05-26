using Newtonsoft.Json;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace selfInteriorSimulation
{
    public delegate void del(object sender);

    class InteriorObject : BasicObject
    {
        static public del notify;

        Image objectImg;
        private Point pointInObject;
        public Point point;

        public int Width;
        public int width { get { return Width; } set { Width = value; objectImg.Width = value; } }
        public int Height;
        public int height { get { return Height; } set { Height = value;  objectImg.Height = value; } }

        public override void setColor(Color color)
        {
            this.BorderBrush = new SolidColorBrush(color);
        }
        public override void setBorderThickness(double thickness)
        {
            this.BorderThickness = new Thickness(thickness);
        }
        public void setRotate(double angle)
        {
            objectImg.RenderTransform = new RotateTransform(angle);
        }

        public InteriorObject(Point point)
        {
            objectImg = new Image();
            this.point = point;
            setPosition(point);
            this.MouseDown += (o, e) => { notify(this); this.CaptureMouse(); pointInObject = e.GetPosition(objectImg); };
            this.MouseMove += (o, e) => {
                if (this.IsMouseCaptured)
                {
                    Point clickPoint = e.GetPosition(canvas);
                    setPosition(new Point(clickPoint.X - pointInObject.X, clickPoint.Y - pointInObject.Y));
                }
            };
            this.MouseUp += (o, e) => {
                Point p = e.GetPosition(canvas);
                PointCollection object_points = new PointCollection();
                object_points.Add(new Point(p.X - width / 2, p.Y - height / 2));
                object_points.Add(new Point(p.X + width / 2, p.Y - height / 2));
                object_points.Add(new Point(p.X - width / 2, p.Y + height / 2));
                object_points.Add(new Point(p.X + width / 2, p.Y + height / 2));

                foreach (var each in BasicObject.walls)
                    if (MainWindow.isCollesion(each.points, object_points) == true) return;


                this.ReleaseMouseCapture();
            };
            this.Child = objectImg;
            canvas.Children.Add(this);
        }

        public virtual void setPosition(Point point)
        {
            Canvas.SetTop(this, point.Y);
            Canvas.SetLeft(this, point.X);
            Canvas.SetZIndex(this, 2);
        }

        public void setImg(string src)
        {
            objectImg.Source = new BitmapImage(new Uri(@"image\"+ src, UriKind.Relative));
        }
    }
}
