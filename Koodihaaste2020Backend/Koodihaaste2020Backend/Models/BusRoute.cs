using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Koodihaaste2020Backend.Models
{
    [DataContract]
    public class BusRoute
    {
        public BusRoute(JsonBusRoute jsonRoute)
        {
            BusStop1 = new BusStop(jsonRoute.Mista);
            BusStop2 = new BusStop(jsonRoute.Mihin);
            Duration = jsonRoute.Kesto;
        }

        [DataMember]
        public BusStop BusStop1 { get; set; }
        [DataMember]
        public BusStop BusStop2 { get; set; }
        [DataMember]
        public int Duration { get; set; }

        public override bool Equals(object obj)
        {
            var route = obj as BusRoute;
            return route != null &&
                   EqualityComparer<BusStop>.Default.Equals(BusStop1, route.BusStop1) &&
                   EqualityComparer<BusStop>.Default.Equals(BusStop2, route.BusStop2);
        }

        public override int GetHashCode()
        {
            var hashCode = -1233567671;
            hashCode = hashCode * -1521134295 + EqualityComparer<BusStop>.Default.GetHashCode(BusStop1);
            hashCode = hashCode * -1521134295 + EqualityComparer<BusStop>.Default.GetHashCode(BusStop2);
            return hashCode;
        }

        //public override int GetHashCode()
        //{
        //    return this.BusStop1.Name;
        //}

        //public override bool Equals(object other)
        //{
        //    if (other is BusRoute)
        //        return ((BusRoute)other).BusStop1.Name == this.BusStop1.Name 
        //            && ((BusRoute)other).BusStop2.Name == this.BusStop2.Name;
        //    return false;
        //}
    }
}