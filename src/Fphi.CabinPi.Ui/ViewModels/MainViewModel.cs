﻿using System;
using System.Threading.Tasks;
using Fphi.Cabin.Pi.Common.Services;
using Fphi.CabinPi.Common.Helpers;
using Fphi.CabinPi.Common.Models;
using Fphi.CabinPi.Ui.Helpers;
using Fphi.CabinPi.Ui.Services;

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

        private Temperature _outsideTemperature;

        public Temperature OutsideTemperature
        {
            get { return _outsideTemperature; }
            set { Set(ref _outsideTemperature, value); }
        }


        
        // public DarkSkyService.Forecast Forecast => _darkSkyService.CurrentForecast;

        private readonly AppService _appService;
        private readonly IWeatherService _darkSkyService;
        private readonly ITemperatureService _temperatureService;

        public RelayCommand UpdateCommand { get; private set; }

        public MainViewModel(AppService appService, IWeatherService darkSkyService, ITemperatureService temperatureService)
        {
            _appService = appService;
            _darkSkyService = darkSkyService;
            _temperatureService = temperatureService;

            UpdateCommand = new RelayCommand(OnUpdateCommand);

        }

        private void OnUpdateCommand()
        {
            throw new NotImplementedException();
        }

        public async Task InitializeAsync()
        {
            //await _darkSkyService.GetForecast();
            InsideTemperature = await _temperatureService.GetTemperature(TemperatureLocation.Inside);
            OutsideTemperature = await _temperatureService.GetTemperature(TemperatureLocation.Outside);
        }
    }
}