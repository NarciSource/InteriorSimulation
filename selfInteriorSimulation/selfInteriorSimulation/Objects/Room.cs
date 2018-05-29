using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace selfInteriorSimulation
{
    class Room : BasicObject
    {
        Canvas canv = new Canvas();

        Polygon polygon;
        List<Polygon> pointSquares = new List<Polygon>();
        public PointCollection points = new PointCollection();

        public new UIElementCollection Children { get { return canv.Children; } }
        public new Brush BorderBrush
        {
            get { return polygon.Stroke; }
            set {
                polygon.Stroke = value;
                foreach (var each in pointSquares) each.Fill = value;
            }
        }
        public new Thickness BorderThickness
        {
            get { return new Thickness(polygon.StrokeThickness / 3); }
            set { polygon.StrokeThickness = value.Left * 3; }
        }

        public Room() { }
        public Room(PointCollection points)
        {
            isType = IsType.Room;

            this.points = points;
            polygon = new Polygon()
            {
                Points = this.points,
                StrokeThickness = 4,
                Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom("#5f6975")),

                Fill = new ImageBrush(new BitmapImage(new Uri(@"pack://application:,,,/image/tile.PNG")))
            };
            polygon.MouseDown += (o, e) => { active_notify(this); };
            canv.Children.Add(polygon);





            foreach (Point point in points)
            {
                Polygon pointSquare = new Polygon()
                {
                    Points = getSquarePoints(point),
                    Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#00b368")),
                    StrokeThickness = 1
                };
                pointSquare.MouseDown += (o, e) => { pointSquare.CaptureMouse(); };
                pointSquare.MouseMove += (o, e) =>
                {
                    if (pointSquare.IsMouseCaptured)
                    {
                        pointSquare.Points = getSquarePoints(e.GetPosition(canvas));
                        polygon.Points[pointSquares.IndexOf(pointSquare)] = e.GetPosition(canvas);
                    }
                };
                pointSquare.MouseUp += (o, e) => { if (pointSquare.IsMouseCaptured) pointSquare.ReleaseMouseCapture(); };

                Canvas.SetZIndex(pointSquare, 1);

                pointSquares.Add(pointSquare);
                canv.Children.Add(pointSquare);
            }


            BasicObject.rooms.Add(this);

            border.Child = canv;
        }

        public void setImg(Uri uri)
        {
            BitmapImage img = new BitmapImage(uri);
            polygon.Fill = new ImageBrush(img);
        }

        private PointCollection getSquarePoints(Point point)
        {
            PointCollection points = new PointCollection();
            int gap = 5;
            points.Add(new Point(point.X - gap, point.Y - gap));
            points.Add(new Point(point.X - gap, point.Y + gap));
            points.Add(new Point(point.X + gap, point.Y + gap));
            points.Add(new Point(point.X + gap, point.Y - gap));
            return points;
        }
    }
}
