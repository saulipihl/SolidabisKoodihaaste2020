using System.Collections.Generic;

namespace Koodihaaste2020Backend.Models.JsonBusDataObjects
{
    public class JsonBusStopLocation
    {
        public Dictionary<string, Point2D> Locations { get; set; }
    }
}