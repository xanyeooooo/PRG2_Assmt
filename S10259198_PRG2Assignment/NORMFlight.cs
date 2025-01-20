using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S10259198_PRG2Assignment
{
    internal class NORMFlight : Flight
    {
        public NORMFlight(string fno, string o, string dest, DateTime expecttime, string stat) : base(fno, o, dest, expecttime, stat)
        {

        }

        public double CalculateFees()
        {
            return 0;
        }

        public override string ToString()
        {
            return $"{FlightNo}, {Origin}, {Destination}, {ExpectedTime}, {Status}";
        }
    }
}
