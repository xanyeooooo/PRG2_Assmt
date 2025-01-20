using S10259198_PRG2Assignment;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;

Terminal terminal = new Terminal("Terminal 5");
Dictionary<string, Airline> Airlines = terminal.Airlines;
Dictionary<string, BoardingGate> BoardingGates = terminal.BoardingGates;
Dictionary<string, Flight> Flights = terminal.Flights;

int airlineCount = 0;
int boardingGateCount = 0;
int flightCount = 0;

// Loading the airlines.csv file
Console.WriteLine($"Loading Airlines...");
using (StreamReader sr = new StreamReader("airlines.csv", false))
{
    string? s;
    s = sr.ReadLine(); //skips header
    while ((s = sr.ReadLine()) != null)
    {
        string[] data = s.Split(',');
        Airline a = new Airline(data[0], data[1]);
        terminal.Airlines.Add(data[0], a);
        airlineCount++;
    }
}
Console.WriteLine($"{airlineCount} Airlines Loaded!");

// Loading the boardinggates.csv file
Console.WriteLine("Loading Boarding Gates...");
using (StreamReader sr = new StreamReader("boardinggates.csv", false))
{
    string? s;
    s = sr.ReadLine(); // skips header
    while ((s = sr.ReadLine()) != null)
    {
        string[] data = s.Split(',');
        bool ddjbstatus = bool.Parse(data[1]);
        bool cfftstatus = bool.Parse(data[2]);
        bool lwttstatus = bool.Parse(data[3]);
        BoardingGate b = new BoardingGate(data[0], cfftstatus, ddjbstatus, lwttstatus, null);
        terminal.BoardingGates.Add(data[0], b);
        boardingGateCount++;
    }
}
Console.WriteLine($"{boardingGateCount} Boarding Gates Loaded!");

// Loading flights.csv
Console.WriteLine("Loading Flights...");
using (StreamReader sr = new StreamReader("flights.csv", false))
{
    string? s;
    s = sr.ReadLine(); //skips the header
    while ((s = sr.ReadLine()) != null)
    {
        string[] data = s.Split(',');
        DateTime expectedtime = DateTime.Parse(data[3]);
        Flight f = null;
        string status = data[4];
        double requestFee = 0;

        if (status == "DDJB")
        {
            requestFee = 300;
            f = new DDJBFlight(data[0], data[1], data[2], expectedtime, status, requestFee);
            //flight. no, origin, destination
        }
        else if (status == "CFFT")
        {
            requestFee = 150;
            f = new CFFTFlight(data[0], data[1], data[2], expectedtime, status, requestFee);
        }
        else if (status == "LWTT")
        {
            requestFee = 500;
            f = new LWTTFlight(data[0], data[1], data[2], expectedtime, status, requestFee);
        }
        else if (status == "")
        {
            requestFee = 0;
            f = new NORMFlight(data[0], data[1], data[2], expectedtime, status);
        }

        terminal.Flights.Add(data[0], f);
        flightCount++;
    }
}
Console.WriteLine($"{flightCount} Flights Loaded!");

Console.WriteLine(""); //separation line

