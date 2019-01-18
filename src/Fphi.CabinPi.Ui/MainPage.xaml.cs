using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Uwp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Fphi.CabinPi.Ui
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        private double _lastReading;
        private double _trend;
        private DispatcherTimer _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };

        public MainPage()
        {
            InitializeComponent();

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

            _trend = 8;

            //_timer.Tick += (sender, o) =>
            //{
            //    var r = new Random();

            //    _trend += (r.NextDouble() > 0.3 ? 1 : -1) * r.Next(0, 5);
            //    LastHourSeries[0].Values.Add(new ObservableValue(_trend));
            //    LastHourSeries[0].Values.RemoveAt(0);
            //    SetReading();
            //};
            //_timer.Start();

            Vals = new ChartValues<double> { 5, 9, 8, 6, 1, 5, 7, 3, 6, 3 };
            Nan = double.NaN;

            DataContext = this;
            SetupAppService();
        }

        public SeriesCollection LastHourSeries { get; set; }
        public ChartValues<double> Vals { get; set; }
        public double Nan { get; set; }
        public string TodaysDate = DateTime.Now.ToString("dd MMM yyyy");
        private AppServiceConnection _backgroundAppService;

        public double LastReading
        {
            get { return _lastReading; }
            set
            {
                _lastReading = value;
                OnPropertyChanged("LastLecture");
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

        public event PropertyChangedEventHandler PropertyChanged;


        protected void OnPropertyChanged(string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void UpdateOnclick(object sender, RoutedEventArgs e)
        {
            TimePowerChart.Update(true);
        }

        private async void SetupAppService()
        {
            // find the installed application(s) that expose the app service PerimeterBreachService
            var listing = await AppServiceCatalog.FindAppServiceProvidersAsync("CabinPiAppService");
            var packageName = "";
            // there may be cases where other applications could expose the same App Service Name, in our case
            // we only have the one
            if (listing.Count == 1)
            {
                packageName = listing[0].PackageFamilyName;
            }
            _backgroundAppService = new AppServiceConnection();
            _backgroundAppService.AppServiceName = "CabinPiAppService";
            _backgroundAppService.PackageFamilyName = packageName;
            //open app service connection
            var status = await _backgroundAppService.OpenAsync();

            if (status != AppServiceConnectionStatus.Success)
            {
                //something went wrong
                Debug.WriteLine("Could not connect to the App Service: " + status.ToString());
            }
            else
            {
                //add handler to receive app service messages (Perimiter messages)
                _backgroundAppService.RequestReceived += BackgroundServiceRequestReceived;
            }
        }

        private async void BackgroundServiceRequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                // if you are doing anything awaitable, you need to get a deferral

                //var x = args.Request;
                LastHourSeries[0].Values.Add(new ObservableValue((double)args.Request.Message["InteriorTempF"]));
                LastHourSeries[0].Values.RemoveAt(0);
                SetReading();

            });
        }
    }
}

