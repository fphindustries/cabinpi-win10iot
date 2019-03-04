using Fphi.Cabin.Pi.Common.Services;
using Fphi.CabinPi.Common.Helpers;
using Fphi.CabinPi.Common.Models;
using Fphi.CabinPi.Ui.Helpers;
using Fphi.CabinPi.Ui.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Fphi.CabinPi.Common;
using Fphi.CabinPi.Common.Services;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Uwp;

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

        public SeriesCollection LastHourSeries { get; set; }

        // public DarkSkyService.Forecast Forecast => _darkSkyService.CurrentForecast;

        private readonly ISensorService _sensorService;
        private readonly IWeatherService _darkSkyService;
        private readonly ITemperatureDescriber _temperatureDescriber;
        private readonly IWeatherDescriber _weatherDescriber;
        private readonly ISettings _settings;

        public RelayCommand UpdateCommand { get; private set; }
        private DispatcherTimer _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };


        public MainViewModel(ISensorService sensorService, IWeatherService darkSkyService,
            ITemperatureDescriber temperatureDescriber, IWeatherDescriber weatherDescriber, ISettings settings)
        {
            _sensorService = sensorService;
            _settings = settings;
            _darkSkyService = darkSkyService;
            _temperatureDescriber = temperatureDescriber;
            _weatherDescriber = weatherDescriber;



            UpdateCommand = new RelayCommand(OnUpdateCommand);

            LastHourSeries = new SeriesCollection
            {
                new LineSeries
                {
                    Values = new ChartValues<ObservableValue>
                    {
                       
                    }
                }
            };

            _timer.Tick += _timer_InvalidateData;
            _timer.Start();
        }

        private void _timer_InvalidateData(object sender, object e)
        {
            InvalidateData();
        }

        public async void InvalidateData()
        {
            InsideTemperature = await _temperatureDescriber.GetTemperature();
            OutsideWeather = await _weatherDescriber.GetWeather();
            await UpdateCurrentData();
        }

        private double _lastReading;
        public double LastReading
        {
            get { return _lastReading; }
            set
            {
                _lastReading = value;
                OnPropertyChanged("LastReading");
            }
        }

        private void OnUpdateCommand()
        {
            throw new NotImplementedException();
        }

        public async Task InitializeAsync()
        {
            try
            {
                InvalidateData();
            }
            catch (Exception ex)
            {
                var dialog = new MessageDialog(ex.Message);
                await dialog.ShowAsync();
            }
        }

        public async Task UpdateCurrentData()
        {
            var lastReading = _sensorService.GetReading(SensorType.Current);
            LastHourSeries[0].Values.Add(new ObservableValue(lastReading));
            LastHourSeries[0].Values.RemoveAt(0);
            LastReading = lastReading;
        }


    }
}
