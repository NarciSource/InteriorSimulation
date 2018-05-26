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

        private int width;
        public int Width { get { return width; } set { width = value; objectImg.Width = value; } }
        private int height;
        public int Height { get { return height; } set { height = value; objectImg.Height = value; } }

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
