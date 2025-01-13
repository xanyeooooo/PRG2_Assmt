using S10259198_PRG2Assignment;
using System;
using System.Collections.Generic;
using System.IO;

Terminal terminal = new Terminal("Terminal 5");
Dictionary<string, Airline> Airlines = terminal.Airlines;
Dictionary<string, BoardingGate> BoardingGates = terminal.BoardingGates;
Dictionary<string, Flight> Flights = terminal.Flights;

// Loading the airlines.csv file
using (StreamReader sr = new StreamReader("airlines.csv", false))
{
    string? s;
    while ((s = sr.ReadLine()) != null)
    {
        string[] data = s.Split(',');
        Airline a = new Airline(data[0], data[1]);
        terminal.Airlines.Add(data[1], a);
    }
}

// Loading the boardinggates.csv file
using (StreamReader sr = new StreamReader("boardinggates.csv", false))
{
    string? s;
    s = sr.ReadLine(); // Read and skip the header line
    while ((s = sr.ReadLine()) != null)
    {
        string[] data = s.Split(',');
        bool ddjbstatus = bool.Parse(data[1]);
        bool cfftstatus = bool.Parse(data[2]);
        bool lwttstatus = bool.Parse(data[3]);
        BoardingGate b = new BoardingGate(data[0], cfftstatus, ddjbstatus, lwttstatus, null);
        terminal.BoardingGates.Add(data[0], b);
    }
}

// Loading flights.csv
using (StreamReader sr = new StreamReader("flights.csv", false))
{
    string? s;
    while ((s = sr.ReadLine()) != null)
    {
        string[] data = s.Split(',');
        DateTime expectedtime = DateTime.Parse(data[3]);
        Flight f = new Flight(data[0], data[1], data[2], expectedtime, data[4]);
        terminal.Flights.Add(data[0], f);
    }
}

// Displaying basic info
foreach (var flight in terminal.Flights.Values)
{
    string airlineName = "Unknown Airline"; // Declare and initialize airlineName here

    foreach (var airline in terminal.Airlines.Values)
    {
        if (airline.Flights.ContainsKey(flight.FlightNo))
        {
            airlineName = airline.Name;
            break;
        }
    }
    Console.WriteLine($"Flight Number: {flight.FlightNo}, Airline Name: {airlineName}, Origin: {flight.Origin}, Destination: {flight.Destination}, Expected Time: {flight.ExpectedTime}");
}