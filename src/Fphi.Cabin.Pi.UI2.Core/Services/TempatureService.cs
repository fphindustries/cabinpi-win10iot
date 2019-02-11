using Fphi.CabinPi.UI2.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fphi.CabinPi.Ui.Services
{
    public interface ITempatureService
    {
        Task<Tempature> GetTempature(TempatureLocation location);
    }

    public class TempatureService : ITempatureService
    {
        IWeatherService _weatherService;
        public TempatureService(IWeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        public async Task<Tempature> GetTempature(TempatureLocation location)
        {
            await Task.Delay(5000);
            //TODO: call the appropriate service (sensor or weather depending on location
            return await Task.FromResult<Tempature>(Tempature.GetTempature(70, location));
        }
    }
}
