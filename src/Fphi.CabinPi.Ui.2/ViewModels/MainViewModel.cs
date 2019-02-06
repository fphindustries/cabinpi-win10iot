using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Fphi.CabinPi.Ui.Models;
using Fphi.CabinPi.Ui.Services;
using Fphi.CabinPi.Ui2.Models;

namespace Fphi.CabinPi.Ui.ViewModels
{
    public class MainViewModel : ViewModel
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

