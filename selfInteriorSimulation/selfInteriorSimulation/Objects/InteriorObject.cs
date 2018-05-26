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

        public override void setColor(Color color)
        {
            this.BorderBrush = new SolidColorBrush(color);
        }
        public override void setBorderThickness(double thickness)
        {
            this.BorderThickness = new Thickness(thickness);
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
                {
                    if (!(isType == IsType.door && isType == IsType.window)) {
                        if (MainWindow.isCollesion(each.points, object_points) == true)
                        {
                            //return; 외부 놓임 방지 해야함
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
            Canvas.SetTop(this, point.Y - height/2);
            Canvas.SetLeft(this, point.X - width/2);
            Canvas.SetZIndex(this, 2);

            Canvas.SetTop(NameLabel, (point.Y - height / 2 )+ 100);
            Canvas.SetLeft(NameLabel, (point.X - width / 2 )+ Width/2);
        }

        public void setImg(string src)
        {
            objectImg.Source = new BitmapImage(new Uri(@"image\"+ src, UriKind.Relative));
        }
    }
}
