using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Uwp;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace Fphi.CabinPi.Ui.Views
{
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
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


            Vals = new ChartValues<double> { 5, 9, 8, 6, 1, 5, 7, 3, 6, 3 };
            Nan = double.NaN;

            ForecastValues = new ChartValues<double> { 50, 90, 80, 60, 55, 67, 70};

            DataContext = this;
            //SetupAppService();

            string icon = String.Format("ms-appx:///Assets/Weather/{0}.png", "01d");
            string iconCat = String.Format("ms-appx:///Assets/Inside/{0}.png", "sleepingcat");

            InsideTempTextBlock.Text = (80 + "°F");
            InsideDescTextBlock.Text = "Ah yeah...it's cozy in here";
            InsideLocationTextBlock.Text = "Inside";
            InsideImage.Source = new BitmapImage(new Uri(iconCat, UriKind.Absolute));


            OutsideTempTextBlock.Text = (65 + "°F");
            OutsideDescTextBlock.Text = "Sunny and nice...maybe you should be outside";
            OutsideLocationTextBlock.Text = "Clam Cabin";
            OutsideImage.Source = new BitmapImage(new Uri(icon, UriKind.Absolute));

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


        private double _lastReading;
        private double _trend;
        private DispatcherTimer _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };

        public SeriesCollection LastHourSeries { get; set; }
        public ChartValues<double> Vals { get; set; }
        public double Nan { get; set; }
        public ChartValues<double> ForecastValues { get; set; }
        public string TodaysDate = DateTime.Now.ToString("dd MMM yyyy");

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


        private void UpdateOnclick(object sender, RoutedEventArgs e)
        {
            TimePowerChart.Update(true);
        }

    }
}

