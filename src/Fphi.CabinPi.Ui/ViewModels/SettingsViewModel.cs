using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Fphi.Cabin.Pi.Common.Models;
using Fphi.Cabin.Pi.Common.Services;
using Fphi.CabinPi.Common.Helpers;
using Fphi.CabinPi.Ui.Helpers;
using Fphi.CabinPi.Ui.Services;

using Windows.ApplicationModel;
using Windows.UI.Xaml;

namespace Fphi.CabinPi.Ui.ViewModels
{
    public class SettingsViewModel : ViewModel
    {
        private ElementTheme _elementTheme;
        private readonly ThemeSelectorService _themeService;
        private readonly SensorService _sensorService;
        private readonly DataService _dataService;
        private readonly ISettings _settings;

        public ObservableCollection<SensorConfiguration> SensorConfigurations => _sensorService.SensorConfigurations;
        public ISettings Settings => _settings;

        public SettingsViewModel(ThemeSelectorService themeService, SensorService sensorService, ISettings settings)
        {
            _themeService = themeService;
            _sensorService = sensorService;
            _settings = settings;
            _elementTheme = _themeService.Theme;
        }

        public ElementTheme ElementTheme
        {
            get { return _elementTheme; }

            set { Set(ref _elementTheme, value); }
        }

        private string _versionDescription;

        public string VersionDescription
        {
            get { return _versionDescription; }

            set { Set(ref _versionDescription, value); }
        }

        private ICommand _switchThemeCommand;

        public ICommand SwitchThemeCommand
        {
            get
            {
                if (_switchThemeCommand == null)
                {
                    _switchThemeCommand = new RelayCommand<ElementTheme>(
                        async (param) =>
                        {
                            ElementTheme = param;
                            await _themeService.SetThemeAsync(param);
                        });
                }

                return _switchThemeCommand;
            }
        }

        private ICommand _saveSettingsCommand;
        public ICommand SaveSettingsCommand
        {
            get
            {
                if (_saveSettingsCommand == null)
                {
                    _saveSettingsCommand = new RelayCommand(OnSaveSettings);
                }
                return _saveSettingsCommand;
            }

        }

        private async void OnSaveSettings()
        {
            await _sensorService.SendConfigurationAsync();
        }

        public async Task InitializeAsync()
        {
            VersionDescription = GetVersionDescription();
            await _sensorService.RequestConfigurationAsync();
        }

        private string GetVersionDescription()
        {
            var appName = "AppDisplayName".GetLocalized();
            var package = Package.Current;
            var packageId = package.Id;
            var version = packageId.Version;

            return $"{appName} - {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        }
    }
}
