using HelixToolkit.Wpf;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace selfInteriorSimulation
{
    class Viewport3D : HelixViewport3D
    {
        ScaleTransform3D adjustTransform3D = new ScaleTransform3D(new Vector3D(0.1, 0.1, 0.1));
        

        public Viewport3D()
        {
            this.Children.Add(new SunLight());

            this.Camera.Position = new Point3D(219, 118, 182);
            this.Camera.UpDirection = new Vector3D(-0.75, 0.01, 0.64);
            this.Camera.LookDirection = new Vector3D(-178.64, 2.99, -209.157);

            this.Background = new SolidColorBrush(Colors.SteelBlue);


            var gridline = new GridLinesVisual3D()
            {
                Center = new Point3D(0, 0, -0.1), //How do I use z-buffer?
                Normal = new Vector3D(0, 0, 1),
                Width = 1000,
                Length = 1000,
                MinorDistance = 3,
                MajorDistance = 4,
                Thickness = 0.05,
                Fill = Brushes.Snow,
            };

            this.Children.Add(gridline);
        }
        public void Build(Canvas canvas)
        {
            Parsing(canvas.Children);
        }



        private void Parsing(UIElementCollection uIElements)
        {
            foreach (var element in uIElements)
            {
                bool chkfirst = true;
                if (element is BaseObject == false) continue;

                else if (element is Room)
                {
                    foreach (var each in (element as Room).Children)
                    {
                        if (each is System.Windows.Shapes.Polygon && chkfirst)
                        {
                            var vertices = (each as System.Windows.Shapes.Polygon).Points;
                            var floor = (element as Room).Background;

                            MakeFloor(vertices, floor);

                            for (int i = 0; i < vertices.Count; i++)
                            {
                                MakeWall(vertices[i], vertices[(i + 1) % vertices.Count]);
                            }
                            chkfirst = false;
                        }

                        else if (each is InteriorObject)
                        {
                            MakeObject(each as InteriorObject);
                        }

                        else if (each is Room.Gate)
                        {
                            MakeGate(each as Room.Gate);
                        }
                    }
                }

                else if (element is InteriorObject)
                {
                    MakeObject(element as InteriorObject);
                }

            }

        }


        private void MakeGate(Room.Gate source)
        {
            String type = source.GetType().Name;
            Vector3D position;
            Vector3D scale;
            double angle = Math.Atan2(source.Line.X2 - source.Line.X1, source.Line.Y2 - source.Line.Y1) * (180 / Math.PI);

            switch (type)
            {
                case "Door":
                    position = new Vector3D((source.Line.Y1 + source.Line.Y2) / 2, (source.Line.X1 + source.Line.X2) / 2, 0);
                    scale = new Vector3D(1, 1, 1);
                    Convert3D(position, angle, scale, @"models\gate\Door.obj");
                    break;

                case "WindowObject":
                    position = new Vector3D((source.Line.Y1 + source.Line.Y2) / 2, (source.Line.X1 + source.Line.X2) / 2, 70);
                    scale = new Vector3D(0.7, 0.7, 0.7);
                    Convert3D(position, angle, scale, @"models\gate\window.obj");
                    break;
            }
        }

        private void MakeObject(InteriorObject source)
        {
            String type = source.GetType().Name;
            Vector3D position = new Vector3D(source.Center.Y, source.Center.X, 0);
            Vector3D scale = new Vector3D(1, 1, 1);

            switch (type)
            {
                case "Sofa":
                    Convert3D(position, 0, scale, @"models\sofa\ready.obj");
                    break;

                case "Chair":
                    Convert3D(position, 0, scale, @"models\chair\chair.obj");
                    break;

                case "Refrigerator":
                    Convert3D(position, 0, scale, @"models\refriger\refriger.obj");
                    break;

                case "Table":
                    Convert3D(position, 0, scale, @"models\desk\Desk.obj");
                    break;

                case "Tv":
                    Convert3D(position, 0, scale, @"models\ledtv\led_tv.obj");
                    break;
            }
        }

        private void MakeWall(Point poststone1, Point poststone2)
        {
            MeshGeometry3D triangleMesh = new MeshGeometry3D();
            Material material = new DiffuseMaterial(new ImageBrush()
            {
                ImageSource = new BitmapImage(new Uri(@"image/wallpaper3.jpg", UriKind.Relative))
            });

            Point3D p1 = new Point3D(poststone1.Y, poststone1.X, 0);
            Point3D p2 = new Point3D(poststone1.Y, poststone1.X, 100);
            Point3D p3 = new Point3D(poststone2.Y, poststone2.X, 0);

            Vector3D normalv1 = new Vector3D();
            Vector3D normalv2 = new Vector3D();
            Vector3D normalv3 = new Vector3D();

            CalculateNormal(p1, p2, p3,
                            ref normalv1, ref normalv2, ref normalv3);

            triangleMesh.Positions.Add(p1);
            triangleMesh.Normals.Add(normalv1);
            triangleMesh.TextureCoordinates.Add(new Point(0, 0));
            triangleMesh.Positions.Add(p2);
            triangleMesh.Normals.Add(normalv2);
            triangleMesh.TextureCoordinates.Add(new Point(0, 1));
            triangleMesh.Positions.Add(p3);
            triangleMesh.Normals.Add(normalv3);
            triangleMesh.TextureCoordinates.Add(new Point(1, 0));
            
            ModelVisual3D model = new ModelVisual3D()
            {
                Transform = adjustTransform3D,
                Content = new GeometryModel3D(triangleMesh, material)
            };
            this.Children.Add(model);




            triangleMesh = new MeshGeometry3D();

            p1 = new Point3D(poststone2.Y, poststone2.X, 0);
            p2 = new Point3D(poststone1.Y, poststone1.X, 100);
            p3 = new Point3D(poststone2.Y, poststone2.X, 100);

            normalv1 = new Vector3D();
            normalv2 = new Vector3D();
            normalv3 = new Vector3D();

            CalculateNormal(p1, p2, p3,
                            ref normalv1, ref normalv2, ref normalv3);

            triangleMesh.Positions.Add(p1);
            triangleMesh.Normals.Add(normalv1);
            triangleMesh.TextureCoordinates.Add(new Point(0, 1));
            triangleMesh.Positions.Add(p2);
            triangleMesh.Normals.Add(normalv2);
            triangleMesh.TextureCoordinates.Add(new Point(1, 1));
            triangleMesh.Positions.Add(p3);
            triangleMesh.Normals.Add(normalv3);
            triangleMesh.TextureCoordinates.Add(new Point(1, 0));

            model = new ModelVisual3D()
            {
                Transform = adjustTransform3D,
                Content = new GeometryModel3D(triangleMesh, material)
            };
            this.Children.Add(model);
        }

        private void MakeFloor(PointCollection vertices, Brush background)
        {
            MeshGeometry3D triangleMesh;
            Material material = new DiffuseMaterial(background);

            for (int i = 2; i < vertices.Count; i++)
            {
                triangleMesh = new MeshGeometry3D();
                triangleMesh.Positions.Add(new Point3D(vertices[0].Y, vertices[0].X, 0));
                triangleMesh.Positions.Add(new Point3D(vertices[i - 1].Y, vertices[i - 1].X, 0));
                triangleMesh.Positions.Add(new Point3D(vertices[i].Y, vertices[i].X, 0));

                triangleMesh.Normals.Add(new Vector3D(0, 0, 1));
                triangleMesh.Normals.Add(new Vector3D(0, 0, 1));
                triangleMesh.Normals.Add(new Vector3D(0, 0, 1));

                triangleMesh.TextureCoordinates.Add(new Point(0, 0));
                triangleMesh.TextureCoordinates.Add(new Point(0, 1));
                triangleMesh.TextureCoordinates.Add(new Point(1, 1));
                

                ModelVisual3D model = new ModelVisual3D()
                {
                    Content = new GeometryModel3D(triangleMesh, material),
                    Transform = adjustTransform3D
                };
                this.Children.Add(model);
            }
        }

        
        private void Convert3D(Vector3D position, double angle, Vector3D scale, String image_src)
        {
            ModelVisual3D modelVisual3D = new ModelVisual3D();
            Material material = new DiffuseMaterial(new ImageBrush(new BitmapImage(new Uri(@"image/marble.jpg", UriKind.Relative))));
            Model3DGroup model = new Model3DGroup();

            switch (System.IO.Path.GetExtension(image_src).ToLower())
            {
                case ".fbx":
                case ".obj":
                    model = (new ObjReader()).Read(image_src);
                    (model.Children[0] as GeometryModel3D).Material = material;
                    break;

                case ".3ds":                    
                    model = (new StudioReader()).Read(image_src);
                    break;


                default:
                    throw new InvalidOperationException("File format not supported.");
            }

            modelVisual3D.Content = model;

            Transform3DGroup transform3DGroup = new Transform3DGroup();
            
            TranslateTransform3D translateTransform3D = new TranslateTransform3D(position);
            AxisAngleRotation3D axisAngleRotation3D = new AxisAngleRotation3D(new Vector3D(0, 0, 1), angle);
            RotateTransform3D rotateTransform3D = new RotateTransform3D(axisAngleRotation3D);
            ScaleTransform3D scaleTransform3D = new ScaleTransform3D(scale);


            transform3DGroup.Children.Add(scaleTransform3D);
            transform3DGroup.Children.Add(rotateTransform3D);
            transform3DGroup.Children.Add(translateTransform3D);
            transform3DGroup.Children.Add(adjustTransform3D);

            modelVisual3D.Transform = transform3DGroup;



            this.Children.Add(modelVisual3D);
        }


        private void CalculateNormal(Point3D p1, Point3D p2, Point3D p3, ref Vector3D normalv1, ref Vector3D normalv2, ref Vector3D normalv3)
        {
            Vector3D v1, v2;

            /** Normal Vector of first point
			* This multiplies theta by weight.	*/
            v1 = p2 - p1; v2 = p3 - p1;
            normalv1 += Vector3D.CrossProduct(v1, v2) * Vector3D.AngleBetween(v1, v2);
            normalv1.Normalize();

            /* Second point */
            v1 = p3 - p2; v2 = p1 - p2;
            normalv2 += Vector3D.CrossProduct(v1, v2) * Vector3D.AngleBetween(v1, v2);
            normalv2.Normalize();

            /* Third point */
            v1 = p1 - p3; v2 = p2 - p3;
            normalv3 += Vector3D.CrossProduct(v1, v2) * Vector3D.AngleBetween(v1, v2);
            normalv3.Normalize();
        }

    }
}
