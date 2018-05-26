using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace selfInteriorSimulation
{
    class InteriorObject : BasicObject
    {
        Image objectImg;
        Point pointInObject;
        bool moveMode = false;
        public InteriorObject(Point point)
        {
            objectImg = new Image();
            setPosition(point);
            objectImg.MouseDown += (o, e) => { moveMode = true; pointInObject = e.GetPosition(objectImg); };
            canvas.MouseMove += (o, e) => {
                Debug.WriteLine(e.GetPosition(canvas));
                Debug.WriteLine(e.GetPosition(objectImg));
                if (moveMode)
                {
                    Point clickPoint = e.GetPosition(canvas);
                    setPosition(new Point(clickPoint.X - pointInObject.X, clickPoint.Y - pointInObject.Y));
                }
            };
            objectImg.MouseUp += (o, e) => { moveMode = false; };
            canvas.Children.Add(objectImg);
        }

        private void setPosition(Point point)
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
