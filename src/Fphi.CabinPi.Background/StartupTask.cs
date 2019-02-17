using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Fphi.CabinPi.Background.Fakes;
using Fphi.CabinPi.Background.Sensors;
using InfluxDB.Collector;
using Microsoft.Extensions.DependencyInjection;

using Windows.ApplicationModel.Background;
using Windows.System.Threading;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace Fphi.CabinPi.Background
{
    public sealed class StartupTask : IBackgroundTask
    {
        private BackgroundTaskDeferral _deferal;
        private IBackgroundTaskInstance _taskInstance;
        private static ServiceProvider _serviceProvider;
        private SensorReader _sensorReader;
        private ThreadPoolTimer _periodicTimer;
        private volatile bool _cancelRequested = false;

        private IServiceCollection _serviceCollection;

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            // 
            // TODO: Insert code to perform background work
            //
            // If you start any asynchronous methods here, prevent the task
            // from closing prematurely by using BackgroundTaskDeferral as
            // described in http://aka.ms/backgroundtaskdeferral
            //

            Debug.WriteLine($"{taskInstance.Task.Name} Starting");


            taskInstance.Canceled += TaskInstance_Canceled;
            _deferal = taskInstance.GetDeferral();
            _taskInstance = taskInstance;

            //Set up DI
            _serviceCollection = new ServiceCollection();
            ConfigureServices(_serviceCollection);

            _serviceProvider = _serviceCollection.BuildServiceProvider();

            var configuration = _serviceProvider.GetService<ConfigurationService>();
            Task.Run(configuration.InitAsync).Wait();

            _sensorReader = _serviceProvider.GetService<SensorReader>();

            Metrics.Collector = new CollectorConfiguration()
                .Tag.With("host", Environment.GetEnvironmentVariable("COMPUTERNAME"))
                .Batch.AtInterval(TimeSpan.FromSeconds(2))
                .WriteTo.InfluxDB("http://cabinpi.fphi.us:8086", "data")
                .CreateCollector();

            _periodicTimer = ThreadPoolTimer.CreatePeriodicTimer(OnTimer, TimeSpan.FromSeconds(5));
        }

        private void ConfigureServices(IServiceCollection serviceCollection)
        {
            //For now, to read from real sensors, switch to the SensorFactory
            //serviceCollection.AddSingleton<ISensorFactory, SensorFactory>();
            serviceCollection.AddSingleton<ConfigurationService>();
            serviceCollection.AddSingleton<ISensorFactory, FakeSensorFactory>();

            serviceCollection.AddSingleton<ISensorDataStore, InMemoryDataStore>();
            serviceCollection.AddSingleton<SensorReader>();
        }

        private void OnTimer(ThreadPoolTimer timer)
        {
            if (_cancelRequested)
            {
                timer.Cancel();
            }
            else
            {

                Debug.WriteLine("OnTimer");
                //Here we read all of the configured sensors, add them to the in-memory store, notify the UI that we have new data, and write it out to the remote influxdb
                _sensorReader.ReadSensors().Wait();

                var bundles = _sensorReader.GetAllReadings();
                foreach (var bundle in bundles.Where(b => b.Any()))
                {
                    var sensorName = bundle.First().Sensor.ToString();
                    var fields = bundle.ToDictionary(
                        reading => reading.Type.ToString(),
                        reading => reading.Value as object);

                    fields.Add("Name", sensorName);
                    Metrics.Write("sensors", fields);
                }
            }
        }

        private void TaskInstance_Canceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            _cancelRequested = true;
            Debug.WriteLine($"{sender.Task.Name} Cancel Requested");
        }

        internal static ServiceProvider ServiceProvider { get => _serviceProvider; }
    }
}
