using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fphi.CabinPi.Common
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
