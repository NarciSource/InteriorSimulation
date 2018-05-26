using Newtonsoft.Json;
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

        public int Width;
        public int width { get { return Width; } set { Width = value; objectImg.Width = value; } }
        public int Height;
        public int height { get { return Height; } set { Height = value;  objectImg.Height = value; } }

        public InteriorObject(Point point)
        {
            objectImg = new Image();
            this.point = point;
            this.setPosition(point);
            objectImg.MouseDown += (o, e) => { notify(this); moveMode = true; pointInObject = e.GetPosition(objectImg); };
            canvas.MouseMove += (o, e) => {
                if (moveMode)
                {
                    Point clickPoint = e.GetPosition(canvas);
                    this.setPosition(new Point(clickPoint.X - pointInObject.X, clickPoint.Y - pointInObject.Y));
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
