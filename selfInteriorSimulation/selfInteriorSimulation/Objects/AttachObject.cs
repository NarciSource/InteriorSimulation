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
        public AttachObject(Point point) : base(point)
        {
            setImg("sofa.PNG");
        }

        public override void setPosition(Point point)
        {
            base.setPosition(point);

            PointCollection points = new PointCollection();
            points.Clear();
            points.Add(point);
            points.Add(new Point(point.X, point.Y + Height));
            points.Add(new Point(point.X + Width , point.Y + Height));
            points.Add(new Point(point.X + Width, point.Y));

            
            foreach (Point vertexPoint in points)
            {
                foreach (Wall wall in BasicObject.walls)
                {
                    for (int i = 0; i< wall.points.Count -1; i++)
                    {
                        int dx = 0;
                        int dy = 0;
                        if (i != wall.points.Count-1)
                        {
                            dx = Math.Abs((int)(wall.points[i].X - wall.points[i+1].X));
                            dy = Math.Abs((int)(wall.points[i].Y - wall.points[i+1].Y));
                        }
                        else
                        {
                            dx = Math.Abs((int)(wall.points[i].X - wall.points[0].X));
                            dy = Math.Abs((int)(wall.points[i].Y - wall.points[0].Y));
                        }

                        if(dx == 0)
                        {
                            if (Math.Abs(wall.points[i].Y - vertexPoint.Y) < 10)
                            {
                                setImg("sofa.PNG");
                                return;
                            }
                            else if (i != wall.points.Count - 1)
                            { 
                                if (Math.Abs(wall.points[i + 1].Y - vertexPoint.Y) < 10)
                                {
                                    setImg("sofa.PNG");
                                    return;
                                }
                            }
                            else
                            {
                                if (Math.Abs(wall.points[0].Y - vertexPoint.Y) < 10)
                                {
                                    setImg("sofa.PNG");
                                    return;
                                }
                                else
                                {
                                    return;
                                }
                            }
                        }
                        if(dx == 0)
                        {
                            return;
                        }
                        double r = (double)dy / (double)dx;
                        int d = (int) (points[i].Y - (r * wall.points[i].X));
                        int ro = (Math.Abs((int)(r * vertexPoint.X + vertexPoint.Y + d))) / (int)(Math.Pow((Math.Pow(r,2)+1),0.5));
                        if (ro < 10)
                        {
                            setImg("sofa.PNG");
                        }
                        else
                        {
                            setImg("Refrigerator.jpg");
                        }
                    }
                }
            }
        }
    }
}
