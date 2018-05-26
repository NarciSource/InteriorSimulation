using System;
using System.Collections.Generic;
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
    class Wall : BasicObject
    {
        private PointCollection points = new PointCollection();
        Polygon polygon;
        List<Polygon> pointSquares = new List<Polygon>();
        int movePointNum;

        bool moveMode = false;

        public Wall(PointCollection points)
        {
            this.points = points;
            polygon = new Polygon()
            {
                Points = this.points,
                StrokeThickness = 3,
                Stroke = new SolidColorBrush(Colors.Red),
                Fill = new ImageBrush { ImageSource = new BitmapImage(new Uri(@"image/\ground1.PNG", UriKind.Relative))},
                //Fill = new SolidColorBrush(Colors.Brown)
            };
            canvas.Children.Add(polygon);
            foreach (Point point in points)
            {
                Polygon pointSquare = new Polygon()
                {
                    Points = getSquarePoints(point),
                    Fill = new SolidColorBrush(Colors.Blue),
                    StrokeThickness = 1
                };
                pointSquare.MouseDown += (o, e) =>
                {
                    moveMode = true;
                    for (int i = 0; i < points.Count; i++)
                    {
                        if (Math.Abs( e.GetPosition(canvas).X - points[i].X) < 10 && Math.Abs(e.GetPosition(canvas).Y - points[i].Y) < 10)
                        {
                            movePointNum = i;
                        }
                    }
                };
                pointSquare.MouseUp += (o, e) => { moveMode=false; };
                Canvas.SetZIndex(pointSquare,1);
                pointSquares.Add(pointSquare);
                canvas.Children.Add(pointSquare);
            }
            canvas.MouseMove += Canvas_MouseMove; ;
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (moveMode)
            {
                pointSquares[movePointNum].Points = getSquarePoints(e.GetPosition(canvas));
                polygon.Points[movePointNum] = e.GetPosition(canvas);
            }
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
