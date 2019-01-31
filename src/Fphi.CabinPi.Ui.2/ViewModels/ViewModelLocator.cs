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
            serviceCollection.AddSingleton<PivotViewModel>();
            serviceCollection.AddSingleton<SettingsViewModel>();

            serviceCollection.AddSingleton<AppService>();
            serviceCollection.AddSingleton<ThemeSelectorService>();
        }

        //The below properties are what each page actually bind to to get their VMs
        public CameraViewModel Camera => _serviceProvider.GetService<CameraViewModel>();
        public ImageGalleryDetailViewModel ImageGalleryDetail => _serviceProvider.GetService<ImageGalleryDetailViewModel>();
        public ImageGalleryViewModel ImageGallery => _serviceProvider.GetService<ImageGalleryViewModel>();
        public MainViewModel Main => _serviceProvider.GetService<MainViewModel>();
        public PivotViewModel Pivot => _serviceProvider.GetService<PivotViewModel>();
        public SettingsViewModel Settings => _serviceProvider.GetService<SettingsViewModel>();

        //General Services
        public AppService AppService => _serviceProvider.GetService<AppService>();
        public ThemeSelectorService ThemeSelectorService => _serviceProvider.GetService<ThemeSelectorService>();

    }
}
