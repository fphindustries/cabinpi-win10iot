using Fphi.Cabin.Pi.Common.Services;
using Fphi.CabinPi.Common.Helpers;
using Fphi.CabinPi.Common.Models;
using Fphi.CabinPi.Ui.Helpers;
using Fphi.CabinPi.Ui.Services;
using System;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Fphi.CabinPi.Common.Services;

namespace Fphi.CabinPi.Ui.ViewModels
{
    public class MainViewModel : Observable
    {

        private Temperature _insideTemperature;

        public Temperature InsideTemperature
        {
            get { return _insideTemperature; }
            set { Set(ref _insideTemperature, value); }
        }

        private Weather _outsideWeather;

        public Weather OutsideWeather
        {
            get { return _outsideWeather; }
            set { Set(ref _outsideWeather, value); }
        }

        // public DarkSkyService.Forecast Forecast => _darkSkyService.CurrentForecast;

        private readonly ISensorService _sensorService;
        private readonly IWeatherService _darkSkyService;
        private readonly ITemperatureDescriber _temperatureDescriber;
        private readonly IWeatherDescriber _weatherDescriber;
        private readonly ISettings _settings;

        public RelayCommand UpdateCommand { get; private set; }

        public MainViewModel(ISensorService sensorService, IWeatherService darkSkyService,
            ITemperatureDescriber temperatureDescriber, IWeatherDescriber weatherDescriber, ISettings settings)
        {
            _sensorService = sensorService;
            _settings = settings;
            _darkSkyService = darkSkyService;
            _temperatureDescriber = temperatureDescriber;
            _weatherDescriber = weatherDescriber;



            UpdateCommand = new RelayCommand(OnUpdateCommand);

        }


        private void OnUpdateCommand()
        {
            throw new NotImplementedException();
        }

        public async Task InitializeAsync()
        {
            try
            {
                //await _darkSkyService.GetForecast();GetTemperature
                InsideTemperature = await _temperatureDescriber.GetTemperature();
                OutsideWeather = await _weatherDescriber.GetWeather();

            }
            catch (Exception ex)
            {
                var dialog = new MessageDialog(ex.Message);
                await dialog.ShowAsync();
            }


        }
    }
}
