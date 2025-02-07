using Microsoft.AspNetCore.Mvc; // Importa el espacio de nombres para las funcionalidades de ASP.NET Core MVC.
using api.Data; // Importa el espacio de nombres que contiene el contexto de la base de datos.
using api.Models; // Importa el espacio de nombres que contiene los modelos de la API.
using api.Services; // Importa el espacio de nombres que contiene los servicios utilizados.
using Microsoft.EntityFrameworkCore; // Importa el espacio de nombres para las funcionalidades de Entity Framework Core.
using Microsoft.Extensions.Logging; // Importa el espacio de nombres para el registro de logs.

namespace api.Controllers // Definición del espacio de nombres para los controladores de la API.
{
    [ApiController] // Indica que esta clase es un controlador de API y habilita características de API específicas.
    [Route("api/[controller]")] // Define la ruta base para las solicitudes, donde [controller] se reemplaza por el nombre del controlador (Journey).
    public class JourneyController : ControllerBase // Clase que maneja las operaciones relacionadas con los viajes.
    {
        private readonly FlightService _flightService; // Servicio para manejar la lógica de vuelos.
        private readonly AppDbContext _context; // Contexto de la base de datos inyectado para acceder a las entidades.
        private readonly ILogger<JourneyController> _logger; // Logger para registrar información y errores.

        // Constructor que recibe el servicio de vuelos, el contexto de la base de datos y el logger.
        public JourneyController(FlightService flightService, AppDbContext context, ILogger<JourneyController> logger)
        {
            _flightService = flightService; // Inicializa el servicio de vuelos.
            _context = context; // Inicializa el contexto de la base de datos.
            _logger = logger; // Inicializa el logger.
        }

        // Método para calcular un viaje basado en el origen y el destino.
        [HttpGet("calculate")] // Indica que este método maneja las solicitudes GET en la ruta "calculate".
        public async Task<IActionResult> CalculateJourney(string origin, string destination)
        {
            _logger.LogInformation("CalculateJourney called with origin: {Origin}, destination: {Destination}", origin, destination);

            // Verifica si la ruta ya existe en la base de datos.
            var existingJourney = await _context.Journeys
                .Include(j => j.JourneyFlights) // Incluir los vuelos del viaje.
                .ThenInclude(jf => jf.Flight) // Incluir detalles del vuelo.
                .ThenInclude(f => f.Transport) // Incluir detalles del transporte.
                .FirstOrDefaultAsync(j => j.Origin == origin && j.Destination == destination); // Busca el viaje existente.

            // Si ya existe, retorna la ruta existente.
            if (existingJourney != null)
            {
                _logger.LogInformation("Existing journey found in database for origin: {Origin}, destination: {Destination}", origin, destination);
                return Ok(existingJourney); // Devuelve el viaje existente.
            }

            // Si no existe, realiza la llamada al servicio para obtener los vuelos.
            _logger.LogInformation("Fetching flights to calculate journey.");
            var flights = await _flightService.GetFlightsAsync(); // Obtiene todos los vuelos disponibles.
            var journey = FindJourney(flights, origin, destination, 10); // Busca un viaje a partir de los vuelos obtenidos.

            // Si no se encuentra un viaje válido, retorna NotFound.
            if (journey == null)
            {
                _logger.LogWarning("No valid journey found for origin: {Origin}, destination: {Destination}", origin, destination);
                return NotFound("Ruta no disponible"); // Devuelve un mensaje indicando que no hay rutas disponibles.
            }

            // Agregar el nuevo Journey a la base de datos.
            _context.Journeys.Add(journey);

            // Agregar las relaciones a JourneyFlight.
            foreach (var flight in journey.JourneyFlights)
            {
                flight.Journey = journey; // Asocia el viaje con cada vuelo.
                _context.Add(flight); // Agrega el vuelo al contexto.
            }

            // Guardar los cambios en la base de datos.
            await _context.SaveChangesAsync();
            _logger.LogInformation("New journey saved to database for origin: {Origin}, destination: {Destination}", origin, destination);

            return Ok(journey); // Devuelve el nuevo viaje creado.
        }

