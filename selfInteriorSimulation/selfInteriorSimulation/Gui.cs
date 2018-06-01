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
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            canvas.Children.Clear();
            History_Clear();
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
                ((InteriorObject)activeObject).Rotate = num;
            }
        }

        private void setting_matrial_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (activeObject is Room)
            {
                switch (((ComboBox)sender).SelectedValue.ToString())
                {
                    case "Marble":
                        ((Room)activeObject).Background = new ImageBrush(new BitmapImage(new Uri(@"pack://application:,,,/image/marble.jpg")));
                        break;
                    case "Wood":
                        ((Room)activeObject).Background = new ImageBrush(new BitmapImage(new Uri(@"pack://application:,,,/image/wood.jpg")));
                        break;
                    case "Oak":
                        ((Room)activeObject).Background = new ImageBrush(new BitmapImage(new Uri(@"pack://application:,,,/image/oak.jpg")));
                        break;
                }
            }
        }

        private void setting_Button_Click(object sender, RoutedEventArgs e)
        {
            Changed("Property changed", activeObject.Name);
        }



        private void coordinate_click(object sender, RoutedEventArgs e)
        {
            Algorithm.std_coordinate_size = Convert.ToInt32(((MenuItem)sender).Header);
        }





        System.Windows.Forms.Timer notice_timer = new System.Windows.Forms.Timer() { Interval = 10 * 2000 };

        private void Timer_elapsed(object sender, EventArgs e)
        {
            help_notice.Visibility = Visibility.Hidden;
            notice_timer.Stop();
        }

        private void Notice_Click(object sender, RoutedEventArgs e)
        {
            help_notice.Visibility = Visibility.Visible;
            notice_timer.Start();
        }
    }
}
