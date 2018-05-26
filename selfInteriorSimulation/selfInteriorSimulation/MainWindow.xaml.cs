using Microsoft.Win32;
using System;
using System.Collections.Generic;
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



        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            canvas.Children.Clear();
        }

        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            if (canvas.Children.Count > 0)
            {
                canvas.Children.RemoveAt(canvas.Children.Count - 1);
            }
            undo_times.Content = canvas.Children.Count.ToString();
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
           
        }






        enum Painting_Mode
        {
            Default,
            Wall,
            Bottom,
            Object
        };
        Painting_Mode painting_mode = Painting_Mode.Default;




        Shape shape = null;
        PointCollection points = new PointCollection();

        private void Mouse_Left_Down(object sender, MouseButtonEventArgs e)
        {
            points = new PointCollection();
            points.Add(e.GetPosition(canvas));

            switch (painting_mode)
            {
                case Painting_Mode.Wall:
                    shape = new Rectangle()
                    {
                        StrokeThickness = 1,
                        Stroke = new SolidColorBrush(Colors.Black),
                    };
                    break;
                case Painting_Mode.Bottom:

                    break;

                case Painting_Mode.Object:
                    
                    break;

                case Painting_Mode.Default:

                    break;
            }
            


            try
            {
                canvas.Children.Add(shape);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString());

            }
        }




        private void Mouse_Move(object sender, MouseEventArgs e)
        {
            Point point = e.GetPosition(canvas);

            Refresh_Status(point, canvas.Children.Count);

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                switch (painting_mode)
                {
                    case Painting_Mode.Wall:
                        shape.Width = Math.Abs(points[0].X - point.X);
                        shape.Height = Math.Abs(points[0].Y - point.Y);
                        shape.Margin = new Thickness(Math.Min(points[0].X, point.X),
                                                    Math.Min(points[0].Y, point.Y), 0, 0);

                        canvas.Children.Remove(shape);
                        break;
                    case Painting_Mode.Bottom:

                        break;

                    case Painting_Mode.Object:

                       
                        break;

                    case Painting_Mode.Default:

                        break;
                }
            }

        }


        private void Mouse_Left_Up(object sender, MouseButtonEventArgs e)
        {
            switch (painting_mode)
            {
                case Painting_Mode.Wall:
                    points.Add(new Point(points[0].X + shape.Width, points[0].Y));
                    points.Add(new Point(points[0].X + shape.Width, points[0].Y + shape.Height));
                    points.Add(new Point(points[0].X, points[0].Y + shape.Height));

                    new Wall(points);
                    break;
            }


            painting_mode = Painting_Mode.Default;
        }

        


        
        Color front_color = Colors.Black;
        Color back_color = Colors.White;
        

        
        

        private void Refresh_Status(Point position, int undos)
        {
            point_position.Content = position.ToString();
            undo_times.Content = undos.ToString();
        }

        private void Wall_Click(object sender, RoutedEventArgs e)
        {
            painting_mode = Painting_Mode.Wall;
        }
    }
}
