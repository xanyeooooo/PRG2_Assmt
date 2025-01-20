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
        public Dictionary<string, Flight> Flights { get; set; }

        public Airline(string n, string c)
        {
            Name = n;
            Code = c;
            Flights = new Dictionary<string, Flight>();
        }

        public bool AddFlight(Flight flight)
        {
            if (Flights.ContainsKey(flight.FlightNo))
            {
                return false;
            }
            Flights.Add(flight.FlightNo, flight);
            return true;
        }

        public bool RemoveFlight(Flight flight)
        {
            if (Flights.ContainsKey(flight.FlightNo))
            {
                Flights.Remove(flight.FlightNo);
                return true;
            }
            return false;
        }

        public double CalculateFees()
        {
            double totalFees = 0;
            double discount = 0;
            int flightCount = 0;
            int earlyLateFlightCount = 0;
            int customOriginCount = 0;
            int noSpecialRequestCodeCount = 0;

            foreach (var flight in Flights.Values)
            {
                totalFees += flight.CalculateFees();
                flightCount++;

                if (flight.ExpectedTime.Hour < 11 || flight.ExpectedTime.Hour > 21)
                {
                    earlyLateFlightCount++;
                }

                if (flight.Origin == "DXB" || flight.Origin == "BKK" || flight.Origin == "NRT")
                {
                    customOriginCount++;
                }

                if (string.IsNullOrEmpty(flight.Status))
                {
                    noSpecialRequestCodeCount++;
                }
            }

            // For more than 5 flights arriving/departing, airlines receive an additional discount of 3% off the Total Bill
            if (flightCount > 5)
            {
                discount += totalFees * 0.03;
            }

            // For every 3 flights arriving/departing, airlines will receive a discount of $350
            discount += (flightCount / 3) * 350;

            // For flights arriving/departing before 11am or after 9pm, airlines receive a discount of $110
            discount += earlyLateFlightCount * 110;

            // For airlines with the Origin of Dubai (DXB), Bangkok (BKK) or Tokyo (NRT), airlines receive a discount of $25
            discount += customOriginCount * 25;

            // For not indicating any Special Request Codes, airlines receive a discount of $50
            discount += noSpecialRequestCodeCount * 50;

            return totalFees - discount;
        }

        public override string ToString()
        {
            return Name + Code;
        }
    }
}
