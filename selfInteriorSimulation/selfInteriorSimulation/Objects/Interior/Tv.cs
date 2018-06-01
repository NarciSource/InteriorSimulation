using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace selfInteriorSimulation
{
    class Tv : InteriorObject
    {
        public Tv() : base()
        {
            Image.Source = new BitmapImage(new Uri(@"image\tv_dan.PNG", UriKind.Relative));
            
        }
    }
}
