using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Windows.ApplicationModel.Background;
using System.Diagnostics;
using Windows.System.Threading;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace Fphi.CabinPi.Background
{
    public sealed class StartupTask : IBackgroundTask
    {
        private BackgroundTaskDeferral _deferal;
        private IBackgroundTaskInstance _taskInstance;
        private ThreadPoolTimer _periodicTimer;
        private volatile bool _cancelRequested = false;

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

            _periodicTimer = ThreadPoolTimer.CreatePeriodicTimer(new TimerElapsedHandler(OnTimer), TimeSpan.FromSeconds(5));
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
            }
        }

        private void TaskInstance_Canceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            _cancelRequested = true;
            Debug.WriteLine($"{sender.Task.Name} Cancel Requested");
        }
    }
}
