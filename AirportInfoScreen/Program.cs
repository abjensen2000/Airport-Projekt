using AirportWebAPI.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;

public class Program
{

    private static IChannel _channel;
    private static List<Flight> _flights = [];
    static async Task Main(string[] args)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        await channel.ExchangeDeclareAsync("toClient", ExchangeType.Topic);

        Console.WriteLine("Declare Airport");
        string? airport = Console.ReadLine();

        QueueDeclareOk queueDeclareResult = await channel.QueueDeclareAsync();
        await channel.QueueBindAsync(queueDeclareResult.QueueName , exchange: "toClient", routingKey: airport);

        _channel = channel;

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += (model, ea) =>
        {
            byte[] body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            List<Flight>? flights = JsonSerializer.Deserialize<List<Flight>>(message);
            if (flights is not null)
            {
                _flights = flights;
                PrintOrders();
            }
            else
            {
                Console.WriteLine("Flights is null");
            }


            return Task.CompletedTask;
        };

        await channel.BasicConsumeAsync(queueDeclareResult.QueueName, autoAck: true, consumer: consumer);


        Console.ReadLine();
    }


    private static void PrintOrders()
    {
        Console.WriteLine("FlightNumber  | Destination | Afgangs tidspunkt | Gate | Status");
        Console.WriteLine("-----------------------------------------");
        _flights.ForEach(flight =>
        {
            Console.WriteLine($"{flight.FlightNumber} | {flight.Destination} | {flight.DepartureTime} | {flight.Gate} | {flight.Status}");
        });
        Console.WriteLine("-----------------------------------------");
    }
}