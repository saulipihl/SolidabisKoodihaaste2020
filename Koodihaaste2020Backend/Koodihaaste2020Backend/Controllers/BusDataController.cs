using Koodihaaste2020Backend.Interfaces;
using Koodihaaste2020Backend.Models;
using Koodihaaste2020Backend.Services;
using System.Collections.Generic;
using System.Web.Http;

namespace Koodihaaste2020Backend.Controllers
{
    [System.Web.Mvc.RoutePrefix("api")]
    public class BusDataController : ApiController
    {
        private IBusDataRepository busDataRepository;
        private IRouteCalculator routeCalculator;
        public BusDataController()
        {
            busDataRepository = new BusDataRespository();
            routeCalculator = new RouteCalculator();
        }

        [System.Web.Mvc.Route("getbusstops")]
        [HttpGet]
        public List<BusStop> GetBusStops()
        {
            return busDataRepository.GetBusStops();
        }

        [System.Web.Mvc.Route("getbusroutes")]
        [HttpGet]
        public List<BusRoute> GetBusRoutes()
        {
            return busDataRepository.GetBusRoutes();
        }

        [System.Web.Mvc.Route("calculateRoute")]
        [HttpGet]
        public List<BusLineRoute> CalculateRoute(string fromStopName, string toStopName)
        {
            return routeCalculator.CalculateRoute(fromStopName, toStopName);
        }
    }
}
