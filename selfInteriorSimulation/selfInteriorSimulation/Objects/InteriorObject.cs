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

        public Image objectImg;
        private Point pointInObject;
        private Point mpoint;
        public Point point { get { return mpoint; } set { setPosition(value); mpoint = value; } }

        private int Width;
        public int width { get { return Width; } set { Width = value; objectImg.Width = value; } }
        private int Height;
        public int height { get { return Height; } set { Height = value;  objectImg.Height = value; } }

        private double rotate;
        public override void setColor(Color color)
        {
            this.BorderBrush = new SolidColorBrush(color);
        }
        public double getBorderThicknessDbl()
        {
            return Double.Parse(this.BorderThickness.ToString());
        }
        public override void setBorderThickness(double thickness)
        {
            this.BorderThickness = new Thickness(thickness);
        }
        public double getRotate()
        {
            return this.rotate;
        }
        public void setRotate(double angle)
        {
            this.rotate = angle;
            objectImg.RenderTransform = new RotateTransform(angle);
        }

        public InteriorObject(Point point)
        {
            Point FirstPoint = new Point();
            objectImg = new Image();
            this.point = point;
            setPosition(point);
            setBorderThickness(getBorderThicknessDbl());
            setRotate(getBorderThicknessDbl());
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
                {
                    if (!(isType == IsType.door && isType == IsType.window)) {
                        if (MainWindow.isCollesion(each.points, object_points) == true)
                        {
                            setPosition( FirstPoint);
                        }
                    }
                }

                this.ReleaseMouseCapture();
            };
            this.Child = objectImg;
            canvas.Children.Add(this);

            Canvas.SetTop(NameLabel,point.Y - 10);
            Canvas.SetLeft(NameLabel, point.X + width/2);
            Canvas.SetZIndex(NameLabel, 3);
            canvas.Children.Add(NameLabel);
        }

        public virtual void setPosition(Point point)
        {
            Canvas.SetTop(this, point.Y);
            Canvas.SetLeft(this, point.X);
            Canvas.SetZIndex(this, 2);

            Canvas.SetTop(NameLabel, point.Y - 20);
            Canvas.SetLeft(NameLabel, point.X + Width/2);
        }

        public void setImg(string src)
        {
            objectImg.Source = new BitmapImage(new Uri(@"image\"+ src, UriKind.Relative));
        }
    }
}
