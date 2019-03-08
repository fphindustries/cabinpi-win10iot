using Fphi.Cabin.Pi.Common.Models;
using Fphi.CabinPi.Common.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fphi.CabinPi.Common.Services
{
    public interface IDataService
    {
        Task<IEnumerable<double>> GetHourlyUsageAverages(DateTime effectiveDate);
    }


    public class DataService : Observable, IDataService
    {
        public async Task<IEnumerable<double>> GetHourlyUsageAverages(DateTime effectiveDate)
        {
            return await Task.FromResult<IEnumerable<double>>(new List<double> { 5, 9, 8, 6, 1, 5, 7, 3, 6, 3 });
        }

    }
}
