using Fphi.Cabin.Pi.UI2.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fphi.Cabin.Pi2.UI.Helpers
{
    public class BackgroundConfiguration
    {
        public List<SensorConfiguration> Sensors { get; set; }

        public BackgroundConfiguration()
        {
            Sensors = new List<SensorConfiguration>();
        }
    }
}
