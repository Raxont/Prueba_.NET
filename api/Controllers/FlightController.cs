using Microsoft.AspNetCore.Mvc; // Importa el espacio de nombres para las funcionalidades de ASP.NET Core MVC.
using api.Data; // Importa el espacio de nombres que contiene el contexto de la base de datos.
using api.Models; // Importa el espacio de nombres que contiene los modelos de la API.
using Microsoft.EntityFrameworkCore; // Importa el espacio de nombres para las funcionalidades de Entity Framework Core.

namespace api.Controllers // Definición del espacio de nombres para los controladores de la API.
{
    [ApiController] // Indica que esta clase es un controlador de API y habilita características específicas de API.
    [Route("api/[controller]")] // Define la ruta base para las solicitudes, donde [controller] se reemplaza por el nombre del controlador (Flight).
    public class FlightController : ControllerBase // Clase que maneja las operaciones relacionadas con los vuelos.
    {
        private readonly AppDbContext _context; // Contexto de la base de datos inyectado para acceder a las entidades.

        // Constructor que recibe el contexto de la base de datos.
        public FlightController(AppDbContext context)
        {
            _context = context; // Inicializa el contexto de la base de datos.
        }

        // Método para obtener la lista de todos los vuelos.
        [HttpGet] // Indica que este método maneja las solicitudes GET en la ruta base del controlador.
        public async Task<ActionResult<IEnumerable<Flight>>> GetFlights()
        {
            // Devuelve la lista de vuelos de manera asíncrona.
            return await _context.Flights.ToListAsync();
        }

        // Método para obtener un vuelo específico por su ID.
        [HttpGet("{id}")] // Indica que este método maneja las solicitudes GET en la ruta "api/Flight/{id}".
        public async Task<ActionResult<Flight>> GetFlight(int id)
        {
            // Busca el vuelo por ID de manera asíncrona.
            var flight = await _context.Flights.FindAsync(id);

            // Si no se encuentra el vuelo, retorna NotFound.
            if (flight == null)
            {
                return NotFound(); // Devuelve un resultado 404 Not Found.
            }

            return flight; // Devuelve el vuelo encontrado.
        }

        // Método para crear un nuevo vuelo.
        [HttpPost] // Indica que este método maneja las solicitudes POST en la ruta base del controlador.
        public async Task<ActionResult<Flight>> PostFlight(Flight flight)
        {
            // Agrega el nuevo vuelo al contexto.
            _context.Flights.Add(flight);
            // Guarda los cambios en la base de datos de manera asíncrona.
            await _context.SaveChangesAsync();

            // Devuelve un resultado 201 Created, incluyendo la ubicación del nuevo recurso.
            return CreatedAtAction(nameof(GetFlight), new { id = flight.Id }, flight);
        }

        // Método para actualizar un vuelo existente.
        [HttpPut("{id}")] // Indica que este método maneja las solicitudes PUT en la ruta "api/Flight/{id}".
        public async Task<IActionResult> PutFlight(int id, Flight flight)
        {
            // Verifica si el ID del vuelo coincide con el ID de la ruta.
            if (id != flight.Id)
            {
                return BadRequest(); // Devuelve un resultado 400 Bad Request si los IDs no coinciden.
            }

            // Marca el vuelo como modificado en el contexto.
            _context.Entry(flight).State = EntityState.Modified;

            try
            {
                // Guarda los cambios en la base de datos de manera asíncrona.
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) // Captura excepciones de concurrencia.
            {
                // Verifica si el vuelo aún existe en la base de datos.
                if (!_context.Flights.Any(e => e.Id == id))
                {
                    return NotFound(); // Devuelve un resultado 404 Not Found si no se encuentra.
                }
                else
                {
                    throw; // Si el vuelo existe, vuelve a lanzar la excepción.
                }
            }

            return NoContent(); // Devuelve un resultado 204 No Content si la actualización fue exitosa.
        }

        // Método para eliminar un vuelo existente.
        [HttpDelete("{id}")] // Indica que este método maneja las solicitudes DELETE en la ruta "api/Flight/{id}".
        public async Task<IActionResult> DeleteFlight(int id)
        {
            // Busca el vuelo por ID de manera asíncrona.
            var flight = await _context.Flights.FindAsync(id);
            // Si no se encuentra el vuelo, retorna NotFound.
            if (flight == null)
            {
                return NotFound(); // Devuelve un resultado 404 Not Found.
            }

            // Elimina el vuelo del contexto.
            _context.Flights.Remove(flight);
            // Guarda los cambios en la base de datos de manera asíncrona.
            await _context.SaveChangesAsync();

            return NoContent(); // Devuelve un resultado 204 No Content si la eliminación fue exitosa.
        }
    }
}
