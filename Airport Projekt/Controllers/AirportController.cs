using AirportWebAPI.Data;
using AirportWebAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace AirportWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AirportController : ControllerBase
    {
        private readonly FlightContext _flightContext;
        private readonly IConnection _connection;

        public AirportController(IConnection connection, FlightContext flightContext)
        {
            _connection = connection;
            _flightContext = flightContext;
        }




        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Flight flight)
        {
            _flightContext.Flights.Add(flight);
            _flightContext.SaveChanges();
            List<Flight> flyListe = _flightContext.Flights.ToList();
            //Det nedenunder fandt jeg selv på btw
            Dictionary<string, List<Flight>> flightsByDestination = flyListe.GroupBy(flight => flight.Destination).ToDictionary(group => group.Key, group => group.ToList());
            foreach (var kvp in flightsByDestination)
            {
                string destination = kvp.Key;
                Console.WriteLine(destination);
                List<Flight> updatedOrderList = kvp.Value;
                var message = JsonSerializer.Serialize(updatedOrderList);
                var body = Encoding.UTF8.GetBytes(message);
                var channel = await _connection.CreateChannelAsync();
                await channel.BasicPublishAsync("toClient", destination, body);
            }




            return Ok(flight);
        }

        [HttpPut]
        public async Task<IActionResult> Put(string flightNumber, Flight newFlight)
        {
            if (flightNumber != newFlight.FlightNumber)
            {
                return BadRequest();
            }
            _flightContext.Entry(newFlight).State = EntityState.Modified;
            await _flightContext.SaveChangesAsync();

            List<Flight> flyListe = _flightContext.Flights.ToList();
            Dictionary<string, List<Flight>> flightsByDestination = flyListe.GroupBy(flight => flight.Destination).ToDictionary(group => group.Key, group => group.ToList());
            foreach (var kvp in flightsByDestination)
            {
                string destination = kvp.Key;
                Console.WriteLine(destination);
                List<Flight> updatedOrderList = kvp.Value;
                var message = JsonSerializer.Serialize(updatedOrderList);
                var body = Encoding.UTF8.GetBytes(message);
                var channel = await _connection.CreateChannelAsync();
                await channel.BasicPublishAsync("toClient", destination, body);
            }

            return Ok(newFlight);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(string flightNumber)
        {
            Flight currentFlight = await _flightContext.Flights.FindAsync(flightNumber);


            if (currentFlight != null)
            {
                _flightContext.Flights.Remove(currentFlight);
                await _flightContext.SaveChangesAsync();
                List<Flight> flyListe = _flightContext.Flights.ToList();
                Dictionary<string, List<Flight>> flightsByDestination = flyListe.GroupBy(flight => flight.Destination).ToDictionary(group => group.Key, group => group.ToList());
                foreach (var kvp in flightsByDestination)
                {
                    string destination = kvp.Key;
                    Console.WriteLine(destination);
                    List<Flight> updatedOrderList = kvp.Value;
                    var message = JsonSerializer.Serialize(updatedOrderList);
                    var body = Encoding.UTF8.GetBytes(message);
                    var channel = await _connection.CreateChannelAsync();
                    await channel.BasicPublishAsync("toClient", destination, body);
                }
                return Ok();

            }
            else
            {
                return BadRequest();
            }
        }


    }
}
