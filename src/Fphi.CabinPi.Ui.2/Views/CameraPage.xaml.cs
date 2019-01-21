using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using Fphi.CabinPi.Ui.EventHandlers;
using Fphi.CabinPi.Ui.Helpers;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace Fphi.CabinPi.Ui.Views
{
    public sealed partial class CameraPage : Page, INotifyPropertyChanged, IPivotPage
    {
        public CameraPage()
        {
            InitializeComponent();
        }

        public async Task OnPivotSelectedAsync()
        {
            await cameraControl.InitializeCameraAsync();
        }

        public async Task OnPivotUnselectedAsync()
        {
            await cameraControl.CleanupCameraAsync();
        }

        private void CameraControl_PhotoTaken(object sender, CameraControlEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Photo))
            {
                Photo.Source = new BitmapImage(new Uri(e.Photo));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
