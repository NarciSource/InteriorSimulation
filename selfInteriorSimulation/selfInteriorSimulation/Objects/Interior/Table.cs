using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace selfInteriorSimulation
{
    class Table : InteriorObject
    {
        public Table() : base()
        {
            Image.Source = new BitmapImage(new Uri(@"image\desk_dan.PNG", UriKind.Relative));
            
        }
    }
}
