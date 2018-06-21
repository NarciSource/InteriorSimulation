using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace selfInteriorSimulation
{
    public partial class MainWindow
    {
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            switch ((sender as MenuItem).Header.ToString())
            {
                case "New":
                    canvas.Children.Clear();
                    History_Clear();
                    break;
                case "Save":
                    Save();
                    break;
                case "Open":
                    Open();
                    break;
                case "Save as Image":
                    Save_as_Image();
                    break;
                case "Clear":
                    canvas.Children.Clear();
                    History_Clear();

                    InitializeCanvas();
                    Initialize3DView();
                    InitializeMetadata();
                    break;
                case "Exit":
                    Application.Current.Shutdown();
                    break;
                case "About":
                    System.Windows.MessageBox.Show("Midas Challenge Application 1 Team\r\r  금준호\r  유한결\r  정원철", "About.");
                    break;
            }
        }

        private void Save_as_Image()
        {
            var saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            saveFileDialog.Filter = "Png Image|*.png";
            saveFileDialog.Title = "Save an Image File";
            saveFileDialog.ShowDialog();

            if (saveFileDialog.FileName == "")
                saveFileDialog.FileName = "image.png";

            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap(
                (int)canvas.RenderSize.Width,
                (int)canvas.RenderSize.Height,
                96d, 96d, System.Windows.Media.PixelFormats.Default);

            renderTargetBitmap.Render(canvas);

            BitmapEncoder pngEncoder = new PngBitmapEncoder();
            pngEncoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));


            using (var fs = System.IO.File.OpenWrite(saveFileDialog.FileName))
            {
                pngEncoder.Save(fs);
            }
        }




        private void Setting_Changed(Object sender, TextChangedEventArgs e)
        {
            int inum = 0;
            double dnum = 0;

            var textbox = sender as TextBox;
            switch (textbox.Name.ToString())
            {
                case "setting_name":
                    if (activeObject.Name != textbox.Text)
                    {
                        activeObject.Name = textbox.Text;
                    }
                    break;

                case "setting_thickness":
                    if (double.TryParse(textbox.Text, out dnum))
                    {
                        ((Room)activeObject).BorderThickness = new Thickness(dnum);
                    }
                    break;

                case "setting_angle":
                    if (double.TryParse(textbox.Text, out dnum))
                    {
                        ((Furniture)activeObject).Rotate = dnum;
                    }
                    break;

                case "setting_width":
                    if (int.TryParse(textbox.Text, out inum))
                    {
                        ((Furniture)activeObject).Width = inum;
                    }
                    break;

                case "setting_height":
                    if (int.TryParse(textbox.Text, out inum))
                    {
                        ((Furniture)activeObject).Height = inum;
                    }
                    break;
            }
        }

        private void Setting_matrial_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (activeObject is Room)
            {
                switch (((ComboBox)sender).SelectedValue.ToString())
                {
                    case "Marble":
                        ((Room)activeObject).Background = new ImageBrush(new BitmapImage(new Uri(@"image/marble.jpg", UriKind.Relative)));
                        break;
                    case "Wood":
                        ((Room)activeObject).Background = new ImageBrush(new BitmapImage(new Uri(@"image/wood.jpg", UriKind.Relative)));
                        break;
                    case "Oak":
                        ((Room)activeObject).Background = new ImageBrush(new BitmapImage(new Uri(@"image/oak.jpg", UriKind.Relative)));
                        break;
                }
            }
        }

        private void Setting_Button_Click(object sender, RoutedEventArgs e)
        {
            Changed("Property changed", activeObject.Name);
        }



        private void Coordinate_click(object sender, RoutedEventArgs e)
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



        bool is3D = false;
        private void Ed_Click(object sender, RoutedEventArgs e)
        {
            if (!is3D)
            {
                statusbar_2d.Visibility = Visibility.Hidden;
                statusbar_2d.MaxWidth = 0;
                statusbar_3d.Visibility = Visibility.Visible;
                statusbar_3d.MaxWidth = int.MaxValue;

                Ed_button.Background = new SolidColorBrush(Colors.LightSkyBlue);
                closure_button.IsEnabled = false;
                SettingDock.Visibility = Visibility.Hidden;
                SettingDock.MaxWidth = 0;
                ObjectControlpad.Visibility = Visibility.Hidden;


                viewport3D.Width = screen.ActualWidth;
                viewport3D.Height = screen.ActualHeight;

                viewport3D.Build(canvas);
                screen.Child = viewport3D;

                is3D = true;
            }
            else
            {
                statusbar_2d.Visibility = Visibility.Visible;
                statusbar_2d.MaxWidth = int.MaxValue;
                statusbar_3d.Visibility = Visibility.Hidden;
                statusbar_3d.MaxWidth = 0;

                Ed_button.Background = new SolidColorBrush(Colors.Snow);
                closure_button.IsEnabled = true;
                SettingDock.Visibility = Visibility.Visible;
                SettingDock.MaxWidth = int.MaxValue;
                ObjectControlpad.Visibility = Visibility.Visible;

                screen.Child = canvas;
                is3D = false;
            }
        }




        bool isActiveClosure = false;
        private void Closure_Chk_Click(object sender, RoutedEventArgs e)
        {
            closureExamination.AllRooms = MetaData.GetInstance.AllRooms;

            if (isActiveClosure == false)
            {
                isActiveClosure = true;

                closureExamination.Grouping();

                closureExamination.Display();
            }
            else
            {
                isActiveClosure = false;

                closureExamination.Undisplay();

                closureExamination.Ungrouping();
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
