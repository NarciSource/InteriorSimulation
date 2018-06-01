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
        int jsonStay = -1;


        private void Changed(string command,string name)
        {
            for (int i = Jsons.Count - 1; i > jsonStay; i--)
            {
                Jsons.RemoveAt(i);
                history.Items.RemoveAt(i);
            }


            jsonStay++;
            history.Items.Add(new { Command = command, Name = name });
            Jsons.Add(SaveStatusToJson());
            history.SelectedIndex = jsonStay;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            string fileName = "";

            SaveFileDialog saveFile = new SaveFileDialog();

            //saveFile.InitialDirectory = @"C:";
            saveFile.Title = "파일 저장";
            saveFile.FileName = "interior";
            saveFile.DefaultExt = "intr";
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
                History_Clear();

                
                Jsons = JArray.Parse(fileContent);
                PrintUIfromJson(Jsons.Last);
                foreach (var records in Jsons)
                {
                    foreach (var jeach in records)
                    {
                        try
                        {
                            string type = jeach["History"].ToString();
                            var record = JsonConvert.DeserializeObject<Object>(jeach["History"].ToString());

                            history.Items.Add(record);
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
        }

        
        

        private void History_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (jsonStay != history.SelectedIndex)
            {
                jsonStay = history.SelectedIndex;
                PrintUIfromJson(Jsons[jsonStay]);
            }
        }

        private void History_Clear()
        {
            jsonStay = -1;
            history.SelectedIndex = -1;
            history.Items.Clear();
            Jsons.Clear();
        }

        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            if (0 < jsonStay)
            {
                canvas.Children.Clear();

                jsonStay--;
                PrintUIfromJson(Jsons[jsonStay]);

                history.SelectedIndex--;
            }
        }
        
        private void Redo_Click(object sender, RoutedEventArgs e)
        {
            if (jsonStay < Jsons.Count-1)
            {
                canvas.Children.Clear();

                jsonStay++;
                PrintUIfromJson(Jsons[jsonStay]);

                history.SelectedIndex++;
            }
        }

        



        private JArray SaveStatusToJson()
        {
            var jdata = new JArray();
            JObject jeach;
            
            foreach (var element in canvas.Children)
            {
                var obj = element as BaseObject;

                if (obj is Room)
                {
                    jeach = new JObject();
                    jeach.Add("Type", obj.GetType().Name);

                    var jpoints = new JArray();
                    foreach (Point point in ((Room)obj).Points)
                    {
                        jpoints.Add(JsonConvert.SerializeObject(point));
                    }

                    jeach.Add("Points", jpoints);
                    jdata.Add(jeach);
                }

                else if (obj is InteriorObject)
                {
                    InteriorObject iobj = obj as InteriorObject;
                    jeach = JObject.FromObject(
                        new
                        {
                            Type = obj.GetType().Name,
                            Name = obj.Name,
                            Width = iobj.Width,
                            Height = iobj.Height,
                            Border = iobj.BorderThickness.Left,
                            Rotate = iobj.Rotate,
                            Point = JsonConvert.SerializeObject(iobj.Center)
                        });
                    jdata.Add(jeach);
                }
            }

            jdata.Add(JObject.FromObject(
                new {
                    History = JsonConvert.SerializeObject(history.Items[jsonStay])
                }));

            return jdata;
        }

        private void PrintUIfromJson(JToken fileContent)
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

                        Room room = new Room(points);
                        canvas.Children.Add(room);
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
                        }

                        obj.Name = jeach["Name"].ToString();
                        obj.Height = JsonConvert.DeserializeObject<int>(jeach["Height"].ToString());
                        obj.Width = JsonConvert.DeserializeObject<int>(jeach["Width"].ToString());
                        obj.BorderThickness = new Thickness(JsonConvert.DeserializeObject<double>(jeach["Border"].ToString()));
                        obj.Rotate = JsonConvert.DeserializeObject<double>(jeach["Rotate"].ToString());
                        obj.Center = JsonConvert.DeserializeObject<Point>(jeach["Point"].ToString());
                        obj.Build();
                        canvas.Children.Add(obj);
                    }
                }
                catch (Exception)
                {
                    //MessageBox.Show("There is some problem with the file contents" + exc);
                }
            }
        }
        
    }
}
