using S10259198_PRG2Assignment;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Net.Http.Headers;

Terminal terminal = new Terminal("Terminal 5");
Dictionary<string, Airline> Airlines = terminal.Airlines;
Dictionary<string, BoardingGate> BoardingGates = terminal.BoardingGates;
Dictionary<string, Flight> Flights = terminal.Flights;
Dictionary<string, string> flightSpecialRequestCodes = new Dictionary<string, string>();

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
        Flight? f = null; // Use nullable type
        string flightSpecialRequestCode = data[4];
        double requestFee = 0;

        if (string.IsNullOrEmpty(flightSpecialRequestCode))
        {
            flightSpecialRequestCode = "N.A";
        }

        if (flightSpecialRequestCode == "DDJB")
        {
            requestFee = 300;
            f = new DDJBFlight(data[0], data[1], data[2], expectedtime, "Scheduled", requestFee);
        }
        else if (flightSpecialRequestCode == "CFFT")
        {
            requestFee = 150;
            f = new CFFTFlight(data[0], data[1], data[2], expectedtime, "Scheduled", requestFee);
        }
        else if (flightSpecialRequestCode == "LWTT")
        {
            requestFee = 500;
            f = new LWTTFlight(data[0], data[1], data[2], expectedtime, "Scheduled", requestFee);
        }
        else if (flightSpecialRequestCode == "N.A")
        {
            requestFee = 0;
            f = new NORMFlight(data[0], data[1], data[2], expectedtime, "Scheduled");
        }

        if (f != null)
        {
            terminal.Flights.Add(data[0], f);
            flightSpecialRequestCodes[data[0]] = flightSpecialRequestCode; // Store the special request code
            flightCount++;
        }
        else
        {
            Console.WriteLine($"Error: Flight object for {data[0]} could not be created.");
        }
    }
}
Console.WriteLine($"{flightCount} Flights Loaded!");

Console.WriteLine(""); //separation line

// Main menu
MainMenu();
void MainMenu()
{
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

            string option = Console.ReadLine();

            if (option == "1")
            {
                ListAllFlights();
            }

            else if (option == "2")
            {
                ListBoardingGates();
            }

            else if (option == "3")
            {
                AssignBoardingGateToFlight();
            }

            //else if (option == "4")
            //{
            //    CreateFlight();
            //}

            else if (option == "5")
            {
                DisplayAirlineFlights(out Airline selectedAirline);
            }

            else if (option == "6")
            {
                ModifyFlightDetails();
            }

            else if (option == "7")

            {
                DisplayFlightSchedule();
            }

            else if (option == "0")
            {
                Console.WriteLine("Goodbye!");
                break;
            }

            else
            {
                Console.WriteLine("Invalid option. Please try again.");
            }


        }
        catch
        {
            Console.WriteLine("Invalid option. Please try again.");
        }
    }
}
void ListAllFlights()
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

void ListBoardingGates()
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

void AssignBoardingGateToFlight()
{
    while (true)
    {
        Console.WriteLine("=============================================");
        Console.WriteLine("Assign a Boarding Gate to a Flight");
        Console.WriteLine("=============================================");
        Console.Write("Enter Flight Number: ");
        string flightNo = Console.ReadLine().ToUpper();

        if (!Flights.ContainsKey(flightNo))
        {
            Console.WriteLine("Flight not found. Please try again.");
            continue;
        }

        string gateName;
        while (true)
        {
            Console.Write("Enter Boarding Gate Name: ");
            gateName = Console.ReadLine().ToUpper();

            if (!BoardingGates.ContainsKey(gateName))
            {
                Console.WriteLine("The Boarding Gate does not exist. Please try again.");
            }
            else if (BoardingGates[gateName].Flight != null)
            {
                Console.WriteLine("The Boarding Gate is already assigned to another flight. Please try again.");
            }
            else
            {
                BoardingGates[gateName].Flight = Flights[flightNo];
                Flights[flightNo].Status = "Scheduled";
                break;
            }
        }

        // Retrieving details
        Flight flight = Flights[flightNo];
        BoardingGate gate = BoardingGates[gateName];
        string specialRequestCode = flightSpecialRequestCodes[flightNo];

        // Displaying details
        Console.WriteLine(" "); // separation line
        Console.WriteLine($"Flight Number: {flight.FlightNo}");
        Console.WriteLine($"Origin: {flight.Origin}");
        Console.WriteLine($"Destination: {flight.Destination}");
        Console.WriteLine($"Expected Time: {flight.ExpectedTime}");
        Console.WriteLine($"Special Request Code: {specialRequestCode}");
        Console.WriteLine($"Boarding Gate Name: {gateName}");
        Console.WriteLine($"Supports DDJB: {gate.SupportsDDJB}");
        Console.WriteLine($"Supports CFFT: {gate.SupportsCFFT}");
        Console.WriteLine($"Supports LWTT: {gate.SupportsLWTT}");

        // Prompt user if want to update status
        while (true)
        {
            Console.Write("Would you like to update the status of the flight? (Y/N): ");
            string updateStatus = Console.ReadLine().ToUpper();

            if (updateStatus == "Y")
            {
                while (true)
                {
                    Console.WriteLine("1. Delayed");
                    Console.WriteLine("2. Boarding");
                    Console.WriteLine("3. On Time");
                    Console.Write("Please select the new status of the flight: ");
                    string statusOption = Console.ReadLine();

                    if (statusOption == "1")
                    {
                        flight.Status = "Delayed";
                        break;
                    }
                    else if (statusOption == "2")
                    {
                        flight.Status = "Boarding";
                        break;
                    }
                    else if (statusOption == "3")
                    {
                        flight.Status = "On Time";
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Invalid option. Please try again.");
                        continue;
                    }
                }
                break;
            }
            else if (updateStatus == "N")
            {
                // Do nothing, just continue
                break;
            }
            else
            {
                Console.WriteLine("Invalid option. Please try again.");
            }
        }

        Console.WriteLine($"Flight {flight.FlightNo} has been assigned to Boarding Gate {gate.GateName}!");
        break;
    }
}

