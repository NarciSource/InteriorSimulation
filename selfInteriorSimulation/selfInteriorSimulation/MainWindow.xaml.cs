using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using static selfInteriorSimulation.BaseObject;

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

            BaseObject.active_notify += Active;
            BaseObject.change_notify += Changed;
            BaseObject.canvas = canvas;

            Changed("New", "");
            
            notice_timer.Tick += new EventHandler(Timer_elapsed);
            notice_timer.Start();
        }

        Shape shape = null;
        Point dragonpoint;
        BaseObject paintingObject = new NullBasicObject();

        

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

            if (paintingObject is InteriorObject)
            {
                ((InteriorObject)paintingObject).Center = point;
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
            if (paintingObject is InteriorObject)
            {
                foreach (var room in BaseObject.rooms)
                    if (Algorithm.Is_collesion(room, (InteriorObject)paintingObject) == true)
                    {
                        canvas.Children.Remove(paintingObject);
                        paintingObject = null;
                        return;
                    }

                ((InteriorObject)paintingObject).Build();

                change_notify("Made", paintingObject.Name);
                paintingObject = null;
            }

            else if (paintingObject is RoomCheckObject)
            {
                Point point = e.GetPosition(canvas);
                PointCollection points = new PointCollection();
                points.Add(new Point(dragonpoint.X, dragonpoint.Y));
                points.Add(new Point(dragonpoint.X, point.Y));
                points.Add(new Point(point.X, point.Y));
                points.Add(new Point(point.X, dragonpoint.Y));

                Room room = new Room(points);
                canvas.Children.Add(room);
                canvas.Children.Remove(shape);

                Changed("Made", "Room");
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
                        if (activeObject is Room) break;

                        Type type = activeObject.GetType();
                        paintingObject = Activator.CreateInstance(type) as BaseObject;

                        paintingObject.Name = ((InteriorObject)activeObject).Name;
                        paintingObject.Width = ((InteriorObject)activeObject).Width;
                        paintingObject.Height = ((InteriorObject)activeObject).Height;
                        ((InteriorObject)paintingObject).Build();

                        canvas.Children.Add(paintingObject);

                        Changed("Copy", activeObject.Name);
                        break;
                }
            }
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
                Alret alret = new Alret();
                if (alret.ShowDialog() == true)
                {
                    paintingObject = new CustomObject()
                    {
                        Name = alret.cName,
                        Width = alret.cWidth,
                        Height = alret.cHeight,
                        Image = new Image() { Source = new BitmapImage(uri) }
                    };
                    canvas.Children.Add(paintingObject);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("파일을 열 수 없습니다. " + ex.Message);
            }
        }

        private void Object_Click(object sender, RoutedEventArgs e)
        {
            switch (((Button)sender).Name.ToString())
            {
                case "refre_button":
                    paintingObject = new Refrigerator() { Name="냉장고", Width = 96, Height = 64 };
                    break;
                case "sofa_button":
                    paintingObject = new Sofa() { Name = "소파", Width = 160, Height = 64 };
                    break;
                case "chair_button":
                    paintingObject = new Chair() { Name = "의자", Width = 48, Height = 48 };
                    break;
                case "table_button":
                    paintingObject = new Table() { Name = "책상", Width = 128, Height = 64 };
                    break;
                case "tv_button":
                    paintingObject = new Tv() { Name = "TV", Width = 192, Height = 32 };
                    break;


                case "room_button":
                    paintingObject = new RoomCheckObject();
                    return;


                case "door_button":
                    paintingObject = ((Room)activeObject).AddDoor();
                    break;
                case "window_button":
                    paintingObject = ((Room)activeObject).AddWindow();
                    break;
            }


            canvas.Children.Add(paintingObject);
        }






        private BaseObject activeObject = null;
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

            activeObject = (BaseObject)sender;

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

        

        

        private void Refresh_Status(Point position)
        {
            point_position.Content = position.ToString();
            if (activeObject != null)
                object_type.Content = activeObject.ToString();
            undo_times.Content = jsonStay.ToString();
        }

        
    }
}
