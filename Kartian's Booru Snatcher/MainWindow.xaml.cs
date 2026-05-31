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
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography.X509Certificates;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics;
using Windows.Security.Cryptography.Core;

namespace Kartian_s_Booru_Snatcher
{
    public sealed partial class MainWindow : Window
    {
        public ObservableCollection<ImageData> imageDatas { get; set; } = new ObservableCollection<ImageData>();
        public bool SettingsOpen = false;
        private int page = 0;
        public BooruConfiguration currentConfig;
        public bool DialogOpen = false;
        public MainWindow()
        {
            InitializeComponent();

            this.AppWindow.MoveAndResize(new RectInt32
            {
                Height = 500,
                Width = 750
            });

            var appPresenter = this.AppWindow.Presenter as OverlappedPresenter;
            appPresenter.IsResizable = false;
            appPresenter.IsMaximizable = false;
        }

        public async void ShowDialog(string message, string title)
        {
            if (DialogOpen) return;
            DialogOpen = true;
            ContentDialog dialog = new ContentDialog();

            dialog.XamlRoot = this.Content.XamlRoot;
            dialog.Title = title;
            dialog.Content = message;
            dialog.CloseButtonText = "Ok";
            
            await dialog.ShowAsync();
            DialogOpen = false;
        }

        private async void Snatch()
        {
            Buttons.Children.Clear();
            string[] tags = TagsTextBox.Text.Split(" ");
            page = 0;
            Snatching snatching = new Snatching();
            if (currentConfig == null)
            {
                ShowDialog("No configuration activated. Activate the configuration first.", "Configuration error");
                return;
            }
            else
            {
                snatching.link = currentConfig.Url;
            }
            snatching.Declaration();
            SnatchingResult datas = await snatching.RetrieveImages(tags, page, 50, currentConfig, this);
            if (datas.ErrorTitle != null)
            {
                ShowDialog(datas.ErrorDescription, datas.ErrorTitle);
                return;
            }
            imageDatas.Clear();
            foreach (var el in datas.Images)
            {
                imageDatas.Add(el);
            }
            Button SnatchMore = new Button();
            SnatchMore.Content = "Snatch more";
            SnatchMore.Click += SnatchMore_Click;
            SnatchMore.Name = "SnatchMoreButton";
            Buttons.Children.Add(SnatchMore);
        }

        private async void SnatchMore()
        {
            page++;
            string[] tags = TagsTextBox.Text.Split(" ");
            Snatching snatching = new Snatching();
            snatching.link = currentConfig.Url;
            snatching.Declaration();
            SnatchingResult datas = await snatching.RetrieveImages(tags, page, 50, currentConfig, this);
            if (datas.ErrorTitle != null)
            {
                ShowDialog(datas.ErrorDescription, datas.ErrorTitle);
                return;
            }
            foreach (var el in datas.Images)
            {
                imageDatas.Add(el);
            }
        }

        private void SnatchMore_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            SnatchMore();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Snatch();
        }

        private void Images_ItemClick(object sender, ItemClickEventArgs e)
        {
            var clickedItem = e.ClickedItem as ImageData;

            if (clickedItem != null)
            {
                FullImage fullImageWindows = new FullImage(clickedItem);
                fullImageWindows.Activate();
            }
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            if (!SettingsOpen)
            {
                SettingsOpen = true;
                BooruConfig booruConfigWindow = new BooruConfig(this);
                booruConfigWindow.Activate();
            }
        }

        public void ChangeUsedConfigText(string configTitle, string configEngine)
        {
            UsingConfigAboutText.Text = $"Using: {configTitle}, {configEngine}";
        }
    }
}
