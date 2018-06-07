using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;

namespace selfInteriorSimulation
{
    partial class Room : BaseObject
    {
        Canvas floor = new Canvas();

        Polygon polygon;
        public Polygon Boundary { get { return polygon; } }
        public PointCollection Points { get { return polygon.Points; } }

        List<Polygon> pointSquares = new List<Polygon>();

        Collection<Door> doors = new Collection<Door>();
        public Collection<Door> Doors { get { return doors; } }

        Collection<WindowObject> windows = new Collection<WindowObject>();
        public Collection<WindowObject> Windows { get { return windows; } }

        public new UIElementCollection Children { get { return floor.Children; } }
        public new Brush BorderBrush
        {
            get { return polygon.Stroke; }
            set { polygon.Stroke = value;
                foreach (var each in pointSquares) each.Fill = value; }
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
                Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom("#5f6975")),

                Fill = new ImageBrush(new BitmapImage(new Uri(@"image\tile.PNG", UriKind.Relative)))
        };
            polygon.MouseDown += (o, e) => {
                active_notify(this);
            };
            floor.Children.Add(polygon);





            foreach (Point point in points)
            {
                Polygon pointSquare = new Polygon()
                {
                    Points = getSquarePoints(point),
                    Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#00b368")),
                    StrokeThickness = 1
                };
                pointSquare.MouseDown += (o, e) => { active_notify(this); pointSquare.CaptureMouse(); };
                pointSquare.MouseMove += (o, e) =>
                {
                    if (pointSquare.IsMouseCaptured)
                    {
                        Point position = e.GetPosition(canvas);
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


            gRooms.Add(this);
        }


        public Door AddDoor()
        {
            Door door = new Door(this);
            Canvas.SetZIndex(door, 2);
            doors.Add(door);
            floor.Children.Add(door);
            return door;
        }
        public void AddDoor(Point from, Point to)
        {
            Door door = AddDoor();
            door.Line.X1 = from.X;
            door.Line.Y1 = from.Y;
            door.Line.X2 = to.X;
            door.Line.Y2 = to.Y;
            door.Fixed = true;
        }
        public WindowObject AddWindow()
        {
            WindowObject window = new WindowObject(this);
            Canvas.SetZIndex(window, 5);
            windows.Add(window);
            floor.Children.Add(window);
            return window;
        }
        public void AddWindow(Point from, Point to)
        {
            WindowObject window = AddWindow();
            window.Line.X1 = from.X;
            window.Line.Y1 = from.Y;
            window.Line.X2 = to.X;
            window.Line.Y2 = to.Y;
            window.Fixed = true;
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

    class RoomCheckObject : BaseObject { }

    class RoomCover : BaseObject
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
