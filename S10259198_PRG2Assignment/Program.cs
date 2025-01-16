using S10259198_PRG2Assignment;
using System;
using System.Collections.Generic;
using System.IO;

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
        Console.WriteLine("8. Modify Flight Details");
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

        else if (option == 8)
        {
            while (true)
            {
                try
                {
                    Console.WriteLine("==============================================");
                    Console.WriteLine("List of Airlines for Changi Airport Terminal 5");
                    Console.WriteLine("==============================================");
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


                            //Prompt user input for modify or delete
                            Console.Write("Enter 1 to choose an existing flight to modify, or 2, to choose an existing flight to delete:");
                            string opt = Console.ReadLine();

                            if (opt == "1")
                            {
                                Console.Write("Please enter flight number to modify: ");
                                string flightNumber = Console.ReadLine().ToUpper();

                                //Retrieve the Flight object selected
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
                                    while (true)
                                    {
                                        Console.WriteLine("What would you like to modify?");
                                        Console.WriteLine("1. Origin");
                                        Console.WriteLine("2. Destination");
                                        Console.WriteLine("3. Expected Departure / Arrival Time");
                                        Console.WriteLine("4. Status");
                                        Console.WriteLine("5. Special Request Code");
                                        Console.WriteLine("6. Boarding Gate");
                                        Console.Write("Please select an option: ");
                                        string modifyOption = Console.ReadLine();

                                        if (modifyOption == "1")
                                        {
                                            Console.Write("Enter new origin: ");
                                            selectedFlight.Origin = Console.ReadLine();
                                            Console.WriteLine($"Origin has been changed to {selectedFlight.Origin}");
                                        }

                                        else if (modifyOption == "2")
                                        {
                                            Console.Write("Enter new destination: ");
                                            selectedFlight.Destination = Console.ReadLine();
                                            Console.WriteLine($"Destination has been changed to {selectedFlight.Destination}");
                                        }

                                        else if (modifyOption == "3")
                                        {
                                            Console.Write("Enter new expected departure / arrival time (yyyy-MM-dd HH:mm");
                                            selectedFlight.ExpectedTime = DateTime.Parse(Console.ReadLine());
                                            Console.WriteLine($"Expected departure / arrival time has been changed to {selectedFlight.ExpectedTime}");
                                        }

                                        //else if (modifyOption == "4")
                                        //{
                                        //    Console.Write("Enter new status: ");
                                        //    selectedFlight.Status = Console.ReadLine();

                                        //}

                                        else if (modifyOption == "5")
                                        {
                                            Console.WriteLine("Enter new special request code: ");
                                            selectedFlight.Status = Console.ReadLine();
                                            Console.WriteLine($"Special request code has been changed to {selectedFlight.Status}");
                                        }

                                        //else if (modifyOption == "6")
                                        //{
                                        //    Console.Write("Enter new boarding gate: ");
                                        //    string newGate = Console.ReadLine();

                                        //    if (terminal.BoardingGates.ContainsKey(newGate))
                                        //    {
                                        //        selectedFlight.
                                        //    }
                                        //}

                                        else
                                        {
                                            Console.WriteLine("Invalid option. Please try again.");
                                        }

                                        //Display updated flight details
                                        Console.WriteLine("=============================================");
                                        Console.WriteLine("Updated Flight Details");
                                        Console.WriteLine("=============================================");
                                        Console.WriteLine($"Flight Number: {selectedFlight.FlightNo}");
                                        Console.WriteLine($"Airline Name: {selectedAirline.Name}");
                                        Console.WriteLine($"Origin: {selectedFlight.Origin}");
                                        Console.WriteLine($"Destination: {selectedFlight.Destination}");
                                        Console.WriteLine($"Expected Departure/Arrival Time: {selectedFlight.ExpectedTime}");
                                        //Console.WriteLine($"Status: {selectedFlight.Status}");
                                        Console.WriteLine($"Special Request Code: {selectedFlight.Status}");
                                        //Console.WriteLine($"Boarding Gate: {(selectedFlight.BoardingGate != null ? selectedFlight.BoardingGate.GateName : "Not available")}");
                                        Console.WriteLine(" "); // separation line
                                        break;
                                    }
                                }

                                else
                                {
                                    Console.WriteLine("Invalid flight number. Please try again.");
                                    continue;
                                }



                            }

                            else if (opt == "2")
                            {
                                Console.Write("Please enter flight number to delete: ");
                                string flightNumber = Console.ReadLine().ToUpper();

                                //Retrieve the Flight object selected
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
                                    Console.Write($"Please confirm deletion of flight {flightNumber} (Y to confirm, N to reject): ");
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
                                        Console.WriteLine("Invalid flight number. Please try again.");
                                        continue;
                                    }
                                }

                            }

                            else
                            {
                                Console.WriteLine("Invalid option. Please try again. ");
                                continue;
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
        Console.WriteLine("Invalid option. Please try again");
    }
}