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
        Task<Temperature> GetTemperature();
    }

    public class TemperatureDescriber : ITemperatureDescriber
    {
        ISettings _settings;
        ISensorService _sensorService;
        private Temperature _cachedTemperature;

        private double defaultTemp = 50;
        public TemperatureDescriber(ISettings settings, ISensorService sensorService)
        {
            _sensorService = sensorService;
            _settings = settings;
        }

        public async Task<Temperature> GetTemperature()
        { 
            //internal
            
            var temp =  _sensorService.GetReading(SensorType.InteriorTemperatureF);
            var temperature = Temperature.GetTemperature(temp);
            
            return await Task.FromResult<Temperature>(Temperature.GetTemperature(temp));

        }
    }
}
