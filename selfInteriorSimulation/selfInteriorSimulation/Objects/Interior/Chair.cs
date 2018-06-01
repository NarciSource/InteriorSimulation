using System;
using System.Windows;
using System.Windows.Media.Imaging;
using static selfInteriorSimulation.BaseObject;

namespace selfInteriorSimulation
{
    class Chair : InteriorObject
    {
        public Chair() : base()
        {
            Image.Source = new BitmapImage(new Uri(@"image\chair2_dan.PNG", UriKind.Relative));
        }
    }
}
