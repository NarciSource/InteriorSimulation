using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace selfInteriorSimulation
{
    class AttachObject : InteriorObject
    {
        public AttachObject(Point point) : base(point)
        {
            setImg("door.PNG");
            canvas.MouseMove += (o, e)  => { attachMode = getWallToPoint(e.GetPosition(canvas));  };
        }

        bool attachMode = false;

        public override void setPosition(Point point)
        {
            if (!attachMode)
            {
                base.setPosition(point);
            }

            Point MainPoint = new Point();
            MainPoint.X = point.X + Width / 2;
            MainPoint.Y = point.Y + Height / 2;

            foreach (Wall wall in BasicObject.walls)
            {
                for (int i = 0; i < wall.points.Count; i++)
                {
                    int dx = 0;
                    int dy = 0;
                    if (i != wall.points.Count - 1)
                    {
                        dx = (int)(wall.points[i].X - wall.points[i + 1].X);
                        dy = (int)(wall.points[i].Y - wall.points[i + 1].Y);
                    }
                    else
                    {
                        dx = (int)(wall.points[i].X - wall.points[0].X);
                        dy = (int)(wall.points[i].Y - wall.points[0].Y);
                    }

                    if (dx == 0)
                    {
                        if (Math.Abs(wall.points[i].X - MainPoint.X) < 10)
                        {
                            setImg("sofa.PNG");
                            return;
                        }
                        else if (i != wall.points.Count - 1)
                        {
                            if (Math.Abs(wall.points[i + 1].X - MainPoint.X) < 10)
                            {
                                setImg("sofa.PNG");
                                return;
                            }
                        }
                        else
                        {
                            if (Math.Abs(wall.points[0].X - MainPoint.X) < 10)
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
                    if (dx == 0)
                    {
                        continue;
                    }
                    double r = (double)dy / (double)dx;
                    double d = (int)(wall.points[i].Y - (r * wall.points[i].X));
                    double ro = (Math.Abs(r * MainPoint.X + -1 * MainPoint.Y + d)) / (Math.Pow((Math.Pow(r, 2) + 1), 0.5));
                    if (ro < 10)
                    {
                        AttachPosition(r, d, MainPoint);
                        attachMode = true;
                        return;
                    }
                    else
                    {
                        setImg("door.PNG");
                        attachMode = false;
                    }
                }
            }
        }

        private bool getWallToPoint(Point point)
        {
            Point MainPoint = point;

            foreach (Wall wall in BasicObject.walls)
            {
                for (int i = 0; i < wall.points.Count; i++)
                {
                    int dx = 0;
                    int dy = 0;
                    if (i != wall.points.Count - 1)
                    {
                        dx = (int)(wall.points[i].X - wall.points[i + 1].X);
                        dy = (int)(wall.points[i].Y - wall.points[i + 1].Y);
                    }
                    else
                    {
                        dx = (int)(wall.points[i].X - wall.points[0].X);
                        dy = (int)(wall.points[i].Y - wall.points[0].Y);
                    }

                    if (dx == 0)
                    {
                        if (Math.Abs(wall.points[i].X - MainPoint.X) < 10)
                        {
                            return true;
                        }
                        else if (i != wall.points.Count - 1)
                        {
                            if (Math.Abs(wall.points[i + 1].X - MainPoint.X) < 10)
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Math.Abs(wall.points[0].X - MainPoint.X) < 10)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                    if (dx == 0)
                    {
                        continue;
                    }
                    double r = (double)dy / (double)dx;
                    double d = (int)(wall.points[i].Y - (r * wall.points[i].X));
                    double ro = (Math.Abs(r * MainPoint.X + -1 * MainPoint.Y + d)) / (Math.Pow((Math.Pow(r, 2) + 1), 0.5));
                    if (ro < 50)
                    {
                        AttachPosition(r,d,MainPoint);
                        return true;
                    }
                }
            }
            return false;
        }
        
        private void AttachPosition(double r, double d, Point point)
        {
            double r1 = -1 / r;
            double d1 = point.Y - r1 * point.X;
            double x = -1 * ((d - d1) / (r - r1));
            double y = r * x + d;
            point.X = x;
            point.Y = y;



            foreach(var each in BasicObject.walls)
                if (!MainWindow.is_inside(each.points, new Point(point.X + Width,point.Y + Height)))
                {
                    point.X -= Width;
                    point.Y -= Height;
                }
            else
            //
            Debug.WriteLine("x: " + point.X + " y: " + point.Y);
            base.setPosition(point);
        }

    }
}
