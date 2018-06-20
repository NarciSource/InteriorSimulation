using HelixToolkit.Wpf;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Threading;

namespace selfInteriorSimulation
{
    class MetaData
    {
        static private MetaData instance = new MetaData();
        protected MetaData() { }
        static public MetaData GetInstance { get { return instance; } }

        dynamic metaData;
        Dictionary<string, Model3DGroup> modelData = new Dictionary<string, Model3DGroup>();

        
        public Action ProgressStatueValueUp { get; set; }
        public Action ProgressStatueMaximumUp { get; set; }
        public Action<dynamic> AddItem { get; set; }
        public Dictionary<string, Model3DGroup> ModelData { get { return modelData; } }
        public Canvas Canvas { get; set; }

        public List<Room> AllRooms
        {
            get { return Canvas.Children.OfType<Room>().Select((room) => { return room; }).ToList(); }
        }

        public void ReadMetaData(string content)
        {
            metaData = JArray.Parse(content).Select((jobject) =>
            {
                return new
                {
                    Type = jobject["Type"].ToString(),
                    Name = jobject["Type"].ToString(),
                    TabImgSrc = new BitmapImage(new Uri(jobject["TabImgSrc"].ToString(), UriKind.Relative)),
                    ViewImgSrc = new BitmapImage(new Uri(jobject["ViewImgSrc"].ToString(), UriKind.Relative)),
                    ViewImgW = Convert.ToDouble(jobject["ViewImgW"]),
                    ViewImgH = Convert.ToDouble(jobject["ViewImgH"]),
                    MdSrc = jobject["MdSrc"].ToString()
                };
            }).ToArray();

            foreach (var data in metaData)
            {
                RoadModel(data.MdSrc);

                if (data.Type == "Door" || data.Type == "Window") continue;

                AddItem(data);

            }
        }

        public void RoadModel(String model_src)
        {
            if (modelData.ContainsKey(model_src) == true) return;

            ProgressStatueMaximumUp();

            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (o, e) =>
            {
                switch (System.IO.Path.GetExtension(model_src).ToLower())
                {
                    case ".fbx":
                    case ".obj":
                        modelData[model_src] = CopyToWPFThread((new ObjReader()).Read(model_src));
                        break;

                    case ".3ds":
                        modelData[model_src] = CopyToWPFThread((new StudioReader()).Read(model_src));
                        break;

                    default:
                        throw new InvalidOperationException("File format not supported.");
                }
            };
            worker.RunWorkerCompleted += (o, e) => { ProgressStatueValueUp(); };

            worker.RunWorkerAsync();
        }

        private Model3DGroup CopyToWPFThread(Model3DGroup src_model)
        {
            /** disassemble */
            var geometrymodel = src_model.Children[0] as GeometryModel3D;
            var geometry = geometrymodel.Geometry as MeshGeometry3D;
            
            string str_vertices = geometry.Positions.ToString();
            string str_normals = geometry.Normals.ToString();
            string str_indices = geometry.TriangleIndices.ToString();
            string str_textures = geometry.TextureCoordinates.ToString();

            Model3DGroup result_model = null;
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {
                result_model = new Model3DGroup();
                result_model.Children.Add(new GeometryModel3D()
                {
                    Geometry = new MeshGeometry3D()
                    {
                        /** re-assemble */
                        Positions = Point3DCollection.Parse(str_vertices),
                        Normals = Vector3DCollection.Parse(str_normals),
                        TriangleIndices = Int32Collection.Parse(str_indices),
                        TextureCoordinates = PointCollection.Parse(str_textures)
                    },
                    Material = new DiffuseMaterial(new ImageBrush(new BitmapImage(new Uri(@"image/marble.jpg", UriKind.Relative))))
                });
            }));

            return result_model;
        }

        public dynamic GetObjectData(string type)
        {
            foreach (var data in metaData)
            {
                if (data.Type == type) return data;
            }
            throw new Exception("Meta data haven't this data.");
        }
    }




}
