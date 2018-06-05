using System;
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


        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            switch ((sender as MenuItem).Header.ToString())
            {
                case "New":
                    canvas.Children.Clear();
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
    }
}
