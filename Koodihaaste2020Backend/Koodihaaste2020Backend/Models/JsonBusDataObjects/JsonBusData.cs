using System.Collections.Generic;

namespace Koodihaaste2020Backend.Models
{
    public class JsonBusData
    {
        public List<string> Pysakit { get; set; }
        public List<JsonBusRoute> Tiet { get; set; }
        public Dictionary<string, List<string>> Linjastot { get; set; }
    }
}