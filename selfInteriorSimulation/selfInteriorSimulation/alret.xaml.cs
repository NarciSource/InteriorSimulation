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
    public partial class Alret : Window
    {
        public Alret()
        {
            InitializeComponent();
        }

        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
        

        public int cWidth
        {
            get {
                int num = 0;
                if (int.TryParse(custom_width.Text, out num))
                    return num;
                else return 0;
            }
        }
        public int cHeight
        {
            get
            {
                int num = 0;
                if (int.TryParse(custom_height.Text, out num))
                    return num;
                else return 0;
            }
        }
        public string cName
        {
            get
            {
                return custom_name.Text;
            }
        }
    }
}
