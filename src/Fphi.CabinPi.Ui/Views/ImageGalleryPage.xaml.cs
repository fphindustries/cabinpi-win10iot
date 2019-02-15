using System;

using Fphi.CabinPi.Ui.ViewModels;

using Windows.UI.Xaml.Controls;

namespace Fphi.CabinPi.Ui.Views
{
    public sealed partial class ImageGalleryPage : Page
    {
        public ImageGalleryViewModel ViewModel { get; } = new ImageGalleryViewModel();

        public ImageGalleryPage()
        {
            InitializeComponent();
        }
    }
}
