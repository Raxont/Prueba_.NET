// Controllers/JourneyController.cs
using Microsoft.AspNetCore.Mvc;
using api.Data;
using api.Models;
using api.Services;

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
            var flights = await _flightService.GetFlightsAsync();
            var journey = FindJourney(flights, origin, destination);

            if (journey == null)
                return NotFound("Ruta no disponible");

            // Verifica si la ruta ya existe en la base de datos
            var existingJourney = _context.Journeys.FirstOrDefault(j => j.Origin == origin && j.Destination == destination);
            if (existingJourney != null)
                return Ok(existingJourney);

            // Guarda la ruta en la base de datos
            _context.Journeys.Add(journey);
            await _context.SaveChangesAsync();

            return Ok(journey);
        }

        private static Journey? FindJourney(List<Flight> flights, string origin, string destination)
        {
            var journey = new Journey { Origin = origin, Destination = destination, Price = 0  };
            var currentOrigin = origin;

            while (currentOrigin != destination)
            {
                var nextFlight = flights.FirstOrDefault(f => f.Origin == currentOrigin);
                if (nextFlight == null) return null;

                journey.Flights.Add(nextFlight);
                currentOrigin = nextFlight.Destination;
            }

            journey.Price = journey.Flights.Sum(f => f.Price);
            return journey;
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJourney(int id)
        {
            var journey = await _context.Journeys.FindAsync(id);

            if (journey == null)
            {
                return NotFound();
            }
            Console.WriteLine($"Eliminando el viaje con ID {id}, Origen: {journey.Origin}, Destino: {journey.Destination}, Precio: {journey.Price}.");

            _context.Journeys.Remove(journey);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
