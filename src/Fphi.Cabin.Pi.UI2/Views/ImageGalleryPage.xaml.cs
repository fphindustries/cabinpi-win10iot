using System;

using Fphi.Cabin.Pi.UI2.ViewModels;

using Windows.UI.Xaml.Controls;

namespace Fphi.Cabin.Pi.UI2.Views
{
    public sealed partial class ImageGalleryPage : Page
    {
        private ImageGalleryViewModel ViewModel
        {
            get { return ViewModelLocator.Current.ImageGalleryViewModel; }
        }

        public ImageGalleryPage()
        {
            InitializeComponent();
        }
    }
}