// Main menu
while (true)
{
    try
    {
        Console.WriteLine(" ");
        Console.WriteLine("==============================================");
        Console.WriteLine("Welcome to Changi Airport Terminal 5");
        Console.WriteLine("==============================================");
        Console.WriteLine("1. List All Flights");
        Console.WriteLine("2. List Boarding Gates");
        Console.WriteLine("3. Assign a Boarding Gate to a Flight");
        Console.WriteLine("4. Create Flight");
        Console.WriteLine("5. Display Airline Flights");
        Console.WriteLine("6. Modify Flight Details");
        Console.WriteLine("7. Display Flight Schedule");
        Console.WriteLine("0. Exit");
        Console.WriteLine("");
        Console.Write("Please select your option:");

        int option = int.Parse(Console.ReadLine());

        if (option == 1)
        {
            // Displaying basic info
            Console.WriteLine(" ");
            Console.WriteLine("=============================================");
            Console.WriteLine("List of Flights for Changi Airport Terminal 5");
            Console.WriteLine("=============================================");
            Console.WriteLine("Flight Number   Airline Name         Origin               Destination          Expected Departure/Arrival Time");

            foreach (var flight in terminal.Flights.Values)
            {
                string airlineName = "";

                string airlineCode = flight.FlightNo.Split(' ')[0];

                foreach (var airline in terminal.Airlines.Values)
                {
                    if (airline.Code == airlineCode)
                    {
                        airlineName = airline.Name;
                        break;
                    }
                }

                Console.WriteLine($"{flight.FlightNo,-15} {airlineName,-20} {flight.Origin,-20} {flight.Destination,-20} {flight.ExpectedTime:dd/MM/yyyy hh:mm tt}");
            }
            Console.WriteLine(" "); //separation line
        }
        else if (option == 2)
        {
            Console.WriteLine("=============================================");
            Console.WriteLine("List of Boarding Gates for Changi Airport Terminal 5");
            Console.WriteLine("=============================================");
            Console.WriteLine("Gate Name       DDJB                   CFFT                   LWTT");

            foreach (var gate in terminal.BoardingGates.Values)
            {
                Console.WriteLine($"{gate.GateName,-15} {gate.SupportsDDJB,-22} {gate.SupportsCFFT,-22} {gate.SupportsLWTT,-22}");
            }
            Console.WriteLine(" "); //separation line
        }

        else if (option == 3)
        {
            Console.WriteLine("=============================================");
            Console.WriteLine("Assign a Boarding Gate to a Flight");
            Console.WriteLine("=============================================");
            Console.Write("Enter Flight Number: ");
            string flightNo = Console.ReadLine().ToUpper();

            if (!Flights.ContainsKey(flightNo))
            {
                Console.WriteLine("Flight not found.");
                continue;
            }

            string gateName;
            while (true)
            {
                Console.Write("Enter Boarding Gate Name: ");
                gateName = Console.ReadLine().ToUpper();

                if (!BoardingGates.ContainsKey(gateName))
                {
                    Console.WriteLine("The Boarding Gate does not exist.");
                }
                else if (BoardingGates[gateName].Flight != null)
                    Console.WriteLine("The Boarding Gate is already assigned to another flight.");
                else
                {
                    BoardingGates[gateName].Flight = Flights[flightNo];
                    break;
                }
            }

            //Retrieving details
            Flight flight = Flights[flightNo];
            BoardingGate gate = BoardingGates[gateName];

            //Displaying details
            Console.WriteLine($"Flight Number: {flight.FlightNo}");
            Console.WriteLine($"Origin: {flight.Origin}");
            Console.WriteLine($"Destination: {flight.Destination}");
            Console.WriteLine($"Expected Time: {flight.ExpectedTime}");
            Console.WriteLine($"Special Request Code: {flight.Status}");
            Console.WriteLine($"Supports DDJB: {gate.SupportsDDJB}");
            Console.WriteLine($"Supports CFFT: {gate.SupportsCFFT}");
            Console.WriteLine($"Supports LWTT: {gate.SupportsLWTT}");

            //Prompt user if want to update status
            Console.Write("Would you like to update the status of the flight? (Y/N): ");
            string updateStatus = Console.ReadLine().ToUpper();

            if (updateStatus == "Y")
            {
                Console.WriteLine("1. Delayed");
                Console.WriteLine("2. Boarding");
                Console.WriteLine("3. On Time");
                Console.Write("Please select the new status of the flight: ");
                string statusOption = Console.ReadLine();

                if (statusOption == "1")
                {
                    flight.Status = "Delayed";
                }
                else if (statusOption == "2")
                {
                    flight.Status = "Boarding";
                }
                else if (statusOption == "3")
                {
                    flight.Status = "On Time";
                }
                else
                {
                    Console.WriteLine("Invalid option. Status not updated.");
                }
            }

            Console.WriteLine($"Flight {flight.FlightNo} has been assigned to Boarding Gate {gate.GateName}!");
        }




        else if (option == 6)
        {
            while (true)
            {
                try
                {
                    Console.WriteLine("==============================================");
                    Console.WriteLine("List of Airlines for Changi Airport Terminal 5");
                    Console.WriteLine("==============================================");
                    Console.WriteLine($"{"Airline Code",-13}{"Airline Name",-19}");
                    // Listing all the airlines available
                    foreach (var airline in terminal.Airlines.Values)
                    {
                        Console.WriteLine($"{airline.Code,-13}{airline.Name,-19}");
                    }
                    Console.WriteLine(" "); // separation line

                    // Prompting user for airline code
                    Console.Write("Enter the 2-Letter Airline Code (e.g. SQ or MH, etc.): ");
                    string airlineCode = Console.ReadLine().ToUpper();



                    // Validate the input
                    if (string.IsNullOrEmpty(airlineCode) || airlineCode.Length != 2)
                    {
                        throw new ArgumentException("Invalid Airline Code. Please enter a valid 2-letter code.");
                    }



                    // Retrieve the selected airline
                    Airline selectedAirline = null;
                    foreach (var airline in terminal.Airlines.Values)
                    {
                        if (airline.Code == airlineCode)
                        {
                            selectedAirline = airline;
                            break;
                        }
                    }



                    if (selectedAirline != null) //if there is an airline
                    {
                        while (true)
                        {
                            // Display flights for the selected airline
                            Console.WriteLine("=============================================");
                            Console.WriteLine($"List of Flights for: {selectedAirline.Name}");
                            Console.WriteLine("=============================================");
                            Console.WriteLine($"{"Flight Number",-15}{"Airline Name",-20}{"Origin",-20}{"Destination",-20}{"Expected Departure/Arrival Time",-20}");

                            foreach (var flight in terminal.Flights.Values)
                            {
                                if (flight.FlightNo.StartsWith(airlineCode))
                                {
                                    Console.WriteLine($"{flight.FlightNo,-15}{selectedAirline.Name,-20}{flight.Origin,-20}{flight.Destination,-20}{flight.ExpectedTime,-20}");
                                }
                            }
                            Console.WriteLine(" "); // separation line


                            //Prompt user input for modify or delete
                            Console.Write("Choose an existing Flight to modify or delete: ");
                            string flightNumber = Console.ReadLine().ToUpper();

                            //Retrieve the Flight
                            Flight selectedFlight = null;
                            foreach (var flight in terminal.Flights.Values)
                            {
                                if (flight.FlightNo == flightNumber)
                                {
                                    selectedFlight = flight;
                                    break;
                                }
                            }

                            if (selectedFlight != null)
                            {
                                Console.WriteLine("1. Modify Flight");
                                Console.WriteLine("2. Delete Flight");
                                Console.Write("Choose an option:");
                                string modifyorDelete = Console.ReadLine();

                                if (modifyorDelete == "1")
                                {
                                    Console.WriteLine("1. Modify Basic Information");
                                    Console.WriteLine("2. Modify Status");
                                    Console.WriteLine("3. Modify Special Request Code");
                                    Console.WriteLine("4. Modify Boarding Gate");
                                    Console.Write("Choose an option: ");
                                    string opt = Console.ReadLine();

                                    if (opt == "1")
                                    {
                                        //New origin
                                        Console.Write("Enter new Origin:");
                                        string newOrigin = Console.ReadLine();

                                        if (string.IsNullOrEmpty(newOrigin))
                                        {
                                            Console.WriteLine("Invalid origin. Please try again.");
                                            continue;
                                        }
                                        selectedFlight.Origin = newOrigin;

                                        //New destination
                                        Console.Write("Enter new Destination: ");
                                        string newDestination = Console.ReadLine();
                                        if (string.IsNullOrEmpty(newDestination))
                                        {
                                            Console.WriteLine("Invalid destination. Please try again");
                                            continue;
                                        }
                                        selectedFlight.Destination = newDestination;

                                        //New arrival / departure time
                                        Console.Write("Enter new Expected Departure / Arrival Time (dd/mm/yyyy hh:mm): ");
                                        string newExpectedTime = Console.ReadLine();

                                        if (!DateTime.TryParse(newExpectedTime, out DateTime expectedTime))
                                        {
                                            Console.WriteLine("Invalid Date/Time format. Please try again");
                                            continue;
                                        }
                                        selectedFlight.ExpectedTime = expectedTime;

                                        Console.WriteLine("Flight updated!");



                                    }

                                    else if (opt == "2")
                                    {
                                        Console.Write("Enter new Status: ");
                                        string newStatus = Console.ReadLine();
                                        if (string.IsNullOrEmpty(newStatus))
                                        {
                                            Console.WriteLine("Invalid Status. Please try again.");
                                            continue;
                                        }
                                        selectedFlight.Status = newStatus;
                                        Console.WriteLine("Status updated!");
                                    }

                                    else if (opt == "3")
                                    {
                                        Console.Write("Enter new Special Request Code: ");
                                        string newSpecialRequestCode = Console.ReadLine();
                                        if (string.IsNullOrEmpty(newSpecialRequestCode))
                                        {
                                            Console.WriteLine("Invalid Special Request Code. Please try again.");
                                            continue;
                                        }
                                        selectedFlight.Status = newSpecialRequestCode;
                                        Console.WriteLine("Special Request Code updated!");
                                    }

                                    else if (opt == "4")
                                    {
                                        Console.Write("Enter new Boarding Gate: ");
                                        string newGate = Console.ReadLine();
                                        if (string.IsNullOrEmpty(newGate) || !terminal.BoardingGates.ContainsKey(newGate))
                                        {
                                            Console.WriteLine("Invalid Boarding Gate. Please try again.");
                                            continue;
                                        }

                                        //Remove flight from the current gate
                                        foreach (var gate in terminal.BoardingGates.Values)
                                        {
                                            if (gate.Flight == selectedFlight)
                                            {
                                                gate.Flight = null;
                                                break;
                                            }
                                        }

                                        // Assuming there is a property BoardingGate in Flight class

                                        terminal.BoardingGates[newGate].Flight = selectedFlight;
                                        Console.WriteLine("Boarding Gate updated!");
                                    }

                                    else
                                    {
                                        Console.WriteLine("Invalid option. Please try again.");
                                        continue;
                                    }

                                    //Get boarding gate
                                    string assignedGate = "Unassigned";
                                    foreach (var gate in terminal.BoardingGates.Values)
                                    {
                                        if (gate.Flight == selectedFlight)
                                        {
                                            assignedGate = gate.GateName;
                                        }
                                    }

                                    // Display updated flight details
                                    Console.WriteLine("=============================================");
                                    Console.WriteLine("Updated Flight Details");
                                    Console.WriteLine("=============================================");
                                    Console.WriteLine($"Flight Number: {selectedFlight.FlightNo}");
                                    Console.WriteLine($"Airline Name: {selectedAirline.Name}");
                                    Console.WriteLine($"Origin: {selectedFlight.Origin}");
                                    Console.WriteLine($"Destination: {selectedFlight.Destination}");
                                    Console.WriteLine($"Expected Departure/Arrival Time: {selectedFlight.ExpectedTime}");
                                    Console.WriteLine($"Status: {selectedFlight.Status}");
                                    Console.WriteLine($"Special Request Code: {selectedFlight.Status}");
                                    Console.WriteLine($"Boarding Gate: {(assignedGate)}");
                                    Console.WriteLine(" "); // separation line
                                }

                                else if (modifyorDelete == "2")
                                {
                                    Console.Write($"Confirm deletion of flight {flightNumber} (Y to confirm, N to reject): ");
                                    string delOption = Console.ReadLine().ToUpper();

                                    if (delOption == "Y")
                                    {
                                        if (terminal.Flights.Remove(flightNumber))
                                        {
                                            Console.WriteLine($"Flight {flightNumber} has been deleted.");

                                        }
                                    }

                                    else if (delOption == "N")
                                    {
                                        Console.WriteLine("Deletion cancelled.");
                                    }
                                    else
                                    {
                                        Console.WriteLine("Invalid option. Please try again.");
                                    }


                                }
                                else
                                {
                                    Console.WriteLine("Invalid option. Please try again.");
                                }

                            }
                            else
                            {
                                Console.WriteLine("Invalid flight number. Please try again.");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid Airline Code. Please try again.");
                    }


                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine(ex.Message);
                }



            }


        }

        else if (option == 7)
        {
            while (true)
            {
                try
                {
                    Console.WriteLine("=============================================");
                    Console.WriteLine("List of Airlines for Changi Airport Terminal 5");
                    Console.WriteLine("=============================================");
                    // Listing all the airlines available
                    foreach (var airline in terminal.Airlines.Values)
                    {
                        Console.WriteLine($"Airline Code: {airline.Code}, Airline Name: {airline.Name}");
                    }
                    Console.WriteLine(" "); // separation line

                    // Prompting user for airline code
                    Console.Write("Enter the 2-Letter Airline Code (e.g. SQ or MH, etc.): ");
                    string airlineCode = Console.ReadLine().ToUpper();



                    // Validate the input
                    if (string.IsNullOrEmpty(airlineCode) || airlineCode.Length != 2)
                    {
                        throw new ArgumentException("Invalid Airline Code. Please enter a valid 2-letter code.");
                    }

                    // Retrieve the selected airline
                    Airline selectedAirline = null;
                    foreach (var airline in terminal.Airlines.Values)
                    {
                        if (airline.Code == airlineCode)
                        {
                            selectedAirline = airline;
                            break;
                        }
                    }

                    if (selectedAirline != null) //if there is an airline
                    {
                        while (true)
                        {
                            // Display flights for the selected airline
                            Console.WriteLine("=============================================");
                            Console.WriteLine($"Flights for Airline: {selectedAirline.Name}");
                            Console.WriteLine("=============================================");
                            foreach (var flight in terminal.Flights.Values)
                            {
                                if (flight.FlightNo.StartsWith(airlineCode))
                                {
                                    Console.WriteLine($"Flight Number: {flight.FlightNo}, Origin: {flight.Origin}, Destination: {flight.Destination}");
                                }
                            }
                            Console.WriteLine(" "); // separation line

                            // Prompt the user to select a Flight Number
                            Console.Write("Please enter the Flight Number: ");
                            string flightNumber = Console.ReadLine().ToUpper();


                            // Retrieve the Flight object selected
                            Flight selectedFlight = null;
                            foreach (var flight in terminal.Flights.Values)
                            {
                                if (flight.FlightNo == flightNumber)
                                {
                                    selectedFlight = flight;
                                    break;
                                }
                            }

                            if (selectedFlight != null)
                            {
                                string airlineName = selectedAirline.Name;
                                string specialRequestCode = selectedFlight.Status;
                                string boardingGate = "Not available";

                                Console.WriteLine("=============================================");
                                Console.WriteLine($"Flight Details of {selectedFlight}");
                                Console.WriteLine("=============================================");
                                Console.WriteLine($"Flight Number: {selectedFlight.FlightNo}");
                                Console.WriteLine($"Airline Name: {airlineName}");
                                Console.WriteLine($"Origin: {selectedFlight.Origin}");
                                Console.WriteLine($"Destination: {selectedFlight.Destination}");
                                Console.WriteLine($"Expected Departure/Arrival Time: {selectedFlight.ExpectedTime}");
                                Console.WriteLine($"Special Request Code: {specialRequestCode}");
                                Console.WriteLine($"Boarding Gate: {boardingGate}");
                                Console.WriteLine(" "); // separation line
                            }
                            else
                            {
                                Console.WriteLine("Invalid flight number. Please try again.");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid Airline Code. Please try again.");
                    }
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine(ex.Message);
                }

                catch (Exception ex)
                {
                    Console.WriteLine($"An error occured: {ex.Message}");
                }

            }
        }






        else if (option == 0)
        {
            break;
        }

        else
        {
            Console.WriteLine("Invalid option. Please try again.");
        }
    }


    catch (Exception ex)
    {
        Console.WriteLine($"An error occured: {ex.Message}");
    }
}