using System;
using System.Collections.Generic;
using System.Text;

namespace Fphi.Cabin.Pi.UI2.Core.Services
{
    public interface ISettings
    {
        string GetWeatherAPIString();
        double GetLatitude();
        double GetLongitude();

    }
}
