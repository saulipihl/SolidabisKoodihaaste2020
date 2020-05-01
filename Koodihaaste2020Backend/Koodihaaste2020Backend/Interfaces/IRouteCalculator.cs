using Koodihaaste2020Backend.Services;
using System.Collections.Generic;

namespace Koodihaaste2020Backend.Interfaces
{
    interface IRouteCalculator
    {
        List<BusLineRoute> CalculateRoute(string fromStopName, string toStopName);
    }
}
