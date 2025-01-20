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

        public double CalculateFees()
        {
            RequestFee = 150;

            double fees = 0;
            //Arriving flight fee
            if (Destination == "SIN")
            {
                fees += 500;
            }

            //Departure flight fee + Boarding gate base fee
            if (Origin == "SIN")
            {
                fees += 800; //departure
                fees += 300; //boarding gate
            }

            return fees + RequestFee;

        }

        public override string ToString()
        {
            return $"{FlightNo}, {Origin}, {Destination}, {ExpectedTime}, {Status}, Request Fee: {RequestFee}";
        }
    }
}
