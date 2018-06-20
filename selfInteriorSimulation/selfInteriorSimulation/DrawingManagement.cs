using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace selfInteriorSimulation
{
    public partial class MainWindow : Window
    {
        Shape shape = null;
        Point dragonpoint;
        Base paintingObject = new NullBasicObject();



        private void Mouse_Left_Down(object sender, MouseButtonEventArgs e)
        {
            if (paintingObject is RoomCheckObject)
            {
                dragonpoint = e.GetPosition(canvas);
                shape = new Rectangle()
                {
                    StrokeThickness = 1,
                    Stroke = new SolidColorBrush(Colors.Red),
                };
                canvas.Children.Add(shape);
            }
        }

        private void Mouse_Move(object sender, MouseEventArgs e)
        {
            Point point = e.GetPosition(canvas);
            Refresh_Status(point);
            point = Algorithm.Adjust_to_fit_std(point);

            if (paintingObject is Furniture)
            {
                ((Furniture)paintingObject).Center = point;
            }

            else if (paintingObject is RoomCheckObject)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    shape.Width = Math.Abs(dragonpoint.X - point.X);
                    shape.Height = Math.Abs(dragonpoint.Y - point.Y);
                    shape.Margin = new Thickness(Math.Min(dragonpoint.X, point.X),
                                                Math.Min(dragonpoint.Y, point.Y), 0, 0);
                }
            }
        }

        private void Mouse_Left_Up(object sender, MouseButtonEventArgs e)
        {
            if (paintingObject is Furniture)
            {
                Furniture realObject = paintingObject.Clone() as Furniture;
                


                canvas.Children.Remove(paintingObject);
                paintingObject = null;

                foreach (var room in Base.allRooms)
                {
                    switch (Algorithm.Which_relation(room, realObject))
                    {
                        case Algorithm.Relation.Inner:

                            room.Children.Add(realObject);
                            Changed("Made", realObject.Name);
                            return;


                        case Algorithm.Relation.Collesion:
                            
                            return;
                    }
                }

                canvas.Children.Add(realObject);
                Changed("Made", realObject.Name);
            }

            else if (paintingObject is RoomCheckObject)
            {
                Point point = e.GetPosition(canvas);
                PointCollection points = new PointCollection();
                
                points.Add(new Point(Math.Min(dragonpoint.X, point.X), Math.Min(dragonpoint.Y, point.Y)));
                points.Add(new Point(Math.Min(dragonpoint.X, point.X), Math.Max(dragonpoint.Y, point.Y)));
                points.Add(new Point(Math.Max(dragonpoint.X, point.X), Math.Max(dragonpoint.Y, point.Y)));
                points.Add(new Point(Math.Max(dragonpoint.X, point.X), Math.Min(dragonpoint.Y, point.Y)));

                Room room = new Room(points);
                canvas.Children.Add(room);
                canvas.Children.Remove(shape);

                Changed("Made", "Room");
            }
        }






        private void Room_Click(object sender, RoutedEventArgs e)
        { 
            switch (((Button)sender).Name.ToString())
            {
                case "room_button":
                    paintingObject = new RoomCheckObject();
                    return;
                case "door_button":
                    ((Room)activeObject).AddDoor();
                    return;
                case "window_button":
                    ((Room)activeObject).AddWindow();
                    return;
            }
            
            canvas.Children.Add(paintingObject);
        }





        private Base activeObject = null;
        private void Active(object sender)
        {

            if (activeObject != null) // Previous active object cleanup.
            {
                if (!(activeObject is Room))
                {
                    activeObject.BorderThickness = new Thickness(0);
                    activeObject.BorderBrush = new SolidColorBrush(Colors.Black);
                }
                else
                {
                    ((Room)activeObject).BorderBrush = new SolidColorBrush(Colors.Black);
                }
            }

            activeObject = (Base)sender;

            if (sender is Room)
            {
                door_button.IsEnabled = true;
                window_button.IsEnabled = true;
                setting_height.IsEnabled = false;
                setting_width.IsEnabled = false;
                setting_angle.IsEnabled = false;
                setting_material.IsEnabled = true;
                setting_thickness.IsEnabled = true;
                setting_name.Text = activeObject.Name.ToString();
                setting_thickness.Text = ((Room)activeObject).BorderThickness.Left.ToString();
                ((Room)activeObject).BorderBrush = new SolidColorBrush(Colors.Red);
            }
            else
            {
                door_button.IsEnabled = false;
                window_button.IsEnabled = false;
                setting_height.IsEnabled = true;
                setting_width.IsEnabled = true;
                setting_angle.IsEnabled = true;
                setting_material.IsEnabled = false;
                setting_thickness.IsEnabled = false;
                setting_name.Text = activeObject.Name.ToString();
                setting_width.Text = activeObject.Width.ToString();
                setting_height.Text = activeObject.Height.ToString();
                activeObject.BorderThickness = new Thickness(3);
                activeObject.BorderBrush = new SolidColorBrush(Colors.Red);
            }

            if (paintingObject is RoomCheckObject)
            {
                paintingObject = new NullBasicObject();
            }
        }









        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (activeObject != null)
            {
                switch (e.Key)
                {
                    case Key.Delete:
                        canvas.Children.Remove(activeObject);
                        Changed("Delete", activeObject.Name);
                        activeObject = null;
                        break;

                    case Key.LeftCtrl: // Copy
                        if (activeObject is Furniture)
                        {
                            canvas.Children.Add((paintingObject as Furniture).Clone() as Furniture);

                            Changed("Copy", activeObject.Name);
                        }
                        break;
                }
            }
        }






        private void Refresh_Status(Point position)
        {
            point_position.Content = position.ToString();
            if (activeObject != null)
                object_type.Content = activeObject.ToString();
            undo_times.Content = recordStay.ToString();
        }

    }
}
