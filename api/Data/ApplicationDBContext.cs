using api.Models; // Importa el espacio de nombres que contiene los modelos de la API.
using Microsoft.EntityFrameworkCore; // Importa el espacio de nombres de Entity Framework Core.

namespace api.Data // Definición del espacio de nombres para los datos de la API.
{
    public class AppDbContext : DbContext // Clase que representa el contexto de la base de datos, heredando de DbContext.
    {
        // Constructor que recibe las opciones de configuración del contexto de la base de datos.
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            // Inicializa el contexto de la base de datos con las opciones proporcionadas.
        }

        // Propiedades que representan las tablas en la base de datos.
        public DbSet<Journey> Journeys { get; set; } // Representa la tabla de viajes.
        public DbSet<Flight> Flights { get; set; } // Representa la tabla de vuelos.
        public DbSet<Transport> Transports { get; set; } // Representa la tabla de transportes.

        // Método que se llama al crear el modelo, se utiliza para configurar relaciones y claves.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configura la relación entre Flight y Transport.
            modelBuilder.Entity<Flight>()
                .HasOne(f => f.Transport) // Cada vuelo tiene un único transporte.
                .WithMany() // Un transporte puede estar asociado a muchos vuelos.
                .HasForeignKey(f => f.TransportId); // Define la clave foránea.

            // Configura la clave primaria compuesta para JourneyFlight.
            modelBuilder.Entity<JourneyFlight>()
                .HasKey(jf => new { jf.JourneyId, jf.FlightId }); // Define la clave primaria como una combinación de JourneyId y FlightId.

            // Configura la relación entre JourneyFlight y Journey.
            modelBuilder.Entity<JourneyFlight>()
                .HasOne(jf => jf.Journey) // Cada JourneyFlight tiene un único viaje.
                .WithMany(j => j.JourneyFlights) // Un viaje puede estar asociado a muchos JourneyFlights.
                .HasForeignKey(jf => jf.JourneyId); // Define la clave foránea.

            // Configura la relación entre JourneyFlight y Flight.
            modelBuilder.Entity<JourneyFlight>()
                .HasOne(jf => jf.Flight) // Cada JourneyFlight tiene un único vuelo.
                .WithMany() // Un vuelo puede estar asociado a muchos JourneyFlights.
                .HasForeignKey(jf => jf.FlightId); // Define la clave foránea.

            // Configura la entidad Transport.
            modelBuilder.Entity<Transport>()
                .HasKey(t => t.Id); // Define la clave primaria para la entidad Transport.
            
            modelBuilder.Entity<Transport>()
                .Property(t => t.Id)
                .ValueGeneratedOnAdd(); // Configura la propiedad Id para que sea autogenerada.
        }
    }
}
