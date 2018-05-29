using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace selfInteriorSimulation
{
    public partial class MainWindow : Window
    {
        public JArray Jsons = new JArray();


        private void Changed(string command,string name)
        {
            history.Items.Add(new { Command = command, Name = name });
            Jsons.Add(saveStatusToJson());
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            string fileName = "";

            SaveFileDialog saveFile = new SaveFileDialog();

            //saveFile.InitialDirectory = @"C:";
            saveFile.Title = "파일 저장";
            saveFile.FileName = "마이다스 인테리어";
            saveFile.DefaultExt = "txt";
            Nullable<bool> result = saveFile.ShowDialog();

            if (result == true)
            {
                fileName = saveFile.FileName.ToString();
            }

            // 파일에 write
            try
            {
                StreamWriter sw = new StreamWriter(fileName);
                sw.WriteLine(Jsons.ToString());
                sw.Close();
            }
            catch (Exception ez)
            {
                Console.WriteLine("Exception: " + ez.Message);
            }
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            String fileContent = "";
            String fileName = "";

            OpenFileDialog openFileDialog = new OpenFileDialog();
            //openFileDialog.InitialDirectory = @"C:\\";
            openFileDialog.Title = "파일 선택";
            Nullable<bool> result = openFileDialog.ShowDialog();

            if (result == true)
            {
                fileName = openFileDialog.FileName.ToString();
            }

            if (fileName != null | fileName != "")
            {
                try
                {
                    StreamReader sr = new StreamReader(fileName);
                    fileContent = sr.ReadToEnd();
                    sr.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("파일을 열 수 없습니다. " + ex.Message);
                }
            }

            if (fileContent != null && fileContent != "")
            {
                canvas.Children.Clear();
                Jsons = JArray.Parse(fileContent);
                printUIfromJson(Jsons.Last);
            }
        }



        int jsonStay = 0;
        List<BasicObject> redocollection = new List<BasicObject>();
        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            if (0 < jsonStay - 1)
            {
                canvas.Children.Clear();
                jsonStay--;
                printUIfromJson(Jsons[jsonStay - 1]);
            }
        }
        
        private void Redo_Click(object sender, RoutedEventArgs e)
        {
            /*
            if (redocollection.Count > 0)
            {
                canvas.Children.Add(redocollection[redocollection.Count - 1]);
                redocollection.RemoveAt(redocollection.Count - 1);
            }
            */
            if (jsonStay < Jsons.Count && 0 < jsonStay)
            {
                canvas.Children.Clear();
                printUIfromJson(Jsons[jsonStay]);
                jsonStay++;
            }
        }

        private JArray saveStatusToJson()
        {
            jsonStay++;
            var jdata = new JArray();

            foreach (BasicObject obj in canvas.Children)
            {
                if (obj.isType == BasicObject.IsType.Room)
                {
                    var jeach = new JObject();
                    jeach.Add("Type", obj.GetType().Name);

                    var jpoints = new JArray();
                    foreach (Point point in ((Room)obj).points)
                    {
                        jpoints.Add(JsonConvert.SerializeObject(point));
                    }

                    jeach.Add("Points", jpoints);
                    jdata.Add(jeach);
                }
                else if (obj.isType == BasicObject.IsType.Chair ||
                    obj.isType == BasicObject.IsType.Refrigerator ||
                    obj.isType == BasicObject.IsType.Sofa ||
                    obj.isType == BasicObject.IsType.Table ||
                    obj.isType == BasicObject.IsType.Tv ||
                    obj.isType == BasicObject.IsType.Washer ||
                    obj.isType == BasicObject.IsType.door ||
                    obj.isType == BasicObject.IsType.window)
                {
                    var jeach = JObject.FromObject(
                        new
                        {
                            Type = obj.GetType().Name,
                            Name = obj.Name,
                            Width = ((InteriorObject)obj).Width,
                            Height = ((InteriorObject)obj).Height,
                            Border = ((InteriorObject)obj).BorderThickness.Left,
                            Rotate = ((InteriorObject)obj).rotate,
                            Point = JsonConvert.SerializeObject(((InteriorObject)obj).Point)
                        });
                    jdata.Add(jeach);
                }
            }
            return jdata;
        }

        private void printUIfromJson(JToken fileContent)
        {
            canvas.Children.Clear();

            foreach (var jeach in fileContent)
            {
                try
                {
                    string type = jeach["Type"].ToString();

                    if (type == "Room")
                    {
                        PointCollection points = new PointCollection();
                        foreach (var jpoint in jeach["Points"])
                        {
                            points.Add(JsonConvert.DeserializeObject<Point>(jpoint.ToString()));
                        }

                        new Room(points);
                    }
                    else
                    {
                        // interior Obj 생성
                        InteriorObject obj = null;
                        switch (type)
                        {
                            case "Chair":
                                obj = new Chair();
                                break;
                            case "Refrigerator":
                                obj = new Refrigerator();
                                break;
                            case "Sofa":
                                obj = new Sofa();
                                break;
                            case "Table":
                                obj = new Table();
                                break;
                            case "Tv":
                                obj = new Tv();
                                break;
                            case "Washer":
                                obj = new Washer();
                                break;
                            case "Door":
                                obj = new Door();
                                break;
                            case "WindowObject":
                                obj = new WindowObject();
                                break;
                        }

                        obj.Name = jeach["Name"].ToString();
                        obj.Height = JsonConvert.DeserializeObject<int>(jeach["Height"].ToString());
                        obj.Width = JsonConvert.DeserializeObject<int>(jeach["Width"].ToString());
                        obj.BorderThickness = new Thickness(JsonConvert.DeserializeObject<double>(jeach["Border"].ToString()));
                        obj.rotate = JsonConvert.DeserializeObject<double>(jeach["Rotate"].ToString());
                        obj.Point = JsonConvert.DeserializeObject<Point>(jeach["Point"].ToString());
                    }
                }
                catch (Exception exc)
                {
                    MessageBox.Show("There is some problem with the file contents" + exc);
                }
            }
        }
        
    }
}
