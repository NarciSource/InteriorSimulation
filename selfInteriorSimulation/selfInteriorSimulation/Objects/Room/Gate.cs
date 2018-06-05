﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace selfInteriorSimulation
{
    partial class Room : BaseObject
    {
        public class Gate : BaseObject
        {
            Room parent_room;
            bool isFixed;
            public bool Fixed { get { return isFixed; } set { isFixed = value; } }

            protected double length =20;            
            protected Line line = new Line();
            public Line Line { get { return line; } }
            

            public Gate(Room room)
            {
                parent_room = room;
                border.Child = line;

                this.Fixed = false;

                parent_room.MouseDown += (o, e) =>
                {
                    if (this.Fixed == false)
                    {
                        change_notify("Made", this.GetType().Name.ToString());
                        this.Fixed = true;
                    }
                };
                parent_room.MouseMove += Mouse_Move;
            }

            private void Mouse_Move(object sender, MouseEventArgs e)
            {
                if (isFixed) return;

                Point point = e.GetPosition(canvas);

                Line closed_line = Closed_line(point);

                Point middle = new Point();
                double gradient = (closed_line.Y1 - closed_line.Y2) / (closed_line.X1 - closed_line.X2);

                if (Double.IsInfinity(gradient))
                {
                    middle.X = closed_line.X1;
                    middle.Y = point.Y;
                }
                else if (Math.Abs(gradient) < 1)
                {
                    middle.X = point.X;
                    middle.Y = gradient * (middle.X - closed_line.X2) + closed_line.Y2;
                }
                else
                {
                    middle.Y = point.Y;
                    middle.X = (middle.Y - closed_line.Y2) / gradient + closed_line.X2;
                }

                var p = Algorithm.Away_point_to(middle, gradient, length);
                this.line.X1 = p.X;
                this.line.Y1 = p.Y;

                p = Algorithm.Away_point_to(middle, gradient, -length);
                this.line.X2 = p.X;
                this.line.Y2 = p.Y;
            }

            private Line Closed_line(Point point)
            {
                Line line = new Line();

                double min = double.MaxValue;
                for (int i = 0; i < parent_room.Points.Count; i++)
                {
                    var p1 = parent_room.Points[i];
                    var p2 = parent_room.Points[(i + 1) % parent_room.Points.Count];

                    double d = Math.Abs((p2.X - p1.X) * (p1.Y - point.Y) - (p1.X - point.X) * (p2.Y - p1.Y))
                        / Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));


                    if (d < min)
                    {
                        min = d;
                        line.X1 = p1.X;
                        line.Y1 = p1.Y;
                        line.X2 = p2.X;
                        line.Y2 = p2.Y;
                    }
                }
                return line;
            }

        }

    }

}
