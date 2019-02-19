using Fphi.CabinPi.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fphi.CabinPi.Background
{
    interface ISensor
    {
        string Name { get; set; }
        Task<IEnumerable<SensorReading>> GetReadingsAsync();

        Task InitializeAsync();
    }
}
