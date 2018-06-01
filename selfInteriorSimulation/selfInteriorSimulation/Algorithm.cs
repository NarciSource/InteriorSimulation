using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace selfInteriorSimulation
{
    class Algorithm
    {
        static public bool Is_inside(Room dst, Point point)
        {
            int crosses = 0;
            for (int i = 0; i < dst.Points.Count; i++)
            {
                int j = (i + 1) % dst.Points.Count;
                if ((dst.Points[i].Y > point.Y) != (dst.Points[j].Y > point.Y))
                {
                    double atX = (dst.Points[j].X - dst.Points[i].X) * (point.Y - dst.Points[i].Y) / (dst.Points[j].Y - dst.Points[i].Y) + dst.Points[i].X;
                    if (point.X < atX)
                        crosses++;
                }
            }
            return crosses % 2 > 0;
        }

        static public bool Is_collesion(Room room, InteriorObject interiorObject)
        {
            bool[] in_chk = new bool[4];

            in_chk[0] = Is_inside(room, 
                new Point(interiorObject.Center.X - interiorObject.Width / 2, interiorObject.Center.Y - interiorObject.Height / 2));
            in_chk[1] = Is_inside(room,
                new Point(interiorObject.Center.X - interiorObject.Width / 2, interiorObject.Center.Y + interiorObject.Height / 2));
            in_chk[2] = Is_inside(room,
                new Point(interiorObject.Center.X + interiorObject.Width / 2, interiorObject.Center.Y + interiorObject.Height / 2));
            in_chk[3] = Is_inside(room,
                new Point(interiorObject.Center.X + interiorObject.Width / 2, interiorObject.Center.Y + interiorObject.Height / 2));
            
            if (in_chk[0] ^ in_chk[1] || in_chk[2] ^ in_chk[3] || in_chk[1] ^ in_chk[2]) return true;
            else return false;
        }

        static public Point Away_point_to(Point startPoint, double gradient, double length)
        {
            Point result = new Point();
            if (Double.IsInfinity(gradient))
            {
                result.X = startPoint.X;
                result.Y = startPoint.Y + length;
            }
            else if (gradient == 0)
            {
                result.X = startPoint.X + length;
                result.Y = startPoint.Y;
            }
            else
            {
                if (length < 0) result.X = startPoint.X - Math.Sqrt(Math.Pow(length, 2) / (Math.Pow(gradient, 2) + 1));
                else result.X = startPoint.X + Math.Sqrt(Math.Pow(length, 2) / (Math.Pow(gradient, 2) + 1));

                result.Y = gradient * (result.X - startPoint.X) + startPoint.Y;
            }

            return result;
        }

        static public double std_coordinate_size = 16;
        static public Point Adjust_to_fit_std(Point point)
        {
            point.X -= point.X % std_coordinate_size;
            point.Y -= point.Y % std_coordinate_size;
            return point;
        }
    }
}
