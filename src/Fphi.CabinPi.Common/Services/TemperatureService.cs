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
        public TemperatureService(IWeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        public async Task<Temperature> GetTemperature(TemperatureLocation location)
        {
            await Task.Delay(5000);
            //TODO: call the appropriate service (sensor or weather depending on location
            return await Task.FromResult<Temperature>(Temperature.GetTemperature(70, location));
        }
    }
}
