using Microsoft.AspNetCore.Mvc;
using api.Data;
using api.Models;
using api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JourneyController(FlightService flightService, AppDbContext context, ILogger<JourneyController> logger) : ControllerBase
    {
        private readonly FlightService _flightService = flightService;
        private readonly AppDbContext _context = context;
        private readonly ILogger<JourneyController> _logger = logger;

        [HttpGet("calculate")]
        public async Task<IActionResult> CalculateJourney(string origin, string destination)
        {
            _logger.LogInformation("CalculateJourney called with origin: {Origin}, destination: {Destination}", origin, destination);

            // Verifica si la ruta ya existe en la base de datos
            var existingJourney = await _context.Journeys
                .Include(j => j.JourneyFlights) // Incluir los vuelos del journey
                .ThenInclude(jf => jf.Flight) // Incluir detalles del vuelo
                .ThenInclude(f => f.Transport) // Incluir detalles del transporte
                .FirstOrDefaultAsync(j => j.Origin == origin && j.Destination == destination);

            // Si ya existe, retorna la ruta existente
            if (existingJourney != null)
            {
                _logger.LogInformation("Existing journey found in database for origin: {Origin}, destination: {Destination}", origin, destination);
                return Ok(existingJourney);
            }

            // Si no existe, realiza la llamada al servicio para obtener los vuelos
            _logger.LogInformation("Fetching flights to calculate journey.");
            var flights = await _flightService.GetFlightsAsync();
            var journey = FindJourney(flights, origin, destination, 10); // Modifica la cantidad de vuelos que puede realizar

            // Si no se encuentra un journey válido, retornar NotFound
            if (journey == null)
            {
                _logger.LogWarning("No valid journey found for origin: {Origin}, destination: {Destination}", origin, destination);
                return NotFound("Ruta no disponible");
            }

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
            _logger.LogInformation("New journey saved to database for origin: {Origin}, destination: {Destination}", origin, destination);

            return Ok(journey);
        }

        private Journey? FindJourney(List<Flight> flights, string origin, string destination, int maxFlights)
        {
            var journey = new Journey { Origin = origin, Destination = destination, Price = 0 };
            var currentOrigin = origin;
            int flightCount = 0; // Contador de vuelos
            var visited = new HashSet<string>();

            // Llamada a una función recursiva que intenta encontrar la ruta
            bool foundJourney = FindRoute(flights, journey, currentOrigin, destination, flightCount, maxFlights, visited);
            
            if (!foundJourney)
            {
                return null; // No se encontró un viaje
            }

            // Calcular el precio total del viaje
            journey.Price = journey.JourneyFlights.Sum(jf => jf.Flight.Price); 
            return journey; // Retornar el viaje encontrado
        }

        private bool FindRoute(List<Flight> flights, Journey journey, string origin, string destination, int flightCount, int maxFlights, HashSet<string> visited)
        {
            // Verifica si se ha alcanzado el número máximo de vuelos
            if (flightCount >= maxFlights)
            {
                _logger.LogWarning("Se ha alcanzado el número máximo de vuelos permitidos.");
                return false; 
            }

            // Verifica si ya se ha visitado el origen actual para evitar ciclos
            if (visited.Contains(origin))
            {
                return false; 
            }

            // Marcar el origen actual como visitado
            visited.Add(origin);

            // Intentar encontrar un vuelo directo al destino
            var directFlight = flights.FirstOrDefault(f => f.Origin == origin && f.Destination == destination);
            if (directFlight != null)
            {
                // Se encontró un vuelo directo, agregarlo al Journey y terminar la búsqueda
                journey.JourneyFlights.Add(new JourneyFlight
                {
                    Flight = directFlight,
                    Journey = journey
                });

                journey.Price += directFlight.Price;
                return true; 
            }

            // Buscar todos los vuelos que salgan del origen actual
            var availableFlights = flights
                .Where(f => f.Origin == origin)
                .OrderBy(f => f.Destination == destination ? 0 : 1)
                .ToList();

            if (!availableFlights.Any())
            {
                _logger.LogWarning("No se encontraron vuelos disponibles desde {CurrentOrigin}.", origin);
                visited.Remove(origin); 
                return false; 
            }

            // Recorrer los vuelos disponibles para buscar una ruta recursivamente
            foreach (var nextFlight in availableFlights)
            {
                _logger.LogInformation($"Siguiente vuelo: origen: {nextFlight.Origin}, destino: {nextFlight.Destination}");

                // Si el vuelo es hacia el destino, considerar la ruta como encontrada
                if (nextFlight.Destination == destination)
                {
                    journey.JourneyFlights.Add(new JourneyFlight
                    {
                        Flight = nextFlight,
                        Journey = journey
                    });

                    journey.Price += nextFlight.Price;
                    return true;
                }

                // Crear un nuevo JourneyFlight y continuar la búsqueda
                var journeyFlight = new JourneyFlight
                {
                    Flight = nextFlight,
                    Journey = journey
                };

                journey.JourneyFlights.Add(journeyFlight);
                var newOrigin = nextFlight.Destination;
                int newFlightCount = flightCount + 1;

                // Llamada recursiva para continuar buscando la ruta
                if (FindRoute(flights, journey, newOrigin, destination, newFlightCount, maxFlights, visited))
                {
                    return true; 
                }

                // Si no se encontró la ruta, revertir el último vuelo
                journey.JourneyFlights.RemoveAt(journey.JourneyFlights.Count - 1);
            }

            // Eliminar el origen actual del conjunto de visitados
            visited.Remove(origin);

            return false; 
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
