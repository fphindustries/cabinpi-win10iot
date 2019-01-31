using Fphi.CabinPi.Ui.Services;
using Fphi.CabinPi.Ui2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fphi.CabinPi.Ui.ViewModels
{
    public class MainViewModel : ViewModel
    {
        public Tempature InsideTempature { get; set; } 
        public Tempature OutsideTempature { get; set; }

        public DarkSkyService.Forecast Forecast => _darkSkyService.CurrentForecast;

        private readonly AppService _appService;
        private readonly DarkSkyService _darkSkyService;

        public MainViewModel(AppService appService, DarkSkyService darkSkyService)
        {
            _appService = appService;
            _darkSkyService = darkSkyService;
        }

        public async Task InitializeAsync()
        {
            InsideTempature = TempatureHelper.GetTempature(80, Tempature.Location.Inside);
            OutsideTempature = TempatureHelper.GetTempature(80, Tempature.Location.Outside);
            await _darkSkyService.GetForecast();
        }

    }
}
