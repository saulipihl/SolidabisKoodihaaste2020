using Koodihaaste2020Backend.Models;
using System.Runtime.Serialization;

namespace Koodihaaste2020Backend.Services
{
    [DataContract]
    public class BusLineRoute
    {
        public BusLineRoute(ColorInformation color, BusRoute route)
        {
            Color = color;
            Route = route;
        }

        [DataMember]
        public ColorInformation Color { get; set; }
        [DataMember]
        public BusRoute Route { get; set; }
    }
}