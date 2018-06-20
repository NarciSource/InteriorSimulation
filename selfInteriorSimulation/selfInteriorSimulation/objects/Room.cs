using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;

namespace selfInteriorSimulation
{
    partial class Room : Base
    {
        Canvas floor = new Canvas();
        public Collection<Gate> Gates { get; set; }

        Polygon polygon;
        public Polygon Boundary { get { return polygon; } }
        public PointCollection Points { get { return polygon.Points; } }

        List<Polygon> pointSquares = new List<Polygon>();
        

        public new UIElementCollection Children { get { return floor.Children; } }
        public new Brush BorderBrush
        {
            get { return polygon.Stroke; }
            set { polygon.Stroke = value; foreach (var each in pointSquares) each.Fill = value; }
        }
        public new Thickness BorderThickness
        {
            get { return new Thickness(polygon.StrokeThickness / 3); }
            set { polygon.StrokeThickness = value.Left * 3; }
        }
        public new Brush Background
        {
            get { return polygon.Fill; }
            set { polygon.Fill = value; }
        }




        public Room(PointCollection points)
        {
            border.Child = floor;

            polygon = new Polygon()
            {
                Points = points,
                StrokeThickness = 4,
                Stroke = new SolidColorBrush(Colors.DimGray),

                Fill = new ImageBrush(new BitmapImage(new Uri(@"image\tile.PNG", UriKind.Relative)))
            };
            polygon.MouseDown += (o, e) => {
                active_notify(this);
            };
            floor.Children.Add(polygon);

            Gates = new Collection<Gate>();




            foreach (Point point in points)
            {
                Polygon pointSquare = new Polygon()
                {
                    Points = getSquarePoints(point),
                    StrokeThickness = 1,
                    Fill = new SolidColorBrush(Colors.Green)
                };
                pointSquare.MouseDown += (o, e) => { active_notify(this); pointSquare.CaptureMouse(); };
                pointSquare.MouseMove += (o, e) =>
                {
                    if (pointSquare.IsMouseCaptured)
                    {
                        Point position = e.GetPosition(MetaData.GetInstance.Canvas);
                        position = Algorithm.Adjust_to_fit_std(position);
                        pointSquare.Points = getSquarePoints(position);
                        polygon.Points[pointSquares.IndexOf(pointSquare)] = position;
                    }
                };
                pointSquare.MouseUp += (o, e) => { if (pointSquare.IsMouseCaptured) pointSquare.ReleaseMouseCapture(); };

                Canvas.SetZIndex(pointSquare, 1);

                pointSquares.Add(pointSquare);
                floor.Children.Add(pointSquare);
            }
        }





        public Gate AddDoor()
        {
            Gate door = new Gate(this)
            {
                Type = "Door",
                Length = 100,
                Thickness = this.BorderThickness.Left * 5,
                Color = new SolidColorBrush(Colors.White),
                ModelSource = @"models\gate\Door.obj"
            };
            Canvas.SetZIndex(door, 2);
            Gates.Add(door);
            floor.Children.Add(door);
            return door;
        }
        public void AddDoor(Point from, Point to)
        {
            Gate door = AddDoor();
            door.Line.X1 = from.X;
            door.Line.Y1 = from.Y;
            door.Line.X2 = to.X;
            door.Line.Y2 = to.Y;
            door.Fixed = true;
        }
        public Gate AddWindow()
        {
            Gate window = new Gate(this)
            {
                Type = "Window",
                Length = 80,
                Thickness = this.BorderThickness.Left * 7,
                Color = new SolidColorBrush(Colors.CadetBlue),
                ModelSource = @"models\gate\window.obj"
            };
            Canvas.SetZIndex(window, 5);
            Gates.Add(window);
            floor.Children.Add(window);
            return window;
        }
        public void AddWindow(Point from, Point to)
        {
            Gate window = AddWindow();
            window.Line.X1 = from.X;
            window.Line.Y1 = from.Y;
            window.Line.X2 = to.X;
            window.Line.Y2 = to.Y;
            window.Fixed = true;
        }

        private PointCollection getSquarePoints(Point point)
        {
            PointCollection points = new PointCollection();
            const int gap = 5;
            points.Add(new Point(point.X - gap, point.Y - gap));
            points.Add(new Point(point.X - gap, point.Y + gap));
            points.Add(new Point(point.X + gap, point.Y + gap));
            points.Add(new Point(point.X + gap, point.Y - gap));
            return points;
        }
    }

    class RoomCheckObject : Base { }

    class RoomCover : Base
    {
        Polygon polygon;
        public new Brush Background { get { return polygon.Fill; } set { polygon.Fill = value; } }
        public RoomCover(Room room)
        {
            string saved = XamlWriter.Save(room.Boundary);
            polygon = (Polygon)XamlReader.Load(XmlReader.Create(new StringReader(saved)));
            polygon.Opacity = 0.5;
            border.Child = polygon;
        }
    }
}
