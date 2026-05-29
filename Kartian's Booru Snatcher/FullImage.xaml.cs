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

            this.AppWindow.MoveAndResize(new RectInt32
            {
                Height = 500,
                Width = 750
            });

            Img.Source = new BitmapImage(new Uri(clickedImg.FileUrl));
            TagsText.Text = "Tags: " + clickedImg.PostTags;
            SourceText.Content = "Source: " + clickedImg.SourceUrl;
            SourceText.NavigateUri = new Uri(clickedImg.SourceUrl);
        }
    }
}
