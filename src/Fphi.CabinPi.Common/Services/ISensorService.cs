using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fphi.CabinPi.Common.Services
{
    public interface ISensorService
    {
        double GetReading(SensorType type);
        Task SetupAppServiceAsync();
        Task SendConfigurationAsync();
        Task RequestConfigurationAsync();

    }
}
