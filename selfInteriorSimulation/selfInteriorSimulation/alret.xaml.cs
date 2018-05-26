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
using System.Windows.Shapes;

namespace selfInteriorSimulation
{
    /// <summary>
    /// alret.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class alret : Window
    {
        public alret()
        {
            InitializeComponent();

            Topmost = true;
            WindowStyle = WindowStyle.None;
            //AllowsTransparency = true;
            //Background = null;
            InitializeComponent();
            if (Application.Current.Properties["aaa"] != null)   // 값이 없으면 에러가 난다.
            {
                lbl1.Content = Application.Current.Properties["aaa"].ToString();
            }
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
