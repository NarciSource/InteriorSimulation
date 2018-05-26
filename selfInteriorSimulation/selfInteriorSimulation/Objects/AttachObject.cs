using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace selfInteriorSimulation
{
    class AttachObject : InteriorObject
    {
        //
        public AttachObject(Point point) : base(point)
        {
            setImg("sofa.PNG");
        }

        PointCollection points = new PointCollection();
        public override void setPosition(Point point)
        {
            base.setPosition(point);
            points.Clear();
            points.Add(point);
            points.Add(new Point(point.X, point.Y - Height));
            points.Add(new Point(point.X - Width , point.Y - Height));
            points.Add(new Point(point.X - Width, point.Y));

            foreach (Point vertexPoint in points)
            {
                foreach (Wall wall in BasicObject.walls)
                {
                    for (int i = 0; i< wall.points.Count-1; i++)
                    {
                        int dx = Math.Abs((int)(wall.points[i].X - wall.points[i + 1].X));
                        int dy = Math.Abs((int)(wall.points[i].Y - wall.points[i+1].Y));
                        double r = dy / dx;
                        int d = (int) (point.Y - (r * wall.points[i].X));
                    }
                }
            }
        }
    }
}
