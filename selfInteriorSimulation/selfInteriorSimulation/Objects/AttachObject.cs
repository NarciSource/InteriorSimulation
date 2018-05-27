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
            this.MouseDown += (o, e) => {moveMode = true; };
            canvas.MouseMove += (o, e)  => { if (moveMode) { attachMode = getWallToPoint(e.GetPosition(canvas)); }  };
            canvas.MouseUp += (o, e) => { if (moveMode) { moveMode = false; if (!attachMode) { MessageBox.Show(Name+"은 벽 위에 두세요."); } } };
        }
        bool moveMode = false;
        bool attachMode = false;
        int pp = - 50;
        
        private bool getWallToPoint(Point point)
        {
            Point MainPoint = point;
            foreach (Wall wall in BasicObject.walls)
            {
                for (int i = 0; i < wall.points.Count; i++)
                {
                    Point FirstPoint;
                    Point SecondPoint;
                    int dx = 0;
                    int dy = 0;

                    if (i != wall.points.Count - 1)
                    {
                        FirstPoint = wall.points[i];
                        SecondPoint = wall.points[i+1];
                    }
                    else
                    {
                        FirstPoint = wall.points[i];
                        SecondPoint = wall.points[0];
                    }

                    dx = (int)(FirstPoint.X - SecondPoint.X);
                    dy = (int)(FirstPoint.Y - SecondPoint.Y);
                    if (dx == 0)
                    {
                        if (Math.Abs(FirstPoint.X - MainPoint.X) < 50)
                        {
                            if ((!((MainPoint.X < FirstPoint.X + pp) && (MainPoint.X < SecondPoint.X + pp))) && (!(FirstPoint.X < MainPoint.X + pp && SecondPoint.X < MainPoint.X + pp)))
                            {
                                if ((!((MainPoint.Y < FirstPoint.Y + pp) && (MainPoint.Y < SecondPoint.Y + pp))) && (!(FirstPoint.Y < MainPoint.Y + pp && SecondPoint.Y < MainPoint.Y + pp)))
                                {
                                    setAttachPosition(new Point(FirstPoint.X, MainPoint.Y));
                                    return true;
                                }
                            }
                        }
                        else if (Math.Abs(SecondPoint.X - MainPoint.X) < 50)
                        {
                            if ((!((MainPoint.X < FirstPoint.X + pp) && (MainPoint.X < SecondPoint.X + pp))) && (!(FirstPoint.X < MainPoint.X + pp && SecondPoint.X < MainPoint.X + pp)))
                            {
                                if ((!((MainPoint.Y < FirstPoint.Y + pp) && (MainPoint.Y < SecondPoint.Y + pp))) && (!(FirstPoint.Y < MainPoint.Y + pp && SecondPoint.Y < MainPoint.Y + pp)))
                                {
                                    setAttachPosition(new Point(FirstPoint.X, MainPoint.Y));
                                    return true;
                                }
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                    if (dx == 0)
                    {
                        continue;
                    }
                    double r = (double)dy / (double)dx;
                    double d = (int)(FirstPoint.Y - (r * FirstPoint.X));
                    double ro = (Math.Abs(r * MainPoint.X + -1 * MainPoint.Y + d)) / (Math.Pow((Math.Pow(r, 2) + 1), 0.5));
                    if (ro < 50)
                    {
                        if ((!((MainPoint.X < FirstPoint.X + pp) && (MainPoint.X < SecondPoint.X + pp))) && (!(FirstPoint.X < MainPoint.X + pp && SecondPoint.X < MainPoint.X + pp)))
                        {
                            if ((!((MainPoint.Y < FirstPoint.Y + pp) && (MainPoint.Y < SecondPoint.Y + pp))) && (!(FirstPoint.Y < MainPoint.Y + pp && SecondPoint.Y < MainPoint.Y + pp)))
                            {
                                AttachPosition(r, d, MainPoint);
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
        
        private void setAttachPosition(Point point)
        {
            base.point = (new Point(point.X - Width/2, point.Y - Height/2));
        }

        private void AttachPosition(double r, double d, Point point)
        {
            double r1 = -1 / r;
            double d1 = point.Y - r1 * point.X;
            double x = -1 * ((d - d1) / (r - r1));
            double y = r * x + d;
            if (r == 0)
            {
                x = point.X;
                y = d;
            }
            point.X = x;
            point.Y = y;

            setAttachPosition(point);
        }

    }
}
