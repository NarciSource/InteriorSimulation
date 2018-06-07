using HelixToolkit.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace selfInteriorSimulation
{
    class Ed : HelixViewport3D
    {
        public Ed()
        {
            this.Children.Add(new DefaultLights());

            this.Camera.Position = new Point3D(26, 12, 10);
            this.Camera.UpDirection = new Vector3D(-0.46, 0.004, 0.88);
            this.Camera.LookDirection = new Vector3D(-8.55, 0.07, -4.45);
        }
        public void Build(Canvas canvas)
        {
            Parsing(canvas.Children);
        }



        private void Parsing(UIElementCollection uIElements)
        {
            foreach (var element in uIElements)
            {
                if (element is BaseObject == false) continue;

                else if (element is Room)
                {
                    foreach (var each in (element as Room).Children)
                    {
                        if (each is System.Windows.Shapes.Polygon)
                        {
                            var vertices = (each as System.Windows.Shapes.Polygon).Points;

                            MakeFloor(vertices);

                            for (int i = 0; i < vertices.Count; i++)
                            {
                                MakeWall(vertices[i], vertices[(i + 1) % vertices.Count]);
                            }
                        }

                        else if (each is InteriorObject)
                        {
                            MakeObject(each as InteriorObject);
                        }
                    }
                }

                else if (element is InteriorObject)
                {
                    MakeObject(element as InteriorObject);
                }

            }

        }

        private void MakeObject(InteriorObject source)
        {
            String type = source.GetType().Name;
            Point position = source.Center;

            switch (type)
            {
                case "Sofa":
                    Convert3D(source, @"models\sofa\ready.obj");
                    break;

                case "Chair":
                    Convert3D(source, @"models\chair\chair.obj");
                    break;

                case "Refrigerator":
                    Convert3D(source, @"models\refriger\refriger.obj");
                    break;

                case "Table":
                    Convert3D(source, @"models\desk\Desk.obj");
                    break;

                case "Tv":
                    Convert3D(source, @"models\ledtv\led_tv.obj");
                    break;
            }
        }



        private void Convert3D(BaseObject obj, String image_src)
        {
            const double adjust_scale = 0.01;

            ModelVisual3D modelVisual3D = new ModelVisual3D();
            
            switch (System.IO.Path.GetExtension(image_src).ToLower())
            {
                case ".obj":
                    modelVisual3D.Content = (new ObjReader()).Read(image_src);
                    break;

                case ".3ds":                    
                    modelVisual3D.Content = (new StudioReader()).Read(image_src);
                    break;


                default:
                    throw new InvalidOperationException("File format not supported.");
            }


            Transform3DGroup transform3DGroup = new Transform3DGroup();

            var position = (obj as InteriorObject).Center;
            TranslateTransform3D translateTransform3D = new TranslateTransform3D()
            {
                OffsetX = position.Y,
                OffsetY = position.X,
                OffsetZ = 0
            };
            ScaleTransform3D scaleTransform3D = new ScaleTransform3D()
            {
                ScaleX = adjust_scale,
                ScaleY = adjust_scale,
                ScaleZ = adjust_scale
            };

            AxisAngleRotation3D axisAngleRotation3D = new AxisAngleRotation3D()
            {
                Axis = new Vector3D(1, 0, 0),
                Angle = 90
            };
            RotateTransform3D rotateTransform3D = new RotateTransform3D()
            {
                Rotation = axisAngleRotation3D
            };

            transform3DGroup.Children.Add(rotateTransform3D);
            transform3DGroup.Children.Add(translateTransform3D);
            transform3DGroup.Children.Add(scaleTransform3D);

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
        private void MakeWall(Point poststone1, Point poststone2)
        {
            MeshGeometry3D triangleMesh = new MeshGeometry3D();
            Material material = new DiffuseMaterial(new ImageBrush()
            {
                ImageSource = new BitmapImage(new Uri(@"pack://application:,,,/image/wallpaper3.jpg"))
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

            ScaleTransform3D scaleTransform3D = new ScaleTransform3D() {
                ScaleX = 0.01, ScaleY = 0.01, ScaleZ = 0.01
            };
            
            ModelVisual3D model = new ModelVisual3D() {
                Transform = scaleTransform3D,
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

            model = new ModelVisual3D() {
                Transform = scaleTransform3D,
                Content = new GeometryModel3D(triangleMesh, material)
            };
            this.Children.Add(model);
        }
        private void MakeFloor(PointCollection vertices)
        {
            MeshGeometry3D triangleMesh;
            Material material = new DiffuseMaterial(new ImageBrush(new BitmapImage(new Uri(@"pack://application:,,,/image/marble.jpg"))));

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


                ScaleTransform3D scaleTransform3D = new ScaleTransform3D() {
                    ScaleX = 0.01, ScaleY = 0.01, ScaleZ = 0.01
                };

                ModelVisual3D model = new ModelVisual3D() {
                    Content = new GeometryModel3D(triangleMesh, material),
                    Transform = scaleTransform3D
                };
                this.Children.Add(model);
            }
        }


    }
}
