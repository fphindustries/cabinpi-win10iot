using Fphi.CabinPi.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fphi.CabinPi.Common;
using Fphi.CabinPi.Common.Services;

namespace Fphi.Cabin.Pi.Common.Services
{
    public interface ITemperatureDescriber
    {
        Task<Temperature> GetTemperature(TemperatureLocation location);
    }

    public class TemperatureDescriber : ITemperatureDescriber
    {
        IWeatherService _weatherService;
        ISettings _settings;
        ISensorService _sensorService;

        private double defaultTemp = 50;
        public TemperatureDescriber(IWeatherService weatherService, ISettings settings, ISensorService sensorService)
        {
            _weatherService = weatherService;
            _sensorService = sensorService;
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
                //internal
                var temp =  _sensorService.GetReading(SensorType.InteriorTemperatureF);
                return await Task.FromResult<Temperature>(Temperature.GetTemperature(temp, location));

            }
        }
    }
}
