using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace selfInteriorSimulation
{
    using Group = HashSet<Room>;

    class ClosureExamination
    {
        Collection<Group> groups = new Collection<Group>();
        SolidColorBrush successColor = new SolidColorBrush(Colors.White);
        SolidColorBrush failColor = new SolidColorBrush(Colors.LightPink);

        public Action Success_layout { get; set; }
        public Action Fail_layout { get; set; }
        public Action Init_layout { get; set; }

        public List<Room> AllRooms { get; set; }


        public void Grouping()
        {
            foreach (var room in AllRooms)
            {
                var pivot_group = Which_group_of(room);

                foreach (var door in (from gate in room.Gates where gate.Type == "Door" select gate))
                {
                    Point point = In_front_of(room, door);

                    foreach (var other_room in AllRooms)
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

        public void Ungrouping()
        {
            groups.Clear();
        }




        private Collection<RoomCover> roomCovers = new Collection<RoomCover>();
        public void Display()
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
                        roomCover.Background = failColor;

                        roomCovers.Add(roomCover);
                        MetaData.GetInstance.Canvas.Children.Add(roomCover);
                    }
                    all_clear = false;
                }
                else
                {
                    foreach (var room in group)
                    {
                        var roomCover = new RoomCover(room);
                        roomCover.Background = successColor;

                        roomCovers.Add(roomCover);
                        MetaData.GetInstance.Canvas.Children.Add(roomCover);
                    }
                }
            }
            if (all_clear)
            {
                Success_layout();
            }
            else
            {
                Fail_layout();
            }

        }

        public void Undisplay()
        {
            roomCovers.ToList().ForEach(
                roomCover => MetaData.GetInstance.Canvas.Children.Remove(roomCover));

            roomCovers.Clear();

            Init_layout();
        }





        private Group Which_group_of(Room room)
        {
            Group result = null;

            try
            {
                result = groups.Where((each) => { return each.Contains(room); }).Single();
            }
            catch (Exception)
            {
                result = new Group();
                result.Add(room);
                groups.Add(result);
            }

            return result;
        }

        private Point In_front_of(Room room, Room.Gate door)
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
                foreach (var door in (from gate in room.Gates where gate.Type == "Door" select gate))
                {
                    bool is_connected_outside = true;
                    Point point = In_front_of(room, door);

                    foreach (var other_room in AllRooms)
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
