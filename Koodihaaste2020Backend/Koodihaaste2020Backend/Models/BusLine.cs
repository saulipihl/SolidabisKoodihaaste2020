using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Koodihaaste2020Backend.Models
{
    [DataContract]
    public class BusLine
    {
        public BusLine(KeyValuePair<string, List<string>> line, List<BusRoute> allRoutes)
        {
            var color = line.Key;
            var stops = line.Value;
            var routes = new List<BusRoute>();
            for(int i = 0; i < stops.Count - 1; i++)
            {
                var from = stops[i];
                var to = stops[i + 1];
                var route = allRoutes.FirstOrDefault(r => (r.BusStop1.Name.Equals(from) && r.BusStop2.Name.Equals(to)) 
                                                            || (r.BusStop1.Name.Equals(to) && r.BusStop2.Name.Equals(from)));
                if (route != null) routes.Add(route);
            }

            Color = new ColorInformation(color);
            Routes = routes;
        }

        [DataMember]
        public ColorInformation Color { get; set; }
        [DataMember]
        public List<BusRoute> Routes { get; set; }
    }
}