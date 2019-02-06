using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using Fphi.CabinPi.Common;

namespace Fphi.CabinPi.Background
{
    class ConfigurationService:  INotifyPropertyChanged
    {
        private BackgroundConfiguration _configuration=null;

        public event PropertyChangedEventHandler PropertyChanged;


        public async Task InitAsync()
        {
            BackgroundConfiguration = await Windows.Storage.ApplicationData.Current.LocalSettings.ReadAsync<BackgroundConfiguration>(nameof(BackgroundConfiguration));
            if(BackgroundConfiguration==null)
            {
                BackgroundConfiguration = new BackgroundConfiguration();
            }
        }

        public BackgroundConfiguration BackgroundConfiguration
        {
            get { return _configuration; }
            set { Set(ref _configuration, value); }
        }

        public async Task SetConfiguration(BackgroundConfiguration configuration)
        {
            await Windows.Storage.ApplicationData.Current.LocalSettings.SaveAsync(nameof(BackgroundConfiguration), configuration);

            _configuration = configuration;
        }


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
