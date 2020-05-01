using Koodihaaste2020Backend.Constants;
using System.Runtime.Serialization;

namespace Koodihaaste2020Backend.Models
{
    [DataContract]
    public class Point2D
    {
        public Point2D(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
        [DataMember]
        public float x { get; set; }
        [DataMember]
        public float y { get; set; }
    }
}