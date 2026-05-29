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
using System.Security.Cryptography.X509Certificates;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics;
using Windows.Security.Cryptography.Core;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Kartian_s_Booru_Snatcher
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public ObservableCollection<ImageData> imageDatas { get; set; } = new ObservableCollection<ImageData>();
        private int page = 0;
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

        private async void Snatch()
        {
            Buttons.Children.Clear();
            string[] tags = TagsTextBox.Text.Split(" ");
            page = 0;
            Snatching snatching = new Snatching();
            snatching.Declaration("https://safebooru.org/");
            List<ImageData> datas = await snatching.RetrieveImages(tags, "undefined", page, 200);
            imageDatas.Clear();
            foreach (var el in datas)
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
            snatching.Declaration("https://safebooru.org/");
            List<ImageData> datas = await snatching.RetrieveImages(tags, "undefined", page, 200);
            foreach (var el in datas)
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
    }
}
