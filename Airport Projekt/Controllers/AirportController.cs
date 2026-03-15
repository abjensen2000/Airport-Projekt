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

            // Vi bruger await her for at sikre, at fejlene i publiceringen bliver fanget.
            await OpdaterSubscribersAsync();

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

            await OpdaterSubscribersAsync();

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
                await OpdaterSubscribersAsync();
                return Ok();

            }
            else
            {
                return BadRequest();
            }
        }

        // Vi ændrer her metoden til at returnere en Task i stedet for void.
        // Dette er vigtigt, da 'async void' gør det svært at fange fejl korrekt.
        private async Task OpdaterSubscribersAsync()
        {
            List<Flight> flyListe = _flightContext.Flights.ToList();
            
            // Flot logik til at gruppere fly efter destination!
            var flightsByDestination = flyListe.GroupBy(f => f.Destination).ToDictionary(g => g.Key, g => g.ToList());
            
            foreach (var kvp in flightsByDestination)
            {
                string destination = kvp.Key;
                List<Flight> updatedList = kvp.Value;
                
                var message = JsonSerializer.Serialize(updatedList);
                var body = Encoding.UTF8.GetBytes(message);
                
                // Vi bruger 'await using' her for at sikre, at kanalen bliver lukket korrekt.
                // Det forhindrer at vi løber tør for ressourcer i RabbitMQ.
                await using var channel = await _connection.CreateChannelAsync();
                await channel.BasicPublishAsync(exchange: "toClient", routingKey: destination, body: body);
                
                Console.WriteLine($"Publiceret flyopdateringer for destination: {destination}");
            }
        }


    }
}
