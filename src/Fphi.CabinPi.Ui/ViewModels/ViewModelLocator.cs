using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fphi.Cabin.Pi.Common.Services;
using Fphi.CabinPi.Common.Services;
using Fphi.CabinPi.Ui.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Fphi.CabinPi.Ui.ViewModels
{
    /// <summary>
    /// This class contains a reference to all the ViewModels in the application
    /// and is the base reference for all binding.
    /// The class is set as an application resource in app.xaml.
    /// </summary>
    public class ViewModelLocator
    {
        private static ViewModelLocator _current;

        public static ViewModelLocator Current => _current ?? (_current = new ViewModelLocator());

        private IServiceCollection _serviceCollection;
        private readonly ServiceProvider _serviceProvider;

        public ViewModelLocator()
        {
            //Set up DI
            _serviceCollection = new ServiceCollection();
            ConfigureServices(_serviceCollection);
            _serviceProvider = _serviceCollection.BuildServiceProvider();

        }

        public ServiceProvider ServiceProvider => _serviceProvider;

        private void ConfigureServices(IServiceCollection serviceCollection)
        {
            //TODO: Verify that these are scoped appropriately
            serviceCollection.AddSingleton<CameraViewModel>();
            serviceCollection.AddSingleton<ImageGalleryDetailViewModel>();
            serviceCollection.AddSingleton<ImageGalleryViewModel>();
            serviceCollection.AddSingleton<MainViewModel>();
            serviceCollection.AddSingleton<SettingsViewModel>();
            serviceCollection.AddSingleton<MapViewModel>();

            serviceCollection.AddSingleton<ISensorService, SensorService>();
            serviceCollection.AddSingleton<ThemeSelectorService>();

            //TODO: for design time editing, need to extract an interface
            serviceCollection.AddSingleton<DataService>();
            serviceCollection.AddSingleton<ISettings, SettingsService>();
            serviceCollection.AddSingleton<IWeatherService, DarkSkyService>();
            serviceCollection.AddSingleton<ITemperatureDescriber, TemperatureDescriber>();
            serviceCollection.AddSingleton<LocationService>();
        }

        //The below properties are what each page actually bind to to get their VMs
        public CameraViewModel Camera => _serviceProvider.GetService<CameraViewModel>();
        public ImageGalleryDetailViewModel ImageGalleryDetail => _serviceProvider.GetService<ImageGalleryDetailViewModel>();
        public ImageGalleryViewModel ImageGallery => _serviceProvider.GetService<ImageGalleryViewModel>();
        public MainViewModel Main => _serviceProvider.GetService<MainViewModel>();
        public SettingsViewModel Settings => _serviceProvider.GetService<SettingsViewModel>();
        public MapViewModel Map => _serviceProvider.GetService<MapViewModel>();

        //General Services
        public ISensorService SensorService => _serviceProvider.GetService<ISensorService>();
        public ThemeSelectorService ThemeSelectorService => _serviceProvider.GetService<ThemeSelectorService>();
        public ISettings SettingsService => _serviceProvider.GetService<ISettings>();
        public IWeatherService WeatherService => _serviceProvider.GetService<IWeatherService>();
        public ITemperatureDescriber TemperatureDescriber => _serviceProvider.GetService<ITemperatureDescriber>();

    }
}
