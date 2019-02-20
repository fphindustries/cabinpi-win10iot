using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fphi.Cabin.Pi.Common.Services;
using Fphi.CabinPi.Common.Models;

namespace Fphi.CabinPi.Common.Services
{
    public interface IWeatherDescriber
    {
        Task<Weather> GetWeather();
    }

    public class WeatherDescriber : IWeatherDescriber
    {
        IWeatherService _weatherService;
        ISettings _settings;

        private double defaultTemp = 50;

        public WeatherDescriber(IWeatherService weatherService, ISettings settings)
        {
            _weatherService = weatherService;
            _settings = settings;
        }

        public async Task<Weather> GetWeather()
        {
            //bleh
            var forecast = await _weatherService.GetForecast(_settings.Latitude, _settings.Longitude);
            if (forecast != null)
            {
                var datapoint = forecast.Currently;

                return await Task.FromResult<Weather>(Weather.GetWeather(datapoint));
            }
            else
            {
                return await Task.FromResult<Weather>(Weather.GetWeather(null));
            }

        }
    }
}
