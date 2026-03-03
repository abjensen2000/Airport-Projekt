using AirportWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AirportWebAPI.Data
{
    public class FlightContext : DbContext
    {
        public FlightContext(DbContextOptions dbContextOptions) : base(options: dbContextOptions) 
        {
            
        }

        public DbSet<Flight> Flights {  get; set; }
    }
}
