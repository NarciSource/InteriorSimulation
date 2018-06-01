using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static selfInteriorSimulation.BaseObject;

namespace selfInteriorSimulation
{
    using Group = HashSet<Room>;

    public partial class MainWindow : Window
    {
        Collection<Group> groups = new Collection<Group>();

        bool isActiveClosure = false;
        private void Closure_Chk_Click(object sender, RoutedEventArgs e)
        {
            if (isActiveClosure == false)
            {
                isActiveClosure = true;

                Grouping();

                Display();
            }
            else
            {
                isActiveClosure = false;

                Undisplay();
            }
        }




        private void Grouping()
        {
            foreach (var room in rooms)
            {
                var pivot_group = Which_group_of(room);

                foreach (var door in room.Doors)
                {
                    Point point = In_front_of(room, door);

                    foreach (var other_room in rooms)
                    {
                        if (room == other_room) continue;
                        if (Algorithm.Is_inside(other_room, point))
                        {
                            var target_group = Which_group_of(other_room);

                            if (pivot_group == target_group) continue;

                            pivot_group.UnionWith(target_group);
                            groups.Remove(target_group);
                            target_group.Clear();
                        }
                    }
                }

            }
        }




        private Collection<RoomCover> roomCovers = new Collection<RoomCover>();
        private void Display()
        {
            Undisplay();

            bool all_clear = true;
            foreach (var group in groups)
            {
                if (Is_connected_outside(group) == false)
                {
                    foreach (var room in group)
                    {
                        var roomCover = new RoomCover(room);
                        roomCover.Background = new SolidColorBrush(Colors.LightPink);

                        roomCovers.Add(roomCover);
                        canvas.Children.Add(roomCover);
                    }
                    all_clear = false;
                }
                else
                {
                    foreach (var room in group)
                    {
                        var roomCover = new RoomCover(room);
                        roomCover.Background = new SolidColorBrush(Colors.White);

                        roomCovers.Add(roomCover);
                        canvas.Children.Add(roomCover);
                    }
                }
            }
            if (all_clear)
            {
                chk_button.Source = new BitmapImage(new Uri(@"image\success.png", UriKind.Relative));
                closure_button.Background = new SolidColorBrush(Colors.AliceBlue);
            }
            else
            {
                chk_button.Source = new BitmapImage(new Uri(@"image\fail.png", UriKind.Relative));
                closure_button.Background = new SolidColorBrush(Colors.LightPink);
            }

        }

        private void Undisplay()
        {
            foreach (var roomCover in roomCovers)
            {
                canvas.Children.Remove(roomCover);
            }
            roomCovers.Clear();

            chk_button.Source = new BitmapImage(new Uri(@"image\scope.png", UriKind.Relative));
            closure_button.Background = new SolidColorBrush(Colors.Snow);
        }





        private Group Which_group_of(Room room)
        {
            Group result = null;
            foreach (var each in groups)
                if (each.Contains(room))
                {
                    result = each;
                    break;
                }
            if (result == null)
            {
                result = new Group();
                result.Add(room);
                groups.Add(result);
            }

            return result;
        }

        private Point In_front_of(Room room, Room.Door door)
        {
            Point middle = new Point() { X = (door.Line.X1 + door.Line.X2) / 2, Y = (door.Line.Y1 + door.Line.Y2) / 2 };
            double gradient = -(door.Line.X1 - door.Line.X2) / (door.Line.Y1 - door.Line.Y2);

            Point in_front_of_door = Algorithm.Away_point_to(middle, gradient, 10);
            if (Algorithm.Is_inside(room, in_front_of_door))
                in_front_of_door = Algorithm.Away_point_to(middle, gradient, -10);

            return in_front_of_door;
        }

        private bool Is_connected_outside(Group group)
        {
            foreach (var room in group)
            {
                foreach (var door in room.Doors)
                {
                    bool is_connected_outside = true;
                    Point point = In_front_of(room, door);

                    foreach (var other_room in rooms)
                    {
                        if (room == other_room) continue;
                        if (Algorithm.Is_inside(other_room, point))
                        {
                            is_connected_outside = false;
                            break;
                        }
                    }
                    if (is_connected_outside)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    
    }
}
