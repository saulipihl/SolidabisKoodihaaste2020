using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Serialization;

namespace Koodihaaste2020Backend.Models
{
    [DataContract]
    public class BusStop
    {
        public BusStop(string name)
        {
            Name = name;
        }

        public BusStop(string name, Point2D location) : this(name)
        {
            this.location = location;
        }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public Point2D location;

        public override bool Equals(object obj)
        {
            var stop = obj as BusStop;
            return stop != null &&
                   Name == stop.Name;
        }

        public override int GetHashCode()
        {
            return 539060726 + EqualityComparer<string>.Default.GetHashCode(Name);
        }
    }
}