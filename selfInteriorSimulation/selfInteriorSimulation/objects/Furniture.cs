using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace selfInteriorSimulation
{


    class Furniture : Base
    {
        public Point Center
        {
            get
            {
                return new Point(Canvas.GetLeft(this) + Width / 2, Canvas.GetTop(this) + Height / 2);
            }
            set
            {
                Canvas.SetLeft(this, value.X - Width / 2);
                Canvas.SetTop(this, value.Y - Height / 2);
            }
        }
        public Image Image
        {
            get { return border.Child as Image; }
            set { border.Child = value; }
        }
        public ImageSource ImageSource
        {
            set { (border.Child as Image).Source = value; }
        }
        public double Rotate
        {
            get
            {
                var rotate = this.Image.RenderTransform as RotateTransform;
                return rotate.Angle;
            }
            set
            {
                const double magic_num = 3;
                this.Image.RenderTransform = new RotateTransform(value, Width / 2 - magic_num, Height / 2 - magic_num);
            }
        }
        public String ModelSource { get; set; }
        

        public Furniture()
        {
            this.Image = new Image() { Stretch = System.Windows.Media.Stretch.Fill };

            this.MouseDown += Mouse_Down;
            this.MouseMove += Mouse_Move;
            this.MouseUp += Mouse_Up;

            Canvas.SetZIndex(this, 3);
        }

        public void Mouse_Move(object sender, MouseEventArgs e)
        {
            if (this.IsMouseCaptured)
            {
                Point clickPoint = e.GetPosition(MetaData.GetInstance.Canvas);
                this.Center = Algorithm.Adjust_to_fit_std(clickPoint);
            }
        }

        public void Mouse_Down(object sender, MouseButtonEventArgs e)
        {
            active_notify(this); this.CaptureMouse();
        }
        
        public void Mouse_Up(object sender, MouseButtonEventArgs e)
        {
            if (this.IsMouseCaptured)
            {
                foreach (var each in MetaData.GetInstance.AllRooms)
                {
                    if (Algorithm.Is_collesion(each, this) == true) return;
                }
                change_notify("Moved", this.Name);
                this.ReleaseMouseCapture();
            }
        }


        public override object Clone()
        {
            var copy = Activator.CreateInstance(this.GetType()) as Furniture;

            copy.Type = this.Type;
            copy.Name = this.Name;
            copy.Width = this.Width;
            copy.Height = this.Height;
            copy.Center = this.Center;
            copy.Image = new Image() { Source = this.Image.Source.Clone(), Stretch = this.Image.Stretch };
            copy.ModelSource = this.ModelSource;
            copy.Rotate = 0;

            return copy;
        }




        static public Furniture Temporary(Furniture interiorObject)
        {
            interiorObject.MouseDown -= interiorObject.Mouse_Down;
            interiorObject.MouseMove -= interiorObject.Mouse_Move;
            interiorObject.MouseUp -= interiorObject.Mouse_Up;
            return interiorObject;
        }
    }
}
