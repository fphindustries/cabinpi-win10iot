using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Fphi.CabinPi.Common;

using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json;

using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Collections;
using Windows.Storage;

namespace Fphi.CabinPi.Background
{
    public sealed class AppServiceTask : IBackgroundTask
    {
        private BackgroundTaskDeferral _deferral;
        private Guid _connectionGuid;
        private AppServiceConnection _connection;
        private static readonly Dictionary<Guid, AppServiceConnection> Connections = new Dictionary<Guid, AppServiceConnection>();

        //https://www.hackster.io/falafel-software/winiot-inter-application-communication-using-app-services-76af1b
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            Debug.WriteLine("***AppService Run***");
            //keep this background task alive
            _deferral = taskInstance.GetDeferral();
            taskInstance.Canceled += OnTaskCanceled;

            //execution triggered by another application requesting this App Service
            //assigns an event handler to fire when a message is received from the client
            var triggerDetails = taskInstance.TriggerDetails as AppServiceTriggerDetails;
            // save a unique identifier for each connection
            _connectionGuid = Guid.NewGuid();
            _connection = triggerDetails?.AppServiceConnection;
            if (_connection == null)
            {
                return;
            }
            // save the guid and connection in a *static* list of all connections
            Connections.Add(_connectionGuid, _connection);

            // listen for incoming app service requests
            _connection.RequestReceived += RequestReceived;
            _connection.ServiceClosed += ConnectionOnServiceClosed;
        }

        private void ConnectionOnServiceClosed(AppServiceConnection sender, AppServiceClosedEventArgs args)
        {
            Debug.WriteLine("Connection closed: " + _connectionGuid);
            RemoveConnection(_connectionGuid);
        }

        private async void RequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            var configurationService = StartupTask.ServiceProvider.GetService<ConfigurationService>();

            foreach (var messageKey in args.Request.Message.Keys)
            {
                Debug.WriteLine($"Background Task RequestReceived: {messageKey}");

                switch (messageKey)
                {
                    case AppServiceMessages.RequestConfiguration:
                        //Send current configuration
                        ValueSet message = new ValueSet();
                        message.Add(AppServiceMessages.Configuration, JsonConvert.SerializeObject(configurationService.BackgroundConfiguration));
                        await BroadcastMessage(message);
                        break;
                    case AppServiceMessages.Configuration:
                        var config = JsonConvert.DeserializeObject<BackgroundConfiguration>(args.Request.Message[messageKey].ToString());
                        await configurationService.SetConfiguration(config);
                        break;
                    default:
                        break;
                }
            }

        }

        private void OnTaskCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            RemoveConnection(_connectionGuid);
            if (_deferral != null)
            {
                _deferral.Complete();
                _deferral = null;
            }
        }

        private static void RemoveConnection(Guid key)
        {
            var connection = Connections[key];
            connection.Dispose();
            Connections.Remove(key);
        }

        internal static async Task BroadcastMessage(ValueSet valueSet)
        {
            foreach(var connection in Connections)
            {
                await SendMessage(connection, valueSet);
            }
        }

        private static async Task SendMessage(KeyValuePair<Guid, AppServiceConnection> connection, ValueSet valueSet)
        {
            try
            {
                var result = await connection.Value.SendMessageAsync(valueSet);
                if (result.Status == AppServiceResponseStatus.Success)
                {
                    Debug.WriteLine("Successfully sent message to " + connection.Key + ". Result = " + result.Message);
                    return;
                }
                if (result.Status == AppServiceResponseStatus.Failure)
                {
                    // When an app with an open connection is terminated and it fails
                    //      to dispose of its connection, the connection object remains
                    //      in Connections.  When someone tries to send to it, it gets
                    //      an AppServiceResponseStatus.Failure response
                    Debug.WriteLine("Error sending to " + connection.Key + ".  Removing it from the list of active connections.");
                    RemoveConnection(connection.Key);
                    return;
                }
                Debug.WriteLine("Error sending to " + connection.Key + " - " + result.Status);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error SendMessage to " + connection.Key + " " + ex);
            }
        }
    }
}
