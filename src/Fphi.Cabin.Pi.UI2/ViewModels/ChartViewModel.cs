using System;
using System.Collections.ObjectModel;

using Fphi.Cabin.Pi.UI2.Core.Models;
using Fphi.Cabin.Pi.UI2.Core.Services;

using GalaSoft.MvvmLight;

namespace Fphi.Cabin.Pi.UI2.ViewModels
{
    public class ChartViewModel : ViewModelBase
    {
        public ChartViewModel()
        {
        }

        public ObservableCollection<DataPoint> Source
        {
            get
            {
                // TODO WTS: Replace this with your actual data
                return SampleDataService.GetChartSampleData();
            }
        }
    }
}
