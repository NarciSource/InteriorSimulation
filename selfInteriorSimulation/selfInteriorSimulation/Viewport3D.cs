using HelixToolkit.Wpf;
using System;
using System.Linq;
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
        public Action ProgressStatueValueUp { get; set; }
        public Action ProgressStatueMaximumUp { get; set; }
        public Action CameraStatue { get; set; }

        public Viewport3D()
        {
            Initialize();

            this.Camera.Position = new Point3D(219, 118, 182);
            this.Camera.UpDirection = new Vector3D(-0.75, 0.01, 0.64);
            this.Camera.LookDirection = new Vector3D(-178.64, 2.99, -209.157);
            this.Camera.Changed += (o, e) => { CameraStatue(); };

            this.Background = new SolidColorBrush(Colors.SteelBlue);
        }
        public void Initialize()
        {
            this.Children.Add(new SunLight());

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
            this.Children
                .Where(each => { return each.GetType() == typeof(ModelVisual3D); })
                .ToList().ForEach(each => this.Children.Remove(each));            
            
            Parsing(canvas.Children);
        }



        private void Parsing(UIElementCollection uIElements)
        {
            foreach (var room in uIElements.OfType<Room>())
            {
                bool chkfirst = true;
                ModelVisual3D room3D = new ModelVisual3D();

                foreach (var each in room.Children)
                {
                    if (each is System.Windows.Shapes.Polygon && chkfirst)
                    {
                        var vertices = (each as System.Windows.Shapes.Polygon).Points;
                        var background = room.Background;

                        ModelVisual3D frame3D = new ModelVisual3D();
                        {
                            ModelVisual3D floor3D = MakeFloor(vertices, background);
                            frame3D.Children.Add(floor3D);

                            for (int i = 0; i < vertices.Count; i++)
                            {
                                ModelVisual3D wall3D = MakeWall(vertices[i], vertices[(i + 1) % vertices.Count]);
                                frame3D.Children.Add(wall3D);
                            }
                        }
                        room3D.Children.Add(frame3D);
                        chkfirst = false;
                    }
                }

                foreach(var furniture in room.Children.OfType<Furniture>())
                {
                    ModelVisual3D furniture3D = MakeObject(furniture);
                    room3D.Children.Add(furniture3D);
                }

                foreach (var gate in room.Children.OfType<Room.Gate>())
                {
                    ModelVisual3D gate3D = MakeGate(gate);
                    room3D.Children.Add(gate3D);
                }

                this.Children.Add(room3D);
            }

            foreach (var furniture in uIElements.OfType<Furniture>())
            {
                ModelVisual3D furniture3D = MakeObject(furniture);
                this.Children.Add(furniture3D);
            }
        }


        private ModelVisual3D MakeGate(Room.Gate source)
        {
            String type = source.Type;
            String datasrc = source.ModelSource;
            Vector3D position;
            Vector3D scale;
            Double angle = Math.Atan2(source.Line.X2 - source.Line.X1, source.Line.Y2 - source.Line.Y1) * (180 / Math.PI);

            switch (type)
            {
                case "Door":
                    position = new Vector3D((source.Line.Y1 + source.Line.Y2) / 2, (source.Line.X1 + source.Line.X2) / 2, 0);
                    scale = new Vector3D(1, 1, 1);
                    return Convert3D(position, angle, scale, datasrc);

                case "Window":
                    position = new Vector3D((source.Line.Y1 + source.Line.Y2) / 2, (source.Line.X1 + source.Line.X2) / 2, 70);
                    scale = new Vector3D(0.7, 0.7, 0.7);
                    return Convert3D(position, angle, scale, datasrc);

                default:
                    throw new Exception();
            }
        }
        
        private ModelVisual3D MakeObject(Furniture source)
        {
            String type = source.Type;
            String datasrc = source.ModelSource;
            Vector3D position = new Vector3D(source.Center.Y, source.Center.X, 0);
            Vector3D scale = new Vector3D(1, 1, 1);
            Double angle = -source.Rotate;

            return Convert3D(position, angle, scale, datasrc);
                
            

        }

        private ModelVisual3D MakeWall(Point poststone1, Point poststone2)
        {
            MeshGeometry3D triangleMesh = new MeshGeometry3D();
            Material material = new DiffuseMaterial(new ImageBrush()
            {
                ImageSource = new BitmapImage(new Uri(@"image/wallpaper3.jpg", UriKind.Relative))
            });

            Action<Point3D,Point3D,Point3D> AddTriangleMesh = (p1,p2,p3) =>
            {
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
            };

            AddTriangleMesh(new Point3D(poststone1.Y, poststone1.X, 0),
                            new Point3D(poststone1.Y, poststone1.X, 100),
                            new Point3D(poststone2.Y, poststone2.X, 0));

            AddTriangleMesh(new Point3D(poststone2.Y, poststone2.X, 0),
                            new Point3D(poststone1.Y, poststone1.X, 100),
                            new Point3D(poststone2.Y, poststone2.X, 100));


            ModelVisual3D model = new ModelVisual3D()
            {
                Transform = adjustTransform3D,
                Content = new GeometryModel3D(triangleMesh, material)
            };
            return model;

        }

        private ModelVisual3D MakeFloor(PointCollection vertices, Brush background)
        {
            MeshGeometry3D triangleMesh = new MeshGeometry3D();
            Material material = new DiffuseMaterial(background);

            void AddTriangleMesh(Point3D p1, Point3D p2, Point3D p3)
            {
                triangleMesh.Positions.Add(p1);
                triangleMesh.Normals.Add(new Vector3D(0, 0, 1));
                triangleMesh.TextureCoordinates.Add(new Point(0, 0));
                triangleMesh.Positions.Add(p2);
                triangleMesh.Normals.Add(new Vector3D(0, 0, 1));
                triangleMesh.TextureCoordinates.Add(new Point(0, 1));
                triangleMesh.Positions.Add(p3);
                triangleMesh.Normals.Add(new Vector3D(0, 0, 1));
                triangleMesh.TextureCoordinates.Add(new Point(1, 1));
            }

            for (int i = 2; i < vertices.Count; i++)
            {
                AddTriangleMesh(new Point3D(vertices[0].Y, vertices[0].X, 0),
                                new Point3D(vertices[i - 1].Y, vertices[i - 1].X, 0),
                                new Point3D(vertices[i].Y, vertices[i].X, 0));
            }


            ModelVisual3D model = new ModelVisual3D()
            {
                Content = new GeometryModel3D(triangleMesh, material),
                Transform = adjustTransform3D
            };
            return model;
        }

        
        private ModelVisual3D Convert3D(Vector3D position, double angle, Vector3D scale, String image_src)
        {
            Model3DGroup model = MetaData.GetInstance.ModelData[image_src];

            Transform3DGroup transform3DGroup = new Transform3DGroup();
            TranslateTransform3D translateTransform3D = new TranslateTransform3D(position);
            AxisAngleRotation3D axisAngleRotation3D = new AxisAngleRotation3D(new Vector3D(0, 0, 1), angle);
            RotateTransform3D rotateTransform3D = new RotateTransform3D(axisAngleRotation3D);
            ScaleTransform3D scaleTransform3D = new ScaleTransform3D(scale);

            transform3DGroup.Children.Add(scaleTransform3D);
            transform3DGroup.Children.Add(rotateTransform3D);
            transform3DGroup.Children.Add(translateTransform3D);
            transform3DGroup.Children.Add(new ScaleTransform3D(new Vector3D(0.1, 0.1, 0.1)));
                
            return new ModelVisual3D()
            {
                Content = model,
                Transform = transform3DGroup
            };
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
