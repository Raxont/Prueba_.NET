using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Data
{
    public class AppDbContext  : DbContext
    {
        public AppDbContext (DbContextOptions<AppDbContext> options): base(options)
        {
                
        }

        public DbSet<Journey> Journeys { get; set; }
        public DbSet<Flight> Flights { get; set; }
        public DbSet<Transport> Transports { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Flight>()
                .HasOne(f => f.Transport)
                .WithMany()
                .HasForeignKey(f => f.TransportId);
            
            modelBuilder.Entity<JourneyFlight>()
                .HasKey(jf => new { jf.JourneyId, jf.FlightId });

            modelBuilder.Entity<JourneyFlight>()
                .HasOne(jf => jf.Journey)
                .WithMany(j => j.JourneyFlights)
                .HasForeignKey(jf => jf.JourneyId);

            modelBuilder.Entity<JourneyFlight>()
                .HasOne(jf => jf.Flight)
                .WithMany()
                .HasForeignKey(jf => jf.FlightId);

            modelBuilder.Entity<Transport>()
                .HasKey(t => t.Id); // Define la clave primaria
            
            modelBuilder.Entity<Transport>()
                .Property(t => t.Id)
                .ValueGeneratedOnAdd(); // Asegúrate de que sea autogenerado
        }
    }
}
