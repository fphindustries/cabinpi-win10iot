using Fphi.CabinPi.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fphi.Cabin.Pi.Common.Services
{
    public interface ITemperatureService
    {
        Task<Temperature> GetTemperature(TemperatureLocation location);
    }

    public class TemperatureService : ITemperatureService
    {
        IWeatherService _weatherService;
        ISettings _settings;
        private double defaultTemp = 50;
        public TemperatureService(IWeatherService weatherService, ISettings settings)
        {
            _weatherService = weatherService;
            _settings = settings;
        }

        public async Task<Temperature> GetTemperature(TemperatureLocation location)
        {
            if (location == TemperatureLocation.Outside)
            {
                //bleh
                var forecast = await _weatherService.GetForecast(_settings.Latitude, _settings.Longitude);
                if (forecast != null)
                {
                    var temp = forecast.Currently.Temperature;
                    if (temp.HasValue)
                    {
                        return await Task.FromResult<Temperature>(Temperature.GetTemperature(temp.Value, location));
                    }
                    else
                    {
                        return await Task.FromResult<Temperature>(Temperature.GetTemperature(defaultTemp, location));
                    }
                }
                else
                {
                    return await Task.FromResult<Temperature>(Temperature.GetTemperature(defaultTemp, location));
                }
            }
            else
            {
                return await Task.FromResult<Temperature>(Temperature.GetTemperature(74, location));

            }
        }
    }
}