        // Método privado para encontrar un viaje basado en los vuelos disponibles.
        private Journey? FindJourney(List<Flight> flights, string origin, string destination, int maxFlights)
        {
            var journey = new Journey { Origin = origin, Destination = destination, Price = 0 }; // Crea un nuevo viaje.
            var currentOrigin = origin; // Almacena el origen actual.
            int flightCount = 0; // Contador de vuelos.
            var visited = new HashSet<string>(); // Conjunto para rastrear los orígenes visitados.

            // Llamada a una función recursiva que intenta encontrar la ruta.
            bool foundJourney = FindRoute(flights, journey, currentOrigin, destination, flightCount, maxFlights, visited);
            
            if (!foundJourney)
            {
                return null; // No se encontró un viaje.
            }

            // Calcular el precio total del viaje.
            journey.Price = journey.JourneyFlights.Sum(jf => jf.Flight.Price); 
            return journey; // Retorna el viaje encontrado.
        }

        // Método privado recursivo para encontrar una ruta entre vuelos.
        private bool FindRoute(List<Flight> flights, Journey journey, string origin, string destination, int flightCount, int maxFlights, HashSet<string> visited)
        {
            // Verifica si se ha alcanzado el número máximo de vuelos.
            if (flightCount >= maxFlights)
            {
                _logger.LogWarning("Se ha alcanzado el número máximo de vuelos permitidos.");
                return false; 
            }

            // Verifica si ya se ha visitado el origen actual para evitar ciclos.
            if (visited.Contains(origin))
            {
                return false; 
            }

            // Marcar el origen actual como visitado.
            visited.Add(origin);

            // Intentar encontrar un vuelo directo al destino.
            var directFlight = flights.FirstOrDefault(f => f.Origin == origin && f.Destination == destination);
            if (directFlight != null)
            {
                // Se encontró un vuelo directo, agregarlo al Journey y terminar la búsqueda.
                journey.JourneyFlights.Add(new JourneyFlight
                {
                    Flight = directFlight,
                    Journey = journey
                });

                journey.Price += directFlight.Price; // Actualiza el precio del viaje.
                return true; 
            }

            // Buscar todos los vuelos que salgan del origen actual.
            var availableFlights = flights
                .Where(f => f.Origin == origin)
                .OrderBy(f => f.Destination == destination ? 0 : 1) // Ordena los vuelos, priorizando aquellos que van directamente al destino.
                .ToList();

            if (!availableFlights.Any())
            {
                _logger.LogWarning("No se encontraron vuelos disponibles desde {CurrentOrigin}.", origin);
                visited.Remove(origin); // Remueve el origen actual del conjunto de visitados.
                return false; 
            }

            // Recorrer los vuelos disponibles para buscar una ruta recursivamente.
            foreach (var nextFlight in availableFlights)
            {
                _logger.LogInformation($"Siguiente vuelo: origen: {nextFlight.Origin}, destino: {nextFlight.Destination}");

                // Si el vuelo es hacia el destino, considerar la ruta como encontrada.
                if (nextFlight.Destination == destination)
                {
                    journey.JourneyFlights.Add(new JourneyFlight
                    {
                        Flight = nextFlight,
                        Journey = journey
                    });

                    journey.Price += nextFlight.Price; // Actualiza el precio del viaje.
                    return true; 
                }

                // Crear un nuevo JourneyFlight y continuar la búsqueda.
                var journeyFlight = new JourneyFlight
                {
                    Flight = nextFlight,
                    Journey = journey
                };

                journey.JourneyFlights.Add(journeyFlight); // Agrega el vuelo al viaje.
                var newOrigin = nextFlight.Destination; // Actualiza el origen al destino del próximo vuelo.
                int newFlightCount = flightCount + 1; // Incrementa el contador de vuelos.

                // Llamada recursiva para continuar buscando la ruta.
                if (FindRoute(flights, journey, newOrigin, destination, newFlightCount, maxFlights, visited))
                {
                    return true; // Se encontró una ruta.
                }

                // Si no se encontró la ruta, revertir el último vuelo.
                journey.JourneyFlights.RemoveAt(journey.JourneyFlights.Count - 1);
            }

            // Eliminar el origen actual del conjunto de visitados.
            visited.Remove(origin);

            return false; // No se encontró una ruta válida.
        }

        // Método para eliminar un viaje existente por ID.
        [HttpDelete("{id}")] // Indica que este método maneja las solicitudes DELETE para un viaje específico.
        public async Task<IActionResult> DeleteJourney(int id)
        {
            var journey = await _context.Journeys.FindAsync(id); // Busca el viaje por ID.

            if (journey == null) // Si no se encuentra el viaje.
            {
                return NotFound(); // Devuelve un resultado 404 Not Found.
            }
            _context.Journeys.Remove(journey); // Elimina el viaje del contexto.
            await _context.SaveChangesAsync(); // Guarda los cambios en la base de datos.

            return NoContent(); // Devuelve un resultado 204 No Content si la eliminación fue exitosa.
        }
    }
}
