using Fphi.Cabin.Pi.Common.Services;
using Fphi.CabinPi.Common.Helpers;
using Fphi.CabinPi.Ui.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fphi.CabinPi.Ui.Services
{
    public class SettingsService : Observable, ISettings
    {
        private bool _hasInstanceBeenInitialized = false;

        public async Task InitializeAsync()
        {
            if (!_hasInstanceBeenInitialized)
            {
                DarkSkyApiKey =
                    await Windows.Storage.ApplicationData.Current.LocalSettings.ReadAsync<string>(nameof(DarkSkyApiKey));

                Latitude =
                    await Windows.Storage.ApplicationData.Current.LocalSettings.ReadAsync<double>(nameof(Latitude));

                Longitude =
                    await Windows.Storage.ApplicationData.Current.LocalSettings.ReadAsync<double>(nameof(Longitude));


                _hasInstanceBeenInitialized = true;
            }
        }


        private string _darkSkyApiKey;

        public string DarkSkyApiKey
        {
            get { return _darkSkyApiKey; }
            set
            {
                if (value != _darkSkyApiKey)
                {
                    Task.Run(async () => await Windows.Storage.ApplicationData.Current.LocalSettings.SaveAsync(nameof(DarkSkyApiKey), value ?? string.Empty));

                }
                Set(ref _darkSkyApiKey, value);
            }
        }

        private double _latitude;

        public double Latitude
        {
            get { return _latitude; }
            set
            {
                if (value != _latitude)
                {
                    Task.Run(async () => await Windows.Storage.ApplicationData.Current.LocalSettings.SaveAsync(nameof(Latitude), value));

                }
                Set(ref _latitude, value);
            }
        }

        private double _longitude;

        public double Longitude
        {
            get { return _longitude; }
            set
            {
                if (value != _longitude)
                {
                    Task.Run(async () => await Windows.Storage.ApplicationData.Current.LocalSettings.SaveAsync(nameof(Longitude), value));

                }
                Set(ref _longitude, value);
            }
        }

    }
}
