using Microsoft.Win32;
using System;
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

            Wall.notify += Active;
            InteriorObject.notify += Active;


            New_Click(new object(), new RoutedEventArgs());
        }

        private BasicObject activeObject = null;
        private void Active(object sender)
        {
            if (activeObject != null)
            {
                if (!(activeObject is Wall))
                    activeObject.setBorderThickness(0);
                activeObject.setColor(Colors.Black);
            }

            activeObject = (BasicObject)sender;
            SettingDock.Visibility = Visibility.Visible;
            activeObject.setColor(Colors.Red);
            setting_name.Text = activeObject.Name.ToString();

            if (sender is Wall)
            {
                setting_height.IsEnabled = false;
                setting_width.IsEnabled = false;
                setting_angle.IsEnabled = false;
                setting_material.IsEnabled = true;
                setting_thickness.IsEnabled = true;
                setting_thickness.Text = ((Wall)activeObject).getBorderThickness().ToString();
            }
            else
            {
                activeObject.setBorderThickness(1);

                setting_height.IsEnabled = true;
                setting_width.IsEnabled = true;
                setting_angle.IsEnabled = true;
                setting_material.IsEnabled = false;
                setting_thickness.IsEnabled = false;
                setting_width.Text = activeObject.ActualWidth.ToString();
                setting_height.Text = activeObject.ActualHeight.ToString();
            }

        }


        private void Save_Click(object sender, RoutedEventArgs e)
        {
            foreach (var each in canvas.Children)
            {

            }

        }

        private void Save_Image_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Png Image|*.png";
            saveFileDialog.Title = "Save an Image File";
            saveFileDialog.ShowDialog();

            if (saveFileDialog.FileName == "")
                saveFileDialog.FileName = "image.png";

            RenderTargetBitmap rtb = new RenderTargetBitmap(
                (int)canvas.RenderSize.Width,
                (int)canvas.RenderSize.Height,
                96d, 96d, System.Windows.Media.PixelFormats.Default);

            rtb.Render(canvas);

            BitmapEncoder pngEncoder = new PngBitmapEncoder();
            pngEncoder.Frames.Add(BitmapFrame.Create(rtb));


            using (var fs = System.IO.File.OpenWrite(saveFileDialog.FileName))
            {
                pngEncoder.Save(fs);
            }
        }


        private void New_Click(object sender, RoutedEventArgs e)
        {
            canvas.Children.Clear();

            double first_width = 400;
            double first_height = 300;

            double canvas_width = 800;
            double canvas_height = 500;

            double center_x1 = canvas_width / 2 - first_width / 2;
            double center_y1 = canvas_height / 2 - first_height / 2;
            double center_x2 = canvas_width / 2 + first_width / 2;
            double center_y2 = canvas_height / 2 + first_height / 2;

            Point point1 = new Point(center_x1, center_y1);
            Point point2 = new Point(center_x1, center_y2);
            Point point3 = new Point(center_x2, center_y2);
            Point point4 = new Point(center_x2, center_y1);


            BasicObject.canvas = canvas;
            PointCollection points = new PointCollection();
            points.Add(point1);
            points.Add(point2);
            points.Add(point3);
            points.Add(point4);
            new Wall(points);
            new AttachObject(new Point(100, 100)) { Width = 50, Height = 50 };

        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            canvas.Children.Clear();
        }

        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            if (canvas.Children.Count > 0)
            {
                redocollection.Add((BasicObject)canvas.Children[canvas.Children.Count - 1]);
                canvas.Children.RemoveAt(canvas.Children.Count - 1);
            }
            undo_times.Content = canvas.Children.Count.ToString();
        }

        List<BasicObject> redocollection = new List<BasicObject>();
        private void Redo_Click(object sender, RoutedEventArgs e)
        {
            if (redocollection.Count > 0)
            {
                canvas.Children.Add(redocollection[redocollection.Count - 1]);
                redocollection.RemoveAt(redocollection.Count - 1);
            }
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
        }






        enum Painting_Mode
        {
            Default,
            Wall,
            Bottom,
            Refre,
            TV,
            Table,
            Sofa,
            Chair
        };
        Painting_Mode painting_mode = Painting_Mode.Default;




        Shape shape = null;
        PointCollection points = new PointCollection();
        InteriorObject nowObject = null;

        private void Mouse_Left_Down(object sender, MouseButtonEventArgs e)
        {
            const int object_width = 100;
            const int object_height = 130;

            Point point = e.GetPosition(canvas);
            points = new PointCollection();
            points.Add(point);

            if (painting_mode == Painting_Mode.Default) return;


            switch (painting_mode)
            {
                case Painting_Mode.Chair:
                case Painting_Mode.Refre:
                case Painting_Mode.Sofa:
                case Painting_Mode.Table:
                case Painting_Mode.TV:

                    PointCollection object_points = new PointCollection();
                    object_points.Add(new Point(point.X - object_width / 2, point.Y - object_height / 2));
                    object_points.Add(new Point(point.X + object_width / 2, point.Y - object_height / 2));
                    object_points.Add(new Point(point.X - object_width / 2, point.Y + object_height / 2));
                    object_points.Add(new Point(point.X + object_width / 2, point.Y + object_height / 2));

                    foreach (var each in BasicObject.walls)
                        if (isCollesion(each.points, object_points) == true) return;
                    
                    painting_mode = Painting_Mode.Default;
                    break;
            }

            if (painting_mode == Painting_Mode.Wall)
            {
                shape = new Rectangle()
                {
                    StrokeThickness = 1,
                    Stroke = new SolidColorBrush(Colors.Black),
                };

                try
                {
                    canvas.Children.Add(shape);
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.ToString());

                }
            }
        }




        private void Mouse_Move(object sender, MouseEventArgs e)
        {
            Point point = e.GetPosition(canvas);

            Refresh_Status(point, canvas.Children.Count);

            switch (painting_mode)
            {
                case Painting_Mode.Chair:
                case Painting_Mode.Refre:
                case Painting_Mode.Sofa:
                case Painting_Mode.Table:
                case Painting_Mode.TV:
                    nowObject.setPosition(new Point(point.X - nowObject.Width / 2, point.Y - nowObject.Height / 2));
                    return;
            }

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (painting_mode == Painting_Mode.Default) return;

                shape.Width = Math.Abs(points[0].X - point.X);
                shape.Height = Math.Abs(points[0].Y - point.Y);
                shape.Margin = new Thickness(Math.Min(points[0].X, point.X),
                                            Math.Min(points[0].Y, point.Y), 0, 0);

                canvas.Children.Remove(shape);
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

        static public bool isCollesion(PointCollection dst,PointCollection src)
        {
            Collection<bool> in_chk = new Collection<bool>();

            foreach (var each in src)
            {
                in_chk.Add(is_inside(dst, each));
            }

            if (in_chk[0] ^ in_chk[1] || in_chk[2]^in_chk[3] || in_chk[1]^in_chk[2])
            {
                return true;
            }
            return false;
        }




        private void Mouse_Left_Up(object sender, MouseButtonEventArgs e)
        {
            Point point = e.GetPosition(canvas);
            
            switch (painting_mode)
            {
                case Painting_Mode.Wall:
                    points.Add(new Point(points[0].X + shape.Width, points[0].Y));
                    points.Add(new Point(points[0].X + shape.Width, points[0].Y + shape.Height));
                    points.Add(new Point(points[0].X, points[0].Y + shape.Height));

                    new Wall(points);

                    canvas.Children.Remove(shape);
                    painting_mode = Painting_Mode.Default;
                    break;

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
            painting_mode = Painting_Mode.Wall;
        }

        private void Object_Click(object sender, RoutedEventArgs e)
        {
            const int object_width = 100;
            const int object_height = 130;

            switch (((Button)sender).Name.ToString())
            {
                case "refre_button":
                    nowObject = new Refrigerator(new Point(0, 0)) { Width = object_width, Height = object_height };
                    painting_mode = Painting_Mode.Refre;
                    break;
                case "sofa_button":
                    nowObject = new Sofa(new Point(0, 0)) { Width = object_width, Height = object_height };
                    painting_mode = Painting_Mode.Sofa;
                    break;
                case "chair_button":
                    nowObject = new Chair(new Point(0, 0)) { Width = object_width, Height = object_height };
                    painting_mode = Painting_Mode.Chair;
                    break;
                case "table_button":
                    nowObject = new Table(new Point(0, 0)) { Width = object_width, Height = object_height };
                    painting_mode = Painting_Mode.Table;
                    break;
                case "tv_button":
                    nowObject = new Tv(new Point(0, 0)) { Width = object_width, Height = object_height };
                    painting_mode = Painting_Mode.TV;
                    break;
            }
            
        }

        private void setting_name_TextChanged(object sender, TextChangedEventArgs e)
        {
            activeObject.Name = ((TextBox)sender).Text;
        }

        private void setting_width_TextChanged(object sender, TextChangedEventArgs e)
        {
            int num = 0;
            if (int.TryParse(((TextBox)sender).Text, out num))
            {
                ((InteriorObject)activeObject).Width = num;
            }
        }

        private void setting_height_TextChanged(object sender, TextChangedEventArgs e)
        {
            int num = 0;
            if (int.TryParse(((TextBox)sender).Text, out num))
            {
                ((InteriorObject)activeObject).Height = num;
            }
        }

        private void setting_thickness_TextChanged(object sender, TextChangedEventArgs e)
        {
            int num = 0;
            if (int.TryParse(((TextBox)sender).Text, out num))
            {
                ((Wall)activeObject).setBorderThickness(num);
            }
        }
        
        private void setting_angle_TextChanged(object sender, TextChangedEventArgs e)
        {
            double num = 0;
            if (double.TryParse(((TextBox)sender).Text, out num))
            {
                ((InteriorObject)activeObject).setRotate(num);
            }
        }

        private void setting_matrial_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (activeObject is Wall)
            {
                switch (((ComboBox)sender).SelectedValue.ToString())
                {
                    case "Marble":
                        ((Wall)activeObject).setImg(new Uri(@"pack://application:,,,/image/marble.jpg"));
                        break;
                    case "Wood":
                        ((Wall)activeObject).setImg(new Uri(@"pack://application:,,,/image/wood.jpg"));
                        break;
                    case "Oak":
                        ((Wall)activeObject).setImg(new Uri(@"pack://application:,,,/image/oak.jpg"));
                        break;
                }
            }
        }


        private void Window_KeyDown(object sender, KeyEventArgs e)
        {

            const int object_width = 100;
            const int object_height = 130;
            if (activeObject != null)
            {
                switch (e.Key)
                {
                    case Key.Delete:
                        canvas.Children.Remove(activeObject);

                        activeObject = null;
                        break;
                    case Key.LeftCtrl:
                        if (activeObject is Wall) break;

                        switch (activeObject.isType)
                        {
                            case BasicObject.IsType.Chair:
                                nowObject = new Chair(new Point(0, 0));
                                painting_mode = Painting_Mode.Chair;
                                break;
                            case BasicObject.IsType.Sofa:
                                nowObject = new Sofa(new Point(0, 0));
                                painting_mode = Painting_Mode.Sofa;
                                break;
                            case BasicObject.IsType.Table:
                                nowObject = new Table(new Point(0, 0));
                                painting_mode = Painting_Mode.Table;
                                break;
                            case BasicObject.IsType.Tv:
                                nowObject = new Tv(new Point(0, 0));
                                painting_mode = Painting_Mode.TV;
                                break;
                            case BasicObject.IsType.Refrigeraot:
                                nowObject = new Refrigerator(new Point(0, 0));
                                painting_mode = Painting_Mode.Refre;
                                break;

                        }
                        nowObject.Width = ((InteriorObject)activeObject).Width;
                        nowObject.Height = ((InteriorObject)activeObject).Height;

                        break;
                }
            }
        }
    }
}
