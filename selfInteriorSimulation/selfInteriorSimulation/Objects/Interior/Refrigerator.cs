using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace selfInteriorSimulation
{
    class Refrigerator : InteriorObject
    {
      
        public Refrigerator() : base()
        {
            Image.Source = new BitmapImage(new Uri(@"image\refrigerator_dan.PNG", UriKind.Relative));
        }
    }
}
