using Fphi.Cabin.Pi.Common.Models;
using Fphi.CabinPi.Common.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fphi.CabinPi.Common.Services
{
    public class SensorReadingReceivedEventArgs : EventArgs
    {
        private SensorReading _reading;

        public SensorReadingReceivedEventArgs(SensorReading reading)
        {
            _reading = reading;
        }

        public SensorReading Reading
        {
            get { return _reading; }
        }

    }

    public interface ISensorService
    {
        event EventHandler<SensorReadingReceivedEventArgs> SensorReadingReceived;
        double GetReading(SensorType type);
        Task SetupAppServiceAsync();
        Task SendConfigurationAsync();
        Task RequestConfigurationAsync();
        ObservableCollection<SensorConfiguration> SensorConfigurations { get; set; }
    }
}
