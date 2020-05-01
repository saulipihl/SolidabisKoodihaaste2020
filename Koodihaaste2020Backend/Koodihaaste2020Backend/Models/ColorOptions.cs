using Koodihaaste2020Backend.Constants;
using System.Runtime.Serialization;

namespace Koodihaaste2020Backend.Models
{
    [DataContract]
    public class ColorInformation
    {
        public ColorInformation(string lineColor)
        {
            Name = lineColor;
            ColorCode = DetermineColorCode(lineColor);
        }

        private string DetermineColorCode(string lineColor)
        {
            switch (lineColor.ToUpper())
            {
                case (ColorOptions.KELTAINEN):
                    return "#EDDB09";
                case (ColorOptions.PUNAINEN):
                    return "#ED0909";
                case (ColorOptions.VIHREÄ):
                    return "#20C00F";
                case (ColorOptions.SININEN):
                    return "#0C0EE3";
                default:
                    return "#000000";
            }
        }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string ColorCode { get; set; }
    }
}