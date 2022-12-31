using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Media;
using DiscordRPC;
using Newtonsoft.Json;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using Label = System.Windows.Controls.Label;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;

namespace DiscordRPCGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DiscordRpcClient client = null;
        Random random = new Random();
        NotifyIcon notifyIcon = new NotifyIcon();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            DirectoryInfo d = new DirectoryInfo(@"./configs");//Assuming Test is your Folder
            FileInfo[] Files = d.GetFiles("*.json"); //Getting Text files
            foreach (FileInfo i in Files)
            {
                LoadOptions(i.Name);
                AddConfig(i.Name, true);
            }
        }

        public void DiscordUpdate()
        {
            client = null;
            client = new DiscordRpcClient(AppID.Text.ToString());
            client.Initialize();

            Assets assets = new Assets() { };

            if (LargeImg.Text != "" && LargeImgText.Text != "" && SmallImg.Text != "" && SmallImgText.Text != "")
            {
                assets = new Assets() {
                    LargeImageKey = LargeImg.Text,
                    LargeImageText = LargeImgText.Text,
                    SmallImageKey = SmallImg.Text,
                    SmallImageText = SmallImgText.Text
                };
            }
            if (LargeImg.Text != "" && LargeImgText.Text != "")
            {
                assets = new Assets()
                {
                    LargeImageKey = LargeImg.Text,
                    LargeImageText = LargeImgText.Text
                };
            }
            if (SmallImg.Text != "" && SmallImgText.Text != "")
            {
                assets = new Assets()
                {
                    SmallImageKey = SmallImg.Text,
                    SmallImageText = SmallImgText.Text
                };
            }
            RichPresence presence = new RichPresence()
            {
                State = State.Text,
                Details = Description.Text,
                Assets = assets
            };
            List<DiscordRPC.Button> b = new List<DiscordRPC.Button>();
            if (Button1URL.Text != "" && Button1Text.Text != "")
            {
                b.Add(new DiscordRPC.Button() { Label = Button1Text.Text, Url = Button1URL.Text });
            }
            if (Button2URL.Text != "" && Button2Text.Text != "")
            {
                b.Add(new DiscordRPC.Button() { Label = Button2Text.Text, Url = Button2URL.Text });
            }
            if ((bool)IsTimestamp.IsChecked)
            {
                presence.Timestamps = new Timestamps()
                {
                    Start = DateTime.UtcNow
                };
            }
            presence.Buttons = b.ToArray();
            client.SetPresence(presence);
            var timer = new System.Timers.Timer(150);
            timer.Elapsed += (sender, args) => { client.Invoke(); };
            timer.Start();
        }

        private void AddConfig(string Name, bool IsLoad = false)
        {
            if (Configs.Children.Count <= 5)
            {
                var converter = new System.Windows.Media.BrushConverter();
                Border mainB = new Border()
                {
                    Height = 60,
                    Width = 230,
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                    Margin = new Thickness(10, 10, 10, 10),
                    BorderThickness = new Thickness(3, 3, 3, 3),
                    CornerRadius = new CornerRadius(10, 10, 10, 10),
                    Background = (System.Windows.Media.Brush)converter.ConvertFromString("#FF36393F"),
                    BorderBrush = (System.Windows.Media.Brush)converter.ConvertFromString("#FF36393F"),
                };
                Border but = new Border()
                {
                    Height = 20,
                    Width = 60,
                    Margin = new Thickness(0, 0, 10, 0),
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Right,
                    BorderBrush = (System.Windows.Media.Brush)converter.ConvertFromString("#FFED4245"),
                    BorderThickness = new Thickness(1, 1, 1, 1),
                    CornerRadius = new CornerRadius(2, 2, 2, 2),
                    Cursor = System.Windows.Input.Cursors.Hand
                };
                TextBlock name = new TextBlock()
                {
                    Text = Name.Replace(".json", ""),
                    Foreground = (System.Windows.Media.Brush)converter.ConvertFromString("#FF8E9297"),
                    FontWeight = FontWeights.Bold,
                    Height = 16,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness(5, 5, 5, 5)
                };
                TextBlock file = new TextBlock()
                {
                    Text = Name.Replace(".json", "") + ".json",
                    Foreground = (System.Windows.Media.Brush)converter.ConvertFromString("#FF8E9297"),
                    FontWeight = FontWeights.Bold,
                    Height = 16,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(5, 15, 5, 5)
                };
                Label label = new Label()
                {
                    Content = "Remove",
                    Foreground = (System.Windows.Media.Brush)converter.ConvertFromString("#FFED4245"),
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                    Height = 28,
                    Width = 60,
                    HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center,
                    Cursor = System.Windows.Input.Cursors.Hand,
                    Margin = new Thickness(0, 3, 0, 0)
                };
                label.MouseEnter += Remove_MouseEnter;
                label.MouseLeave += Remove_MouseLeave;
                label.MouseUp += Remove_MouseUp;
                Grid grid = new Grid() { };
                grid.Children.Add(name);
                grid.Children.Add(file);
                grid.Children.Add(but);
                but.Child = label;
                mainB.Child = grid;
                mainB.MouseUp += Load_MouseUp;
                Configs.Children.Add(mainB);
                foreach (Border i in Configs.Children)
                {
                    i.Margin = new Thickness(10, 10 + 80 * (Configs.Children.IndexOf(i)), 10, 10);
                }
                if (IsLoad)
                {
                    SaveOptions(Name);
                }
                else
                {
                    SaveOptions(Name + ".json");
                }
            }
        }

        private void RemoveConfig(object sender)
        {
            Border mainB;
            Grid grid;
            Label label = sender as Label;
            mainB = label.Parent as Border;
            grid = mainB.Parent as Grid;
            mainB = grid.Parent as Border;
            TextBlock file = grid.Children[1] as TextBlock;
            Configs.Children.Remove(mainB);
            foreach (Border i in Configs.Children)
            {
                i.Margin = new Thickness(10, 10 + 80 * (Configs.Children.IndexOf(i)), 10, 10);
            }
            File.Delete($"./configs/{file.Text}");
        }

        private void LoadOptions(string name)
        {
            if (File.Exists($"./configs/{name}"))
            {
                string sData = File.ReadAllText($"./configs/{name}");
                ConfigName.Text = name.Replace(".json", "");
                SaveData data = JsonConvert.DeserializeObject<SaveData>(sData);
                AppID.Text = data.appid;
                Description.Text = data.Details;
                State.Text = data.State;
                LargeImg.Text = data.largeimg;
                LargeImgText.Text = data.largetext;
                SmallImg.Text = data.smallimg;
                SmallImgText.Text = data.smalltext;
                IsTimestamp.IsChecked = data.timestamps;
                if (data.Btns != "" && data.Btns != null)
                {
                    if (data.Btns.Split("=")[0].Split("-")[0] != null && data.Btns.Split("=")[0].Split("-")[1] != null)
                    {
                        Button1Text.Text = data.Btns.Split("=")[0].Split("-")[0];
                        Button1URL.Text = data.Btns.Split("=")[0].Split("-")[1];
                    }
                    try
                    {
                        if (data.Btns.Split("=")[1].Split("-")[0] != null && data.Btns.Split("=")[1].Split("-")[1] != null)
                        {
                            Button2Text.Text = data.Btns.Split("=")[1].Split("-")[0];
                            Button2URL.Text = data.Btns.Split("=")[1].Split("-")[1];
                        }
                    } catch
                    {
                        Button2Text.Text = "";
                        Button2URL.Text = "";
                    }
                }
                else
                {
                    Button1Text.Text = "";
                    Button1URL.Text = "";
                    Button2Text.Text = "";
                    Button2URL.Text = "";
                }
            }
        }

        private void SaveOptions(string name)
        {
            SaveData data = new SaveData();
            data.appid = AppID.Text.ToString();
            data.Details = Description.Text.ToString();
            data.State = State.Text.ToString();
            data.largeimg = LargeImg.Text.ToString();
            data.largetext = LargeImgText.Text.ToString();
            data.smallimg = SmallImg.Text.ToString();
            data.smalltext = SmallImgText.Text.ToString();
            data.timestamps = (bool)IsTimestamp.IsChecked;
            if (Button1URL.Text != "" && Button1Text.Text != "")
            {
                data.Btns += Button1Text.Text + "-" + Button1URL.Text + "=";
            }
            if (Button2URL.Text != "" && Button2Text.Text != "")
            {
                data.Btns += Button2Text.Text + "-" + Button2URL.Text + "=";
            }
            File.WriteAllText($"./configs/{name}", JsonConvert.SerializeObject(data));
        }

        private class SaveData
        {
            public string appid;
            public string Details;
            public string State;
            public string largeimg;
            public string largetext;
            public string smallimg;
            public string smalltext;
            public string Btns;
            public bool timestamps;
        }

        private void AddConfig_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (ConfigName.Text != "")
                {
                    AddConfig(ConfigName.Text);
                }
            }
        }

        private void Save_MouseUp(object sender, RoutedEventArgs e)
        {
            if (ConfigName.Text != "")
            {
                SaveOptions(ConfigName.Text + ".json");
            }
        }

        private void Load_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                Border border = sender as Border;
                Grid grid = border.Child as Grid;
                TextBlock file = grid.Children[1] as TextBlock;
                LoadOptions(file.Text);
            }
        }

        private void Start_MouseUp(object sender, RoutedEventArgs e)
        {
            DiscordUpdate();
            StopBut.IsEnabled = true;
            ReloadBut.IsEnabled = true;
        }

        private void Stop_MouseUp(object sender, RoutedEventArgs e)
        {
            if (client != null)
            {
                client.Deinitialize();
                StopBut.IsEnabled = false;
                ReloadBut.IsEnabled = false;
            }
        }

        private void Minimaze_MouseUp(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Hidden;
            notifyIcon = new System.Windows.Forms.NotifyIcon();
            notifyIcon.Click += new EventHandler(notifyIcon_Click);
            notifyIcon.Icon = new System.Drawing.Icon("ico.ico");
            notifyIcon.Visible = true;
        }

        private void notifyIcon_Click(object sender, EventArgs e)
        {
            Visibility = Visibility.Visible;
            notifyIcon.Visible = false;
        }

        private void Remove_MouseEnter(object sender, MouseEventArgs e)
        {
            var converter = new System.Windows.Media.BrushConverter();
            Border mainB;
            Label label = sender as Label;
            mainB = label.Parent as Border;
            label.Foreground = (Brush)converter.ConvertFromString("#FFF");
            mainB.Background = (Brush)converter.ConvertFromString("#FFED4245");
        }

        private void Remove_MouseLeave(object sender, MouseEventArgs e)
        {
            var converter = new System.Windows.Media.BrushConverter();
            Border mainB;
            Label label = sender as Label;
            mainB = label.Parent as Border;
            label.Foreground = (Brush)converter.ConvertFromString("#FFED4245");
            mainB.Background = Brushes.Transparent;
        }

        private void Remove_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                RemoveConfig(sender);
            }
        }

        private void Hover_Size_Enter(object sender, MouseEventArgs e)
        {
            Label label = sender as Label;
            label.FontSize += 16;
        }

        private void Hover_Size_Leave(object sender, MouseEventArgs e)
        {
            Label label = sender as Label;
            label.FontSize -= 16;
        }

        private void Hover_Color_Enter(object sender, MouseEventArgs e)
        {
            var converter = new System.Windows.Media.BrushConverter();
            Border border = sender as Border;
            border.Background = (Brush)converter.ConvertFromString("#FFB9BBBE");
        }

        private void Hover_Color_Leave(object sender, MouseEventArgs e)
        {
            var converter = new System.Windows.Media.BrushConverter();
            Border border = sender as Border;
            border.Background = (Brush)converter.ConvertFromString("#FF8E9297");
        }

        private void Reload_MouseUp(object sender, RoutedEventArgs e)
        {
            if (client != null)
            {
                client.Deinitialize();
            }
            DiscordUpdate();
        }
    }
}
