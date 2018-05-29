using Microsoft.Win32;
using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static selfInteriorSimulation.BasicObject;

namespace selfInteriorSimulation
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();

            BasicObject.active_notify += Active;
            BasicObject.change_notify += Changed;

            New_Click(new object(), new RoutedEventArgs());
        }

        

        private BasicObject activeObject = null;
        private void Active(object sender)
        {
            if (activeObject != null)
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
            
            activeObject = (BasicObject)sender;
            SettingDock.Visibility = Visibility.Visible;
            activeObject.BorderBrush = new SolidColorBrush(Colors.Red);
            setting_name.Text = activeObject.Name.ToString();

            if (sender is Room)
            {
                setting_height.IsEnabled = false;
                setting_width.IsEnabled = false;
                setting_angle.IsEnabled = false;
                setting_material.IsEnabled = true;
                setting_thickness.IsEnabled = true;
                setting_thickness.Text = ((Room)activeObject).BorderThickness.Left.ToString();
                ((Room)activeObject).BorderBrush = new SolidColorBrush(Colors.Red);
            }
            else
            {
                setting_height.IsEnabled = true;
                setting_width.IsEnabled = true;
                setting_angle.IsEnabled = true;
                setting_material.IsEnabled = false;
                setting_thickness.IsEnabled = false;
                setting_width.Text = activeObject.ActualWidth.ToString();
                setting_height.Text = activeObject.ActualHeight.ToString();
                activeObject.BorderThickness = new Thickness(3);
            }
        }





        enum Painting_Mode
        {
            Default,
            Room,
            Bottom,
            Refre,
            TV,
            Table,
            Sofa,
            Chair,
            Window,
            Custom
        };
        Painting_Mode painting_mode = Painting_Mode.Default;


        Shape shape = null;
        PointCollection points = new PointCollection();
        BasicObject selectedObject = null;

 



        private void Mouse_Move(object sender, MouseEventArgs e)
        {
            if (selectedObject == null) return;

            Point point = e.GetPosition(canvas);

            Refresh_Status(point, canvas.Children.Count);

            switch (painting_mode)
            {
                case Painting_Mode.Chair:
                case Painting_Mode.Refre:
                case Painting_Mode.Sofa:
                case Painting_Mode.Table:
                case Painting_Mode.TV:
                case Painting_Mode.Custom:
                    ((InteriorObject)selectedObject).Point = new Point(point.X, point.Y);
                    return;
            }

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (painting_mode == Painting_Mode.Room)
                {
                    shape.Width = Math.Abs(points[0].X - point.X);
                    shape.Height = Math.Abs(points[0].Y - point.Y);
                    shape.Margin = new Thickness(Math.Min(points[0].X, point.X),
                                                Math.Min(points[0].Y, point.Y), 0, 0);
                }
            }
        }

        private void Mouse_Left_Down(object sender, MouseButtonEventArgs e)
        {
            if (selectedObject == null) return;

            const int object_width = 100;
            const int object_height = 130;

            Point point = e.GetPosition(canvas);
            points = new PointCollection();
            points.Add(point);


            switch (selectedObject.isType)
            {
                case BasicObject.IsType.Chair:
                case BasicObject.IsType.Refrigerator:
                case BasicObject.IsType.Sofa:
                case BasicObject.IsType.Table:
                case BasicObject.IsType.Tv:
                case BasicObject.IsType.Custom:

                    PointCollection object_points = new PointCollection();
                    object_points.Add(new Point(point.X - object_width / 2, point.Y - object_height / 2));
                    object_points.Add(new Point(point.X + object_width / 2, point.Y - object_height / 2));
                    object_points.Add(new Point(point.X - object_width / 2, point.Y + object_height / 2));
                    object_points.Add(new Point(point.X + object_width / 2, point.Y + object_height / 2));

                    foreach (var each in BasicObject.rooms)
                        if (isCollesion(each.points, object_points) == true)
                        {
                            canvas.Children.Remove(activeObject);
                            selectedObject = null;
                            return;
                        }

                    change_notify("New", selectedObject.Name);
                    selectedObject = null;
                    break;


                case BasicObject.IsType.Room:
                    shape = new Rectangle()
                    {
                        StrokeThickness = 1,
                        Stroke = new SolidColorBrush(Colors.Red),
                    };
                    
                    canvas.Children.Add(shape);
                    break;
            }
        }
    
        private void Mouse_Left_Up(object sender, MouseButtonEventArgs e)
        {
            Point point = e.GetPosition(canvas);
            if(selectedObject==null) return;

            switch (selectedObject.isType)
            {
                case BasicObject.IsType.Room:
                    points.Add(new Point(points[0].X, point.Y));
                    points.Add(new Point(point.X, point.Y));
                    points.Add(new Point(point.X, points[0].Y));

                    new Room(points);

                    canvas.Children.Remove(shape);
                    Changed("New", "Room");
                    selectedObject = null;
                    break;

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

                        activeObject = null;
                        break;
                    case Key.LeftCtrl:
                        if (activeObject is Room) break;

                        switch (activeObject.isType)
                        {
                            case BasicObject.IsType.Chair:
                                selectedObject = new Chair();
                                painting_mode = Painting_Mode.Chair;
                                break;
                            case BasicObject.IsType.Sofa:
                                selectedObject = new Sofa();
                                painting_mode = Painting_Mode.Sofa;
                                break;
                            case BasicObject.IsType.Table:
                                selectedObject = new Table();
                                painting_mode = Painting_Mode.Table;
                                break;
                            case BasicObject.IsType.Tv:
                                selectedObject = new Tv();
                                painting_mode = Painting_Mode.TV;
                                break;
                            case BasicObject.IsType.Refrigerator:
                                selectedObject = new Refrigerator();
                                painting_mode = Painting_Mode.Refre;
                                break;
                            case BasicObject.IsType.Custom:
                                selectedObject = new CustomObject();
                                painting_mode = Painting_Mode.Custom;
                                break;

                        }
                        selectedObject.Width = ((InteriorObject)activeObject).Width;
                        selectedObject.Height = ((InteriorObject)activeObject).Height;

                        break;
                }
            }
        }

        private void Refresh_Status(Point position, int undos)
        {
            point_position.Content = position.ToString();
            if(activeObject!=null)
                object_type.Content = activeObject.ToString();
            undo_times.Content = undos.ToString();
        }






        private void Wall_Click(object sender, RoutedEventArgs e)
        {
            selectedObject = new Room();
            painting_mode = Painting_Mode.Room;
        }

        private void Window_Click(object sender, RoutedEventArgs e)
        {
            new WindowObject()
            {
                Width = 100,
                Height = 100,
                Name = "Window"
            };
        }

        private void Custom_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Png Image|*.png";
            openFileDialog.Title = "Open an Image File";
            
            Nullable<bool> result = openFileDialog.ShowDialog();

            try
            {
                Uri uri = new Uri(openFileDialog.FileName);
                alret w = new alret();
                if (w.ShowDialog() == true)
                {
                    selectedObject = new CustomObject()
                    {
                        Width = w.cWidth,
                        Height = w.cHeight,
                        Name = w.cName,
                        Image = new Image() { Source = new BitmapImage(uri) }
                    };
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("파일을 열 수 없습니다. " + ex.Message);
            }
            painting_mode = Painting_Mode.Custom;
        }

        private void Object_Click(object sender, RoutedEventArgs e)
        {
            switch (((Button)sender).Name.ToString())
            {
                case "refre_button":
                    selectedObject = new Refrigerator() { Name="냉장고", Width = 90, Height = 90 };
                    painting_mode = Painting_Mode.Refre;
                    break;
                case "sofa_button":
                    selectedObject = new Sofa() { Name = "소파", Width = 170, Height = 100 };
                    painting_mode = Painting_Mode.Sofa;
                    break;
                case "chair_button":
                    selectedObject = new Chair() { Name = "의자", Width = 70, Height = 70 };
                    painting_mode = Painting_Mode.Chair;
                    break;
                case "table_button":
                    selectedObject = new Table() { Name = "책상", Width = 140, Height = 100 };
                    painting_mode = Painting_Mode.Table;
                    break;
                case "tv_button":
                    selectedObject = new Tv() { Name = "TV", Width = 200, Height = 50 };
                    painting_mode = Painting_Mode.TV;
                    break;
            }
        }




        static public bool is_inside(PointCollection dst, Point point)
        {
            int crosses = 0;
            for (int i = 0; i < dst.Count; i++)
            {
                int j = (i + 1) % dst.Count;
                if ((dst[i].Y > point.Y) != (dst[j].Y > point.Y))
                {
                    double atX = (dst[j].X - dst[i].X) * (point.Y - dst[i].Y) / (dst[j].Y - dst[i].Y) + dst[i].X;
                    if (point.X < atX)
                        crosses++;
                }
            }
            return crosses % 2 > 0;
        }

        static public bool isCollesion(PointCollection dst, PointCollection src)
        {
            Collection<bool> in_chk = new Collection<bool>();

            foreach (var each in src)
            {
                in_chk.Add(is_inside(dst, each));
            }

            if (in_chk[0] ^ in_chk[1] || in_chk[2] ^ in_chk[3] || in_chk[1] ^ in_chk[2])
            {
                return true;
            }
            return false;
        }

        private void history_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            jsonStay = history.SelectedIndex+1;
            Undo_Click(null, e);
        }

        
    }
}
