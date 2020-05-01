using Koodihaaste2020Backend.Models;
using System.Collections.Generic;

namespace Koodihaaste2020Backend.Interfaces
{
    interface IBusDataRepository
    {
        List<BusLine> GetBusLines();
        List<BusRoute> GetBusRoutes();
        List<BusStop> GetBusStops();
    }
}
