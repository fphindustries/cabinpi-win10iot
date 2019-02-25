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
        private double _trend;

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
                        new ObservableValue(3),
                        new ObservableValue(5),
                        new ObservableValue(6),
                        new ObservableValue(7),
                        new ObservableValue(3),
                        new ObservableValue(4),
                        new ObservableValue(2),
                        new ObservableValue(5),
                        new ObservableValue(8),
                        new ObservableValue(3),
                        new ObservableValue(5),
                        new ObservableValue(6),
                        new ObservableValue(7),
                        new ObservableValue(3),
                        new ObservableValue(4),
                        new ObservableValue(2),
                        new ObservableValue(5),
                        new ObservableValue(8)
                    }
                }
            };


            _timer.Tick += (sender, o) =>
            {
                var r = new Random();

                _trend += (r.NextDouble() > 0.3 ? 1 : -1) * r.Next(0, 5);
                LastHourSeries[0].Values.Add(new ObservableValue(_trend));
                LastHourSeries[0].Values.RemoveAt(0);
                SetReading();
            };
            _timer.Start();

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


        private async void SetReading()
        {
            var target = ((ChartValues<ObservableValue>)LastHourSeries[0].Values).Last().Value;
            var step = (target - _lastReading) / 4;

            await Task.Delay(100);
            LastReading += step;

            LastReading = target;
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
