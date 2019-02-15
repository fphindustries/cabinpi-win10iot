using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Fphi.Cabin.Pi.Common.Services
{
    public interface ISettings
    {
        string DarkSkyApiKey { get; set; }
        double Latitude { get; set; }
        double Longitude { get; set; }
        Task InitializeAsync();

    }
}
