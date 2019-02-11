using Fphi.Cabin.Pi.UI2.Core.Helpers;
using Fphi.Cabin.Pi.UI2.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fphi.CabinPi.Ui.Services
{
    public class DataService : Observable
    {
        private ObservableCollection<SensorConfiguration> _sensorConfigurations = new ObservableCollection<SensorConfiguration>();

        public ObservableCollection<SensorConfiguration> SensorConfigurations
        {
            get { return _sensorConfigurations; }
            set { Set(ref _sensorConfigurations, value); }
        }

    }
}
