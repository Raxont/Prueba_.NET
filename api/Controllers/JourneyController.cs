using Microsoft.AspNetCore.Mvc;
using api.Data;
using api.Models;
using api.Services;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JourneyController(FlightService flightService, AppDbContext context) : ControllerBase
    {
        private readonly FlightService _flightService = flightService;
        private readonly AppDbContext _context = context;

        [HttpGet("calculate")]
        public async Task<IActionResult> CalculateJourney(string origin, string destination)
        {
            // Verifica si la ruta ya existe en la base de datos
            var existingJourney = await _context.Journeys
                .Include(j => j.JourneyFlights) // Incluir los vuelos del journey
                .ThenInclude(jf => jf.Flight) // Incluir detalles del vuelo
                .FirstOrDefaultAsync(j => j.Origin == origin && j.Destination == destination);

            // Si ya existe, retorna la ruta existente
            if (existingJourney != null)
                return Ok(existingJourney);

            // Si no existe, realiza la llamada al servicio para obtener los vuelos
            var flights = await _flightService.GetFlightsAsync();
            var journey = FindJourney(flights, origin, destination,3); // Modifica la cantidad de vuelos que puede realizar

            // Si no se encuentra un journey válido, retornar NotFound
            if (journey == null)
                return NotFound("Ruta no disponible");

            // Agregar el nuevo Journey a la base de datos
            _context.Journeys.Add(journey);

            // Agregar las relaciones a JourneyFlight
            foreach (var flight in journey.JourneyFlights)
            {
                flight.Journey = journey;
                _context.Add(flight);
            }

            // Guardar los cambios en la base de datos
            await _context.SaveChangesAsync();

            return Ok(journey);
        }

        private static Journey? FindJourney(List<Flight> flights, string origin, string destination, int maxFlights)
        {
            var journey = new Journey { Origin = origin, Destination = destination, Price = 0 };
            var currentOrigin = origin;
            int flightCount = 0; // Contador de vuelos

            while (currentOrigin != destination)
            {
                // Verifica si se ha alcanzado el número máximo de vuelos
                if (flightCount >= maxFlights)
                {
                    return null; // Retorna null si se excede el límite de vuelos
                }

                var nextFlight = flights.FirstOrDefault(f => f.Origin == currentOrigin);
                if (nextFlight == null)
                {
                    return null; // No se encontró un vuelo
                }

                // Crear un nuevo JourneyFlight y establecer las propiedades necesarias
                var journeyFlight = new JourneyFlight
                {
                    Flight = nextFlight, // Establecer el vuelo
                    Journey = journey    // Establecer la relación con el viaje
                };

                journey.JourneyFlights.Add(journeyFlight); // Agregar JourneyFlight a la colección
                currentOrigin = nextFlight.Destination; // Actualizar el origen actual
                flightCount++; // Incrementar el contador de vuelos
            }

            journey.Price = journey.JourneyFlights.Sum(jf => jf.Flight.Price); // Calcular el precio
            return journey; // Retornar el viaje encontrado
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJourney(int id)
        {
            var journey = await _context.Journeys.FindAsync(id);

            if (journey == null)
            {
                return NotFound();
            }
            _context.Journeys.Remove(journey);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
