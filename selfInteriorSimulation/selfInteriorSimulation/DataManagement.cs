using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace selfInteriorSimulation
{
    public partial class MainWindow : Window
    {
        JArray metaData;
        private void ReadMetaData(string content)
        {
            metaData = JArray.Parse(content);


            foreach (var metaObject in metaData)
            {
                Button button = new Button()
                {
                    Margin = new Thickness(0, 0, 0, 0),
                    Width = 70,
                };
                {
                    StackPanel stackPanel = new StackPanel()
                    {
                        Orientation = Orientation.Vertical,
                        HorizontalAlignment = HorizontalAlignment.Left,
                    };
                    {
                        stackPanel.Children.Add(new Image()
                        {
                            Height = 50,
                            Source = new BitmapImage(new Uri(metaObject["TabImgSrc"].ToString(), UriKind.Relative))
                        });
                        stackPanel.Children.Add(new TextBlock()
                        {
                            HorizontalAlignment = HorizontalAlignment.Center,
                            Text = metaObject["Type"].ToString()
                        });
                    }


                    button.Click += (o, e) =>
                    {
                        paintingObject = Furniture.Temporary(new Furniture()
                        {
                            Type = metaObject["Type"].ToString(),
                            Name = metaObject["Type"].ToString(),
                            Width = Convert.ToDouble(metaObject["ViewImgW"]),
                            Height = Convert.ToDouble(metaObject["ViewImgH"]),
                            ImageSource = new BitmapImage(new Uri(metaObject["ViewImgSrc"].ToString(), UriKind.Relative)),
                            ModelSource = metaObject["MdSrc"].ToString()
                        });

                        canvas.Children.Add(paintingObject);
                    };


                    button.Content = stackPanel;
                }
                tab_objects.Children.Add(button);
            }
        }






        public JArray Records = new JArray();
        int recordStay = -1;


        private void Changed(string command,string name)
        {
            for (int i = Records.Count - 1; i > recordStay; i--)
            {
                Records.RemoveAt(i);
                history.Items.RemoveAt(i);
            }


            recordStay++;
            history.Items.Add(new { Command = command, Name = name });
            Records.Add(SaveStatusToJson());
            history.SelectedIndex = recordStay;
        }



        private void Save()
        {
            string fileName = "";

            SaveFileDialog saveFile = new SaveFileDialog();
            
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
                sw.WriteLine(Records.ToString());
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

                
                Records = JArray.Parse(fileContent);
                PrintUIfromJson(Records.Last);
                foreach (var records in Records)
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
            if (recordStay != history.SelectedIndex)
            {
                recordStay = history.SelectedIndex;
                PrintUIfromJson(Records[recordStay]);
            }
        }

        private void History_Clear()
        {
            recordStay = -1;
            history.SelectedIndex = -1;
            history.Items.Clear();
            Records.Clear();
        }

        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            if (0 < recordStay)
            {
                canvas.Children.Clear();

                recordStay--;
                PrintUIfromJson(Records[recordStay]);

                history.SelectedIndex--;
            }
        }
        
        private void Redo_Click(object sender, RoutedEventArgs e)
        {
            if (recordStay < Records.Count-1)
            {
                canvas.Children.Clear();

                recordStay++;
                PrintUIfromJson(Records[recordStay]);

                history.SelectedIndex++;
            }
        }

        



        private JArray SaveStatusToJson()
        {
            var jdata = new JArray();
            

            jdata = JArray.FromObject(
                canvas.Children.OfType<Room>().Select((room) =>
                {
                    return JObject.FromObject(new
                    {
                        Type = room.GetType().Name,

                        Points = JArray.FromObject(
                            room.Points.Select((point) => { return JsonConvert.SerializeObject(point); }
                        )),

                        Interior = JArray.FromObject(
                            room.Children.OfType<Furniture>().Select((furniture) =>
                            {
                                return JObject.FromObject(new
                                {
                                    Type = furniture.Type,
                                    Name = furniture.Name,
                                    Width = furniture.Width,
                                    Height = furniture.Height,
                                    Border = furniture.BorderThickness.Left,
                                    Rotate = furniture.Rotate,
                                    Point = JsonConvert.SerializeObject(furniture.Center)
                                });
                            })
                        ),

                        Doors = JArray.FromObject(
                            room.Children.OfType<Room.Gate>()
                            .Where((gate) =>
                            {
                                return gate.Type == "Door";
                            })
                            .Select((gate) =>
                            {
                                JArray json_lv4 = new JArray();
                                json_lv4.Add(JsonConvert.SerializeObject(new Point(gate.Line.X1, gate.Line.Y1)));
                                json_lv4.Add(JsonConvert.SerializeObject(new Point(gate.Line.X2, gate.Line.Y2)));
                                return json_lv4;
                            })
                        ),

                        Windows = JArray.FromObject(
                            room.Children.OfType<Room.Gate>()
                            .Where((gate) =>
                            {
                                return gate.Type == "Window";
                            })
                            .Select((gate) =>
                            {
                                JArray jpoints = new JArray();
                                jpoints.Add(JsonConvert.SerializeObject(new Point(gate.Line.X1, gate.Line.Y1)));
                                jpoints.Add(JsonConvert.SerializeObject(new Point(gate.Line.X2, gate.Line.Y2)));
                                return jpoints;
                            })
                        )


                    });
                }
            ));

            jdata.Merge(JArray.FromObject(
                canvas.Children.OfType<Furniture>().Select((furniture) =>
                {
                    return JObject.FromObject(new
                    {
                        Type = furniture.Type,
                        Name = furniture.Name,
                        Width = furniture.Width,
                        Height = furniture.Height,
                        Border = furniture.BorderThickness.Left,
                        Rotate = furniture.Rotate,
                        Point = JsonConvert.SerializeObject(furniture.Center)
                    });
                })
            ));
            
            jdata.Add(JObject.FromObject(
                new { History = JsonConvert.SerializeObject(history.Items[recordStay]) }
            ));

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
                        Room room = new Room(new PointCollection(jeach["Points"].Select((jpoint) =>
                        {
                            return JsonConvert.DeserializeObject<Point>(jpoint.ToString());
                        })));

                        foreach (JObject jinterior in jeach["Interior"])
                        {
                            room.Children.Add(
                                MakeFurniture(jinterior["Type"].ToString(), jinterior));
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
                            MakeFurniture(jeach["Type"].ToString(), jeach));
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        private Furniture MakeFurniture(String type, JToken jdata)
        {
            Furniture furniture = new Furniture();

            foreach (var metaObject in metaData)
            {
                if (metaObject["Type"].ToString() == type)
                {
                    furniture.Type = metaObject["Type"].ToString();
                    furniture.ImageSource = new BitmapImage(new Uri(metaObject["ViewImgSrc"].ToString(), UriKind.Relative));
                    furniture.ModelSource = metaObject["MdSrc"].ToString();
                    break;
                }
            }
            furniture.Name = jdata["Name"].ToString();
            furniture.Height = JsonConvert.DeserializeObject<int>(jdata["Height"].ToString());
            furniture.Width = JsonConvert.DeserializeObject<int>(jdata["Width"].ToString());
            furniture.BorderThickness = new Thickness(JsonConvert.DeserializeObject<double>(jdata["Border"].ToString()));
            furniture.Rotate = JsonConvert.DeserializeObject<double>(jdata["Rotate"].ToString());
            furniture.Center = JsonConvert.DeserializeObject<Point>(jdata["Point"].ToString());

            return furniture;
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
