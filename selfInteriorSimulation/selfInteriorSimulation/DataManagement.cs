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
using System.Windows.Media.Imaging;

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

        private void Save()
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

        private void Open()
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
            var json_lv1 = new JArray();
            JObject json_lv2;
            
            foreach (var element in canvas.Children)
            {
                if (!(element is BaseObject)) continue;

                json_lv2 = new JObject();
                if (element is Room)
                {
                    var room = element as Room;
                    
                    json_lv2.Add("Type", room.GetType().Name);


                    var jpoints = new JArray();
                    foreach (Point point in room.Points)
                    {
                        jpoints.Add(JsonConvert.SerializeObject(point));
                    }
                    json_lv2.Add("Points", jpoints);


                    json_lv2.Add("Interior", new JArray());
                    json_lv2.Add("Doors", new JArray());
                    json_lv2.Add("Windows", new JArray());

                    foreach (var each in room.Children)
                    {
                        if (each is InteriorObject)
                        {
                            var iobj = each as InteriorObject;
                            ((JArray)json_lv2["Interior"]).Add(
                                JObject.FromObject(new
                                {
                                    Type = iobj.GetType().Name,
                                    Name = iobj.Name,
                                    Width = iobj.Width,
                                    Height = iobj.Height,
                                    Border = iobj.BorderThickness.Left,
                                    Rotate = iobj.Rotate,
                                    Point = JsonConvert.SerializeObject(iobj.Center)
                                })
                            );
                        }
                        else if (each is Room.Gate)
                        {
                            var gate = each as Room.Gate;

                            JArray json_lv4 = new JArray();
                            json_lv4.Add(
                                JsonConvert.SerializeObject(new Point(gate.Line.X1, gate.Line.Y1))
                            );
                            json_lv4.Add(
                                JsonConvert.SerializeObject(new Point(gate.Line.X2, gate.Line.Y2))
                            );

                            if(each is Room.Door)
                                ((JArray)json_lv2["Doors"]).Add(json_lv4);
                            else if(each is Room.WindowObject)
                                ((JArray)json_lv2["Windows"]).Add(json_lv4);
                        }
                    }
                    
                }

                else if (element is InteriorObject)
                {
                    var iobj = element as InteriorObject;
                    json_lv2 = JObject.FromObject(
                        new
                        {
                            Type = iobj.GetType().Name,
                            Name = iobj.Name,
                            Width = iobj.Width,
                            Height = iobj.Height,
                            Border = iobj.BorderThickness.Left,
                            Rotate = iobj.Rotate,
                            Point = JsonConvert.SerializeObject(iobj.Center)
                        });
                }

                json_lv1.Add(json_lv2);
            }

            json_lv1.Add(JObject.FromObject(
                new { History = JsonConvert.SerializeObject(history.Items[jsonStay]) }
            ));

            return json_lv1;
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


                        foreach (JObject jinterior in jeach["Interior"])
                        {
                            room.Children.Add(
                                MakeInteriorObject(jinterior["Type"].ToString(), jinterior));
                        }

                        foreach (JArray jgates in jeach["Doors"])
                        {
                            Point p1= JsonConvert.DeserializeObject<Point>(jgates[0].ToString());
                            Point p2 = JsonConvert.DeserializeObject<Point>(jgates[1].ToString());

                            room.AddDoor(p1, p2);
                        }

                        foreach (JArray jgates in jeach["Windows"])
                        {
                            Point p1 = JsonConvert.DeserializeObject<Point>(jgates[0].ToString());
                            Point p2 = JsonConvert.DeserializeObject<Point>(jgates[1].ToString());

                            room.AddWindow(p1, p2);
                        }
                       

                        canvas.Children.Add(room);
                    }
                    else
                    {
                        canvas.Children.Add(
                            MakeInteriorObject(jeach["Type"].ToString(), jeach));
                    }
                }
                catch (Exception)
                {
                    //MessageBox.Show("There is some problem with the file contents" + exc);
                }
            }
        }
        private InteriorObject MakeInteriorObject(String type, JToken jdata)
        {
            InteriorObject obj = new InteriorObject();
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

            obj.Name = jdata["Name"].ToString();
            obj.Height = JsonConvert.DeserializeObject<int>(jdata["Height"].ToString());
            obj.Width = JsonConvert.DeserializeObject<int>(jdata["Width"].ToString());
            obj.BorderThickness = new Thickness(JsonConvert.DeserializeObject<double>(jdata["Border"].ToString()));
            obj.Rotate = JsonConvert.DeserializeObject<double>(jdata["Rotate"].ToString());
            obj.Center = JsonConvert.DeserializeObject<Point>(jdata["Point"].ToString());

            return obj;
        }




        private void Save_as_Image()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Png Image|*.png";
            saveFileDialog.Title = "Save an Image File";
            saveFileDialog.ShowDialog();

            if (saveFileDialog.FileName == "")
                saveFileDialog.FileName = "image.png";

            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap(
                (int)canvas.RenderSize.Width,
                (int)canvas.RenderSize.Height,
                96d, 96d, System.Windows.Media.PixelFormats.Default);

            renderTargetBitmap.Render(canvas);

            BitmapEncoder pngEncoder = new PngBitmapEncoder();
            pngEncoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));


            using (var fs = System.IO.File.OpenWrite(saveFileDialog.FileName))
            {
                pngEncoder.Save(fs);
            }
        }

    }
}
