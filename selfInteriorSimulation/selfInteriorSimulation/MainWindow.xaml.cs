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
            BasicObject.canvas = canvas;
            new BasicObject() { Name = "Test"};
            PointCollection points = new PointCollection();
            points.Add(new Point(150, 150));
            points.Add(new Point(150,50));
            points.Add(new Point(70, 50));
            points.Add(new Point(30,150));
            new Wall(points);
            new Refrigeraot(new Point(100,200));
        }
    }
}
