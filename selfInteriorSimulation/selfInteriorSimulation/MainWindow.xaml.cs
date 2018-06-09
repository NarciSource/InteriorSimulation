using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace selfInteriorSimulation
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        Viewport3D viewport3D;
        public MainWindow()
        {
            InitializeComponent();

            BaseObject.active_notify += Active;
            BaseObject.change_notify += Changed;
            BaseObject.canvas = canvas;

            Changed("New", "");



            viewport3D = new Viewport3D()
            {
                Height = screen.ActualHeight,
                Width = screen.ActualWidth,
                Progress = progressbar,
                ProgressLabel = numof3dobjects
            };
            viewport3D.CameraChanged += (o, ev) =>
            {
                camera_position.Content = viewport3D.Camera.Position.X.ToString(".##") + ","
                                        + viewport3D.Camera.Position.Y.ToString(".##") + ","
                                        + viewport3D.Camera.Position.X.ToString(".##");
                camera_up.Content = viewport3D.Camera.UpDirection.X.ToString("0.##") + ","
                                        + viewport3D.Camera.UpDirection.Y.ToString("0.##") + ","
                                        + viewport3D.Camera.UpDirection.X.ToString("0.##");
                camera_look.Content = viewport3D.Camera.LookDirection.X.ToString(".##") + ","
                                        + viewport3D.Camera.LookDirection.Y.ToString(".##") + ","
                                        + viewport3D.Camera.LookDirection.X.ToString(".##");
            };



            notice_timer.Tick += new EventHandler(Timer_elapsed);
            notice_timer.Start();
        }


        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            switch ((sender as MenuItem).Header.ToString())
            {
                case "New":
                    canvas.Children.Clear();
                    History_Clear();
                    BaseObject.gRooms.Clear();
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
                    BaseObject.gRooms.Clear();
                    break;
                case "Exit":
                    Application.Current.Shutdown();
                    break;
                case "About":
                    System.Windows.MessageBox.Show("Midas Challenge Application 1 Team\r\r  금준호\r  유한결\r  정원철", "About.");
                    break;
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
                        ((InteriorObject)activeObject).Rotate = dnum;
                    }
                    break;

                case "setting_width":
                    if (int.TryParse(textbox.Text, out inum))
                    {
                        ((InteriorObject)activeObject).Width = inum;
                    }
                    break;

                case "setting_height":
                    if (int.TryParse(textbox.Text, out inum))
                    {
                        ((InteriorObject)activeObject).Height = inum;
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
                        ((Room)activeObject).Background = new ImageBrush(new BitmapImage(new Uri(@"image/marble.jpg",UriKind.Relative)));
                        break;
                    case "Wood":
                        ((Room)activeObject).Background = new ImageBrush(new BitmapImage(new Uri(@"image/wood.jpg",UriKind.Relative)));
                        break;
                    case "Oak":
                        ((Room)activeObject).Background = new ImageBrush(new BitmapImage(new Uri(@"image/oak.jpg",UriKind.Relative)));
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
        private void Ed_Chk_Click(object sender, RoutedEventArgs e)
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
                objectAddControl.Visibility = Visibility.Hidden;


                viewport3D.Width = screen.ActualWidth;
                viewport3D.Height = screen.ActualHeight;

                viewport3D.Build(canvas);
                screen.Child = viewport3D;

                //progressbar.IsIndeterminate = true;

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
                objectAddControl.Visibility = Visibility.Visible;

                screen.Child = canvas;
                is3D = false;
            }
        }
    }
}
