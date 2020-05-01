using Koodihaaste2020Backend.Interfaces;
using Koodihaaste2020Backend.Models;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Configuration;
using Newtonsoft.Json;
using Koodihaaste2020Backend.Models.JsonBusDataObjects;

namespace Koodihaaste2020Backend.Services
{
    /// <summary>
    /// Handles bus data loading and can be used to fetch bus data
    /// </summary>
    public class BusDataRespository : IBusDataRepository
    {
        private string dataFilePath;
        private string busStopLocationsFilePath;
        private JsonBusData busDataJson;
        private JsonBusStopLocation busStopLocations;

        public BusDataRespository()
        {
            var dataFileName = ConfigurationManager.AppSettings["BusDataFileName"];
            dataFilePath = Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "App_Data", dataFileName);

            var busStopLocationsFileName = ConfigurationManager.AppSettings["BusStopLocationsFileName"];
            busStopLocationsFilePath = Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "App_Data", busStopLocationsFileName);

            busDataJson = ReadBusData();
            busStopLocations = ReadBusDataLocations();
        }

        public List<BusStop> GetBusStops()
        {
            var stops = new List<BusStop>();
            foreach (var stop in busDataJson.Pysakit)
            {
                var location = busStopLocations.Locations.ContainsKey(stop) 
                    ? busStopLocations.Locations[stop]
                    : new Point2D(-1000, -1000);
                stops.Add(new BusStop(stop, location));
            }
            return stops;
        }

        public List<BusLine> GetBusLines()
        {
            var allRoutes = GetBusRoutes();
            var lines = new List<BusLine>();
            foreach (var line in busDataJson.Linjastot)
            {
                lines.Add(new BusLine(line, allRoutes));
            }
            return lines;
        }

        public List<BusRoute> GetBusRoutes()
        {
            var routes = new List<BusRoute>();
            foreach(var route in busDataJson.Tiet)
            {
                routes.Add(new BusRoute(route));
            }
            return routes;
        }

        private JsonBusData ReadBusData()
        {
            using (StreamReader r = new StreamReader(dataFilePath))
            {
                string json = r.ReadToEnd();
                return JsonConvert.DeserializeObject<JsonBusData>(json);
            }
        }

        private JsonBusStopLocation ReadBusDataLocations()
        {
            using (StreamReader r = new StreamReader(busStopLocationsFilePath))
            {
                string json = r.ReadToEnd();
                return JsonConvert.DeserializeObject<JsonBusStopLocation>(json);
            }
        }
    }
}