//void CreateFlight() - TODO


void DisplayAirlineFlights(out Airline selectedAirline)
{
    selectedAirline = null;
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
                Console.WriteLine("Invalid Airline Code. Please enter a valid 2-letter code.");
                continue;
            }

            // Retrieve the selected airline
            foreach (var airline in terminal.Airlines.Values)
            {
                if (airline.Code == airlineCode)
                {
                    selectedAirline = airline;
                    break;
                }
            }

            // Check if the airline code is valid
            if (selectedAirline == null)
            {
                Console.WriteLine("Airline Code not found. Please try again.");
                continue;
            }

            // Display flights for the selected airline
            Console.WriteLine("=============================================");
            Console.WriteLine($"List of Flights for: {selectedAirline.Name}");
            Console.WriteLine("=============================================");
            Console.WriteLine($"{"Flight Number",-15}{"Airline Name",-25}{"Origin",-19}{"Destination",-20}{"Expected Departure/Arrival Time",-40}");

            foreach (var flight in terminal.Flights.Values)
            {
                if (flight.FlightNo.StartsWith(airlineCode))
                {
                    Console.WriteLine($"{flight.FlightNo,-15}{selectedAirline.Name,-25}{flight.Origin,-19}{flight.Destination,-20}{flight.ExpectedTime,-40}");
                }
            }
            Console.WriteLine(" "); // separation line
            break;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}

Flight GetSelectedFlight()
{
    while (true)
    {
        Console.Write("Choose an existing Flight to modify or delete: ");
        string flightNumber = Console.ReadLine().ToUpper();

        // Retrieve the Flight
        foreach (var flight in terminal.Flights.Values)
        {
            if (flight.FlightNo == flightNumber)
            {
                return flight;
            }
        }

        Console.WriteLine("Invalid flight number. Please try again.");
    }
}




