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
        }
    }
}
