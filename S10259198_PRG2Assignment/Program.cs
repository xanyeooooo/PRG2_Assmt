using S10259198_PRG2Assignment;

Terminal terminal = new Terminal("Terminal 5");
Dictionary<string, Airline> Airlines = terminal.Airlines;

using (StreamReader sr = new StreamReader("airlines.csv", true))
{
    string s;
    string airlinename;
    string airlinecode;
    while ((s=sr.ReadLine()) != null)
    {
        string[] data = s.Split(',');

        airlinename = data[0];
        airlinecode = data[1];

        Airline a = new Airline(data[0], data[1]);
        terminal.Airlines.Add(airlinecode,a);
    }
}









    Console.WriteLine("Loading Airlines...");
