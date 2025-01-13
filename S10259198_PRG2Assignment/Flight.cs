using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S10259198_PRG2Assignment
{
    public abstract class Flight
    {
        public string FlightNo { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public DateTime ExpectedTime { get; set; }
        public string Status { get; set; }

        public Flight (string fno,string o,string dest, DateTime expecttime, string stat)
        {
            FlightNo = fno;
            Origin = o;
            Destination = dest;
            ExpectedTime = expecttime;
            Status = stat;
        }

        public abstract double CalculateFees();

        public override string ToString()
        {
            return FlightNo + Origin + Destination + ExpectedTime + Status;
        }
    }
}
