using System;
using System.Threading.Tasks;
using Fphi.Cabin.Pi.UI2.Core.Helpers;
using Fphi.Cabin.Pi.UI2.Services;
using Fphi.CabinPi.UI2.Core.Models;
using GalaSoft.MvvmLight;

namespace Fphi.Cabin.Pi.UI2.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public NotifyTaskCompletion<Tempature> InsideTempature { get; set; }
        public NotifyTaskCompletion<Tempature> OutsideTempature { get; set; }

        // public DarkSkyService.Forecast Forecast => _darkSkyService.CurrentForecast;

        private readonly AppService _appService;
        private readonly IWeatherService _darkSkyService;
        private readonly ITempatureService tempatureService;

        public MainViewModel(AppService appService, IWeatherService darkSkyService, ITempatureService tempatureService)
        {
            _appService = appService;
            _darkSkyService = darkSkyService;
            InsideTempature = new NotifyTaskCompletion<Tempature>(tempatureService.GetTempature(TempatureLocation.Inside));
            OutsideTempature = new NotifyTaskCompletion<Tempature>(tempatureService.GetTempature(TempatureLocation.Outside));

        }

        public async Task InitializeAsync()
        {
            //await _darkSkyService.GetForecast();
        }
    }
}