void ModifyFlightDetails()
{
    while (true)
    {
        try
        {
            DisplayAirlineFlights(out Airline selectedAirline);

            Flight selectedFlight = GetSelectedFlight();

            if (selectedFlight != null)
            {
                while (true)
                {
                    Console.WriteLine("1. Modify Flight");
                    Console.WriteLine("2. Delete Flight");
                    Console.Write("Choose an option:");

                    string modifyorDelete = Console.ReadLine();

                    if (modifyorDelete == "1")
                    {
                        Console.WriteLine(" "); // separation line
                        Console.WriteLine("1. Modify Basic Information");
                        Console.WriteLine("2. Modify Status");
                        Console.WriteLine("3. Modify Special Request Code");
                        Console.WriteLine("4. Modify Boarding Gate");
                        Console.Write("Choose an option: ");
                        string opt = Console.ReadLine();

                        if (opt == "1")
                        {
                            while (true)
                            {
                                // New origin
                                Console.Write("Enter new Origin: ");
                                string newOrigin = Console.ReadLine();

                                if (string.IsNullOrEmpty(newOrigin))
                                {
                                    Console.WriteLine("Invalid origin. Please try again.");
                                    continue;
                                }
                                selectedFlight.Origin = newOrigin;

                                // New destination
                                Console.Write("Enter new Destination: ");
                                string newDestination = Console.ReadLine();
                                if (string.IsNullOrEmpty(newDestination))
                                {
                                    Console.WriteLine("Invalid destination. Please try again");
                                    continue;
                                }
                                selectedFlight.Destination = newDestination;

                                // New arrival / departure time
                                Console.Write("Enter new Expected Departure / Arrival Time (dd/mm/yyyy hh:mm): ");
                                string newExpectedTime = Console.ReadLine();

                                if (!DateTime.TryParse(newExpectedTime, out DateTime expectedTime))
                                {
                                    Console.WriteLine("Invalid Date/Time format. Please try again");
                                    continue;
                                }
                                selectedFlight.ExpectedTime = expectedTime;

                                Console.WriteLine("Flight updated!");
                                break;
                            }
                        }
                        else if (opt == "2")
                        {
                            while (true)
                            {
                                Console.WriteLine("1. Delayed");
                                Console.WriteLine("2. Boarding");
                                Console.WriteLine("3. On Time");
                                Console.Write("Please select the new status of the flight (1/2/3): ");
                                string statusOption = Console.ReadLine();

                                if (statusOption == "1")
                                {
                                    selectedFlight.Status = "Delayed";
                                    break;
                                }
                                else if (statusOption == "2")
                                {
                                    selectedFlight.Status = "Boarding";
                                    break;
                                }
                                else if (statusOption == "3")
                                {
                                    selectedFlight.Status = "On Time";
                                    break;
                                }
                                else
                                {
                                    Console.WriteLine("Invalid option. Please try again.");
                                }
                            }

                            Console.WriteLine("Status updated!");
                        }
                        else if (opt == "3")
                        {
                            while (true)
                            {
                                Console.Write("Enter new Special Request Code (CFFT/LWTT/DDJB): ");
                                string newSpecialRequestCode = Console.ReadLine().ToUpper();
                                if (string.IsNullOrEmpty(newSpecialRequestCode) || !flightSpecialRequestCodes.ContainsValue(newSpecialRequestCode))
                                {
                                    Console.WriteLine("Invalid Special Request Code. Please try again.");
                                    continue;
                                }
                                flightSpecialRequestCodes[selectedFlight.FlightNo] = newSpecialRequestCode;
                                Console.WriteLine("Special Request Code updated!");
                                break;
                            }
                        }
                        else if (opt == "4")
                        {
                            while (true)
                            {
                                Console.Write("Enter new Boarding Gate: ");
                                string newGate = Console.ReadLine().ToUpper();
                                if (string.IsNullOrEmpty(newGate) || !terminal.BoardingGates.ContainsKey(newGate))
                                {
                                    Console.WriteLine("Invalid Boarding Gate. Please try again.");
                                    continue;
                                }

                                // Remove flight from the current gate
                                foreach (var gate in terminal.BoardingGates.Values)
                                {
                                    if (gate.Flight == selectedFlight)
                                    {
                                        gate.Flight = null;
                                        break;
                                    }
                                }

                                // Assign the flight to the new gate
                                terminal.BoardingGates[newGate].Flight = selectedFlight;
                                Console.WriteLine("Boarding Gate updated!");
                                break;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid option. Please try again.");
                            continue;
                        }

                        // Get boarding gate
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
                        Console.WriteLine($"Special Request Code: {flightSpecialRequestCodes[selectedFlight.FlightNo]}");
                        Console.WriteLine($"Boarding Gate: {assignedGate}");
                        Console.WriteLine(" "); // separation line
                        return; // Return to main menu
                    }
                    else if (modifyorDelete == "2")
                    {
                        while (true)
                        {
                            Console.Write($"Confirm deletion of flight {selectedFlight.FlightNo} (Y to confirm, N to reject): ");
                            string delOption = Console.ReadLine().ToUpper();

                            if (delOption == "Y")
                            {
                                if (terminal.Flights.Remove(selectedFlight.FlightNo))
                                {
                                    Console.WriteLine($"Flight {selectedFlight.FlightNo} has been deleted.");
                                    return; // Return to main menu
                                }
                            }
                            else if (delOption == "N")
                            {
                                Console.WriteLine("Deletion cancelled.");
                                return; // Return to main menu
                            }
                            else
                            {
                                Console.WriteLine("Invalid option. Please try again.");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid option. Please try again.");
                    }
                }
            }
            else
            {
                Console.WriteLine("Invalid flight number. Please try again.");
                continue;
            }
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}

void DisplayFlightSchedule()
{
    try
    {
        Console.WriteLine("=============================================");
        Console.WriteLine("Flight Schedule for Changi Airport Terminal 5");
        Console.WriteLine("=============================================");
        Console.WriteLine($"{"Flight Number",-20}{"Airline Name",-20}{"Origin",-20}{"Destination",-25}{"Expected Departure / Arrival Time",-35}{" ",5}{"Status",-20}{"Boarding Gate",-20}");

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

            // Get boarding gate
            string assignedGate = "Unassigned";
            foreach (var gate in terminal.BoardingGates.Values)
            {
                if (gate.Flight == flight)
                {
                    assignedGate = gate.GateName;
                    break;
                }
            }

            // Check for empty status and replace with "N.A."
            string status = string.IsNullOrEmpty(flight.Status) ? "N.A." : flight.Status;

            Console.WriteLine($"{flight.FlightNo,-20}{airlineName,-20}{flight.Origin,-20}{flight.Destination,-25}{flight.ExpectedTime,-25}{status,-20}{assignedGate,-20}");
        }
        Console.WriteLine(" "); // separation line
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred: {ex.Message}");
    }
}








