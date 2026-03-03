using System.ComponentModel.DataAnnotations;

namespace AirportWebAPI.Models
{
    public class Flight
    {
        [Key]
        private string _flightNumber;
        private string _destination;
        private string _departureTime;
        private string _gate;
        private string _status;

        public Flight(string flightNumber, string destination, string departureTime, string gate, string status)
        {
            _flightNumber = flightNumber;
            _destination = destination;
            _departureTime = departureTime;
            _gate = gate;
            _status = status;
        }

        [Key]

        public string FlightNumber { get => _flightNumber; set => _flightNumber = value; }
        public string Destination { get => _destination; set => _destination = value; }
        public string DepartureTime { get => _departureTime; set => _departureTime = value; }
        public string Gate { get => _gate; set => _gate = value; }
        public string Status { get => _status; set => _status = value; }
    }
}
