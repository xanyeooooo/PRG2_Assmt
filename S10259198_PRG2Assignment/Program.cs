using S10259198_PRG2Assignment;
using System;
using System.Collections.Generic;
using System.Data;
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
        string specialrequestCode = data[4];
        double requestFee = 0;

        //Find the airline for the flight
        string airlineCode = data[0].Split(',')[0];
        Airline airline = terminal.Airlines[airlineCode];

        if (specialrequestCode == "DDJB")
        {
            requestFee = 300;
            f = new DDJBFlight(data[0], data[1], data[2], expectedtime, specialrequestCode, requestFee);
            //flight. no, origin, destination
        }
        else if (specialrequestCode == "CFFT")
        {
            requestFee = 150;
            f = new CFFTFlight(data[0], data[1], data[2], expectedtime, specialrequestCode, requestFee);
        }
        else if (specialrequestCode == "LWTT")
        {
            requestFee = 500;
            f = new LWTTFlight(data[0], data[1], data[2], expectedtime, specialrequestCode, requestFee);
        }
        else if (specialrequestCode == "")
        {
            requestFee = 0;
            f = new NORMFlight(data[0], data[1], data[2], expectedtime, specialrequestCode);
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

        else if (option == 3)
        {
            Console.WriteLine("=============================================");
            Console.WriteLine("Assign a Boarding Gate to a flight");
            Console.WriteLine("=============================================\n");
                
            Console.Write("Enter Flight Number: ");
            string FlightNumber = Console.ReadLine().ToUpper();

            if (!Flights.ContainsKey(FlightNumber))
            {
                Console.WriteLine("Flight not found.");
                return;
            }

            Flight flight = Flights[FlightNumber];
            Console.WriteLine($"\nFlight Details:\nFlight Number: {flight.FlightNo}\nAirline: {flight.Airline.Name}\nOrigin: {flight.Origin}\nDestination: {flight.Destination}\nTime: {flight.ExpectedTime}Status:{flight.Status}\nSpecial Request: {flight.specialrequestCode}");

            Console.Write("Enter Boarding Gate: ");
            string gateName = Console.ReadLine().ToUpper();

            if (!BoardingGates.ContainsKey(gateName))
            {
                Console.WriteLine("Invalid Boarding Gate.");
                return;
            }

            BoardingGate gate = BoardingGates[gateName];

            if (gate.IsAssigned)
            {
                Console.WriteLine("This Boarding Gate is already assigned to another flight.");
                return;
            }

            gate.AssignToFlight(flight);
            flight.BoardingGate = gate;

            Console.WriteLine($"\nFlight {flight.FlightNumber} has been successfully assigned to Boarding Gate {gate.Name}.");

            Console.Write("Would you like to update the status of the flight? (Y/N): ");
            string updateStatus = Console.ReadLine()?.ToUpper();

            if (updateStatus == "Y")
            {
                Console.WriteLine("\n1. Delayed\n2. Boarding\n3. On Time");
                Console.Write("Please select status of the flight: ");
                string statusChoice = Console.ReadLine();

                if (statusChoice == "1") flight.Status = "Delayed";
                else if (statusChoice == "2") flight.Status = "Boarding";
                else flight.Status = "On Time";
            }
            else
            {
                flight.Status = "On Time";
            }

            Console.WriteLine($"\nUpdated Flight Details:\nFlight Number: {flight.FlightNumber}\nStatus: {flight.Status}\nAssigned Gate: {gate.Name}");
        }
            

            else if (options == "N")
            {
                Console.WriteLine("Flight status not updated.");
            }
            else
            {
                Console.WriteLine("Invalid option. Please try again.");
            }

        

        else if (option == 4)
        {
            Console.WriteLine("\n--- Create New Flight ---");

            Console.Write("Enter Flight Number: ");
            string flightNumber = Console.ReadLine();

            if (terminal.Flights.ContainsKey(flightNumber))
            {
                Console.WriteLine("Flight already exists.");
                return;
            }

            Console.Write("Enter Airline Name: ");
            string airlineName = Console.ReadLine();

            Console.Write("Enter Origin: ");
            string origin = Console.ReadLine();

            Console.Write("Enter Destination: ");
            string destination = Console.ReadLine();

            Console.Write("Enter Expected Departure/Arrival Time: ");
            string time = Console.ReadLine();

            Console.Write("Enter Special Request Code (optional): ");
            string specialRequest = Console.ReadLine();

            var newFlight = string Flight(flightNumber, airlineName, origin, destination, time, specialRequest);
            flights[flightNumber] = newFlight;

            // Save to CSV file how do I do this 
            File.AppendAllText(flight.csv, $"{flightNumber},{airlineName},{origin},{destination},{time},{specialRequest}\n");

            Console.WriteLine("\nNew flight added successfully!");
            Console.WriteLine($"Flight Details:\nFlight Number: {flightNumber}\nAirline: {airlineName}\nOrigin: {origin}\nDestination: {destination}\nTime: {time}\nSpecial Request: {specialRequest}");

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
                            Console.WriteLine($"{"Flight Number",-15}{"Airline Name",-19}{"Origin",-19}{"Destination",-15}{"Expected Departure/Arrival Time",-40}");

                            foreach (var flight in terminal.Flights.Values)
                            {
                                if (flight.FlightNo.StartsWith(airlineCode))
                                {
                                    Console.WriteLine($"{flight.FlightNo,-15}{selectedAirline.Name,-19}{flight.Origin,-19}{flight.Destination,-15}{flight.ExpectedTime,-40}");
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

                                    else if (modifyorDelete == "2")
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

                                    else if (modifyorDelete == "3")
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

                                    //else if (modifyorDelete == "4")
                                    //{
                                    //    Console.Write("Enter new Boarding Gate: ");
                                    //    string newGate = Console.ReadLine();
                                    //    if (string.IsNullOrEmpty(newGate) || !terminal.BoardingGates.ContainsKey(newGate))
                                    //    {
                                    //        Console.WriteLine("Invalid Boarding Gate. Please try again.");
                                    //        continue;
                                    //    }
                                    //    // Assuming there is a property BoardingGate in Flight class
                                    //    selectedFlight.BoardingGate = terminal.BoardingGates[newGate];
                                    //    Console.WriteLine("Boarding Gate updated!");
                                    //}

                                    else
                                    {
                                        Console.WriteLine("Invalid option. Please try again.");
                                        continue;
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
                                    //Console.WriteLine($"Boarding Gate: {(selectedFlight.BoardingGate != null ? selectedFlight.BoardingGate.GateName : "Unassigned")}");
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
                                //string boardingGate = "Not available";

                                Console.WriteLine("=============================================");
                                Console.WriteLine($"Flight Details of {selectedFlight}");
                                Console.WriteLine("=============================================");
                                Console.WriteLine($"Flight Number: {selectedFlight.FlightNo}");
                                Console.WriteLine($"Airline Name: {selectedAirline.Name}");
                                Console.WriteLine($"Origin: {selectedFlight.Origin}");
                                Console.WriteLine($"Destination: {selectedFlight.Destination}");
                                Console.WriteLine($"Expected Departure/Arrival Time: {selectedFlight.ExpectedTime}");
                                Console.WriteLine($"Status: {selectedFlight.Status}");
                                //Console.WriteLine($"Special Request Code: {specialRequestCode}");
                                //Console.WriteLine($"Boarding Gate: {boardingGate}");
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
        Console.WriteLine("Invalid option. Please try again");
    }
}





