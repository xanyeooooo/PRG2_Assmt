using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S10259198_PRG2Assignment
{
    internal class CFFTFlight : Flight
    {
        public double RequestFee { get; set; }
        public CFFTFlight(string fno, string o, string dest, DateTime expecttime, string stat, double rf) : base(fno, o, dest, expecttime, stat)
        {
            RequestFee = rf;
        }

        public override double CalculateFees()
        {
            RequestFee = 150;
            return RequestFee;
        }

        public override string ToString()
        {
            return $"{FlightNo}, {Origin}, {Destination}, {ExpectedTime}, {Status}, Request Fee: {RequestFee}";
        }
    }
}
