using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fphi.Cabin.Pi.Common.Services;

namespace Fphi.CabinPi.Common.Services
{
    public interface IWeatherService
    {
        Task<Forecast> GetForecast(double latitude, double longitude);
    }
}
