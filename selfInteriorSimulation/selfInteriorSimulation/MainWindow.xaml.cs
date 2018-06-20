using System;
using System.IO;
using System.Windows;
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

            InitializeCanvas();

            Initialize3DView();

            InitializeClosureExamination();

            InitializeMetadata();
        }


        private void InitializeCanvas()
        {
            Base.active_notify += Active;
            Base.change_notify += Changed;

            Changed("New", "");

            notice_timer.Tick += new EventHandler(Timer_elapsed);
            notice_timer.Start();
        }


        Viewport3D viewport3D;

        private void Initialize3DView()
        {
            
            Action cameraStatue = delegate
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

            viewport3D = new Viewport3D()
            {
                Height = screen.ActualHeight,
                Width = screen.ActualWidth,
                CameraStatue = cameraStatue
            };
        }


        ClosureExamination closureExamination;

        private void InitializeClosureExamination()
        {
            closureExamination = new ClosureExamination()
            {
                Success_layout = () =>
                {
                    chk_button.Source = new BitmapImage(new Uri(@"image\success.png", UriKind.Relative));
                    closure_button.Background = new SolidColorBrush(Colors.MediumSpringGreen);
                },
                Fail_layout = () =>
                {
                    chk_button.Source = new BitmapImage(new Uri(@"image\fail.png", UriKind.Relative));
                    closure_button.Background = new SolidColorBrush(Colors.LightPink);
                },
                Init_layout = () =>
                {
                    chk_button.Source = new BitmapImage(new Uri(@"image\scope.png", UriKind.Relative));
                    closure_button.Background = new SolidColorBrush(Colors.Snow);
                }
            };
        }

        
        private void InitializeMetadata()
        {
            const string fileName = "meta.json";
            string fileContent = "";

            StreamReader sr = new StreamReader(fileName);
            fileContent = sr.ReadToEnd();



            Action progressControlValueUp = delegate
            {
                progressbar.Value++;
                if (progressbar.Value == progressbar.Maximum)
                {
                    progressbar.Visibility = Visibility.Hidden;
                    Ed_button.IsEnabled = true;
                    txt_closure_checking.Visibility = Visibility.Visible;
                }
            };

            MetaData.GetInstance.ProgressStatueMaximumUp = () => { progressbar.Maximum++; };
            MetaData.GetInstance.ProgressStatueValueUp = progressControlValueUp;
            MetaData.GetInstance.AddItem = AddItemToObecjtTab;
            MetaData.GetInstance.Canvas = canvas;

            MetaData.GetInstance.ReadMetaData(fileContent);
            
            sr.Close();
        }


        
    }
}
