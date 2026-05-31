using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics;
using Windows.Media.Core;
using Windows.Media.Effects;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Kartian_s_Booru_Snatcher
{
    public sealed partial class FullImage : Window
    {
        public ImageData image { get; }
        public FullImage(ImageData clickedImg)
        {
            InitializeComponent();

            image = clickedImg;

            this.AppWindow.MoveAndResize(new RectInt32
            {
                Height = 700,
                Width = 950
            });

            if (clickedImg.FileUrl.EndsWith(".mp4") || clickedImg.FileUrl.EndsWith(".mov") || clickedImg.FileUrl.EndsWith(".webm"))
            {
                Vid.Visibility = Visibility.Visible;
                Vid.Source = MediaSource.CreateFromUri(new Uri(clickedImg.FileUrl));
            }
            else
            {
                Vid.Visibility = Visibility.Collapsed;
                Img.Source = new BitmapImage(new Uri(clickedImg.FileUrl));
            }
            TagsText.Text = "Tags: " + clickedImg.PostTags;
            
            if (clickedImg.SourceUrl.Trim() == "")
            {
                return;
            }
            else
            {
                SourceText.Content = "Source: " + clickedImg.SourceUrl;
                SourceText.NavigateUri = new Uri(clickedImg.SourceUrl);
            }
        }

        private void DownloadButton_Click(object sender, RoutedEventArgs e)
        {

            string fileName = image.PostTags;
            if (fileName.Length > 100)
            {
                fileName.Substring(fileName.Length - 100);
            }
            using (var client = new System.Net.WebClient())
            {
                client.DownloadFile(image.FileUrl, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", Path.GetFileName(image.FileUrl)));
            }
        }
    }
}
