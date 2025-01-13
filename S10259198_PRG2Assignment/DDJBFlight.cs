using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S10259198_PRG2Assignment
{
    internal class DDJBFlight : Flight
    {
        public double RequestFee { get; set; }
        public DDJBFlight(string fno, string o, string dest, DateTime expecttime, string stat, double rf) : base(fno, o, dest, expecttime, stat)
        {
            RequestFee = rf;
        }

        public override double CalculateFees()
        {
            return 0;
        }

        public override string ToString()
        {
            return $"{FlightNo}, {Origin}, {Destination}, {ExpectedTime}, {Status}, Request Fee: {RequestFee}";
        }
    }
}
