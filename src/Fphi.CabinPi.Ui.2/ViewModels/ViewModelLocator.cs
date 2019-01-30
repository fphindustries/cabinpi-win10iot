using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fphi.CabinPi.Ui.ViewModels
{
    /// <summary>
    /// This class contains a reference to all the ViewModels in the application
    /// and is the base reference for all binding.
    /// The class is set as an application resource in app.xaml.
    /// </summary>
    public class ViewModelLocator
    {
        private IServiceCollection _serviceCollection;
        private ServiceProvider _serviceProvider;

        public ViewModelLocator()
        {
            //Set up DI
            _serviceCollection = new ServiceCollection();
            ConfigureServices(_serviceCollection);
            _serviceProvider = _serviceCollection.BuildServiceProvider();

        }

        private void ConfigureServices(IServiceCollection serviceCollection)
        {
            //TODO: Verify that these are scoped appropriately
            serviceCollection.AddSingleton<CameraViewModel>();
            serviceCollection.AddSingleton<ImageGalleryDetailViewModel>();
            serviceCollection.AddSingleton<ImageGalleryViewModel>();
            serviceCollection.AddSingleton<MainViewModel>();
            serviceCollection.AddSingleton<PivotViewModel>();
            serviceCollection.AddSingleton<SettingsViewModel>();
        }

        //The blow properties are what each page actually bind to to get their VMs
        public CameraViewModel Camera => _serviceProvider.GetService<CameraViewModel>();
        public ImageGalleryDetailViewModel ImageGalleryDetail => _serviceProvider.GetService<ImageGalleryDetailViewModel>();
        public ImageGalleryViewModel ImageGallery => _serviceProvider.GetService<ImageGalleryViewModel>();
        public MainViewModel Main => _serviceProvider.GetService<MainViewModel>();
        public PivotViewModel Pivot => _serviceProvider.GetService<PivotViewModel>();
        public SettingsViewModel Settings => _serviceProvider.GetService<SettingsViewModel>();

    }
}
