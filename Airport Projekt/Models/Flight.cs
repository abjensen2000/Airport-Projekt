using System.ComponentModel.DataAnnotations;

namespace AirportWebAPI.Models
{
    public class Flight
    {
        // Vi bruger PascalCase (stort begyndelsesbogstav) til public properties i C#.
        // Auto-properties gør koden meget mere læsbar og moderne.
        [Key]
        public string FlightNumber { get; set; }
        public string Destination { get; set; }
        
        // Vi bruger DateTime i stedet for string for at kunne sortere og lave beregninger på tid.
        public DateTime DepartureTime { get; set; }
        public string Gate { get; set; }
        public string Status { get; set; }

        public Flight() { } // EF Core kræver en tom constructor.

        public Flight(string flightNumber, string destination, DateTime departureTime, string gate, string status)
        {
            FlightNumber = flightNumber;
            Destination = destination;
            DepartureTime = departureTime;
            Gate = gate;
            Status = status;
        }
    }
}
