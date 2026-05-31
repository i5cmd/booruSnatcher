using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.Json;
using System.Threading;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics;

namespace Kartian_s_Booru_Snatcher
{
    public enum BooruEngine
    {
        Gelbooru,
        Danbooru,
        Moebooru
    }
    public class BooruConfiguration
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public BooruEngine Engine { get; set; }
        public string Api { get; set; }
        public string UserId { get; set; }

    }
    public sealed partial class BooruConfig : Window
    {
        private MainWindow window;
        public ObservableCollection<BooruConfiguration> configs;
        public BooruConfig(MainWindow mainWindow)
        {
            InitializeComponent();
            this.Closed += BooruConfig_Closed;
            this.AppWindow.MoveAndResize(new RectInt32
            {
                Width = 400,
                Height = 700
            });

            window = mainWindow;

            var appWindowPresenter = this.AppWindow.Presenter as OverlappedPresenter;
            appWindowPresenter.IsResizable = false;
            appWindowPresenter.IsMaximizable = false;
            LoadJson();

        }
        private void BooruConfig_Closed(object sender, WindowEventArgs e)
        {
            window.SettingsOpen = false;
        }

        private void DeleteConfig_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            BooruConfiguration toDelete = (BooruConfiguration)button.DataContext;
            configs.Remove(toDelete);
            SaveJson();
        }
        private void UseConfig_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            BooruConfiguration toBeUsed = (BooruConfiguration)button.DataContext;
            window.currentConfig = toBeUsed;
            window.ChangeUsedConfigText(toBeUsed.Name, toBeUsed.Engine.ToString());
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            BooruConfiguration config = new BooruConfiguration();
            string link = "";
            if (BooruUrl.Text.Trim() == "" || !Uri.TryCreate(BooruUrl.Text, UriKind.Absolute, out _))
            {
                return;
            }
            else
            {
                if (!BooruUrl.Text.EndsWith("/"))
                {
                    BooruUrl.Text = BooruUrl.Text + "/";
                    link = BooruUrl.Text;
                    config.Url = link;
                }
                else
                {
                    link = BooruUrl.Text;
                    config.Url = link;
                }
            }
            if (BooruName.Text.Trim() == "")
            {
                config.Name = BooruUrl.Text;
            }
            else
            {
                config.Name = BooruName.Text;
            }
            config.Engine = (BooruEngine)Enum.Parse(typeof(BooruEngine), BooruEngineComboBox.SelectedItem.ToString());
            config.Api = ApiKey.Text;
            config.UserId = UserId.Text;
            configs.Add(config);
            SaveJson();
        }

        public void SaveJson()
        {
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string myAppFolder = Path.Combine(localAppData, "Kartian's Booru Snatcher");
            Directory.CreateDirectory(myAppFolder);
            string file = Path.Combine(myAppFolder, "booruconfigurations.json");
            string json = JsonSerializer.Serialize(configs, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(file, json);
        }

        public void LoadJson()
        {
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string myAppFolder = Path.Combine(localAppData, "Kartian's Booru Snatcher");
            Directory.CreateDirectory(myAppFolder);
            string file = Path.Combine(myAppFolder, "booruconfigurations.json");
            if (File.Exists(file))
            {
                string fileContent = File.ReadAllText(file);
                configs = JsonSerializer.Deserialize<ObservableCollection<BooruConfiguration>>(fileContent);
            }
            else
            {
                configs = new ObservableCollection<BooruConfiguration>();
            }
        }
    }
}
