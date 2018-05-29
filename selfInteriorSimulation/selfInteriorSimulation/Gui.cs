using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace selfInteriorSimulation
{
    public partial class MainWindow : Window
    {
        private void New_Click(object sender, RoutedEventArgs e)
        {
            canvas.Children.Clear();

            const double first_width = 700;
            const double first_height = 500;

            const double canvas_width = 1000;
            const double canvas_height = 700;

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
            new Room(points);
            new Door() { Name = "Door", Width = 100, Height = 100, Point=point1 };

            Changed("New"," ");
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            canvas.Children.Clear();
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

        private void About_Click(object sender, RoutedEventArgs e)
        {

            System.Windows.MessageBox.Show("Midas Challenge Application 1 Team\r\r  금준호\r  유한결\r  정원철", "About.");
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }





        private void setting_name_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (activeObject.Name != ((TextBox)sender).Text)
            {
                activeObject.Name = ((TextBox)sender).Text;
            }
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
            double num = 0;
            if (double.TryParse(((TextBox)sender).Text, out num))
            {
                ((Room)activeObject).BorderThickness = new Thickness(num);
            }
        }

        private void setting_angle_TextChanged(object sender, TextChangedEventArgs e)
        {
            double num = 0;
            if (double.TryParse(((TextBox)sender).Text, out num))
            {
                ((InteriorObject)activeObject).rotate = num;
            }
        }

        private void setting_matrial_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (activeObject is Room)
            {
                switch (((ComboBox)sender).SelectedValue.ToString())
                {
                    case "Marble":
                        ((Room)activeObject).setImg(new Uri(@"pack://application:,,,/image/marble.jpg"));
                        break;
                    case "Wood":
                        ((Room)activeObject).setImg(new Uri(@"pack://application:,,,/image/wood.jpg"));
                        break;
                    case "Oak":
                        ((Room)activeObject).setImg(new Uri(@"pack://application:,,,/image/oak.jpg"));
                        break;
                }
            }
        }

        private void setting_Button_Click(object sender, RoutedEventArgs e)
        {
            Changed("Property changed", activeObject.Name);
        }
    }
}
