using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace selfInteriorSimulation
{
    class Sofa : InteriorObject
    {
        public Sofa() : base()
        {
            Image.Source = new BitmapImage(new Uri(@"image\sofa_dan.PNG", UriKind.Relative));
            
        }
    }
}
