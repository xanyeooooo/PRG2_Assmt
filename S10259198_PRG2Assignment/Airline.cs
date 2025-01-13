using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S10259198_PRG2Assignment
{
    internal class Airline
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public Dictionary <string,Flight> Flights { get; set; }
        
        public Airline (string n, string c)
        {
            Flights = new Dictionary<string, Flight>();
        }
    }
}
