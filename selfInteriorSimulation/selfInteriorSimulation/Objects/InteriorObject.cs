using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace selfInteriorSimulation
{
    public delegate void del(object sender);

    class InteriorObject : BasicObject
    {
        static public del notify;

        Image objectImg;
        private Point pointInObject;
        bool moveMode = false;
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
            objectImg.MouseDown += (o, e) => { moveMode = true; pointInObject = e.GetPosition(objectImg); };
            canvas.MouseMove += (o, e) => {
                if (moveMode)
                {
                    Point clickPoint = e.GetPosition(canvas);
                    setPosition(new Point(clickPoint.X - pointInObject.X, clickPoint.Y - pointInObject.Y));
                }
            };
            objectImg.MouseUp += (o, e) => { moveMode = false; };
            canvas.Children.Add(objectImg);
        }

        public virtual void setPosition(Point point)
        {
            Canvas.SetTop(objectImg, point.Y);
            Canvas.SetLeft(objectImg, point.X);
            Canvas.SetZIndex(objectImg, 2);
        }

        public void setImg(string src)
        {
            objectImg.Source = new BitmapImage(new Uri(@"image\"+ src, UriKind.Relative));
        }
    }
}
