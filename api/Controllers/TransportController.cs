using Microsoft.AspNetCore.Mvc; // Importa el espacio de nombres para las funcionalidades de ASP.NET Core MVC.
using api.Data; // Importa el espacio de nombres que contiene el contexto de la base de datos.
using api.Models; // Importa el espacio de nombres que contiene los modelos de la API.
using Microsoft.EntityFrameworkCore; // Importa el espacio de nombres para las funcionalidades de Entity Framework Core.

namespace api.Controllers // Definición del espacio de nombres para los controladores de la API.
{
    [ApiController] // Indica que esta clase es un controlador de API y habilita características de API específicas.
    [Route("api/[controller]")] // Define la ruta base para las solicitudes, donde [controller] se reemplaza por el nombre del controlador (Transport).
    public class TransportController : ControllerBase // Clase que maneja las operaciones relacionadas con el transporte.
    {
        private readonly AppDbContext _context; // Contexto de la base de datos inyectado para acceder a las entidades.

        // Constructor que recibe el contexto de la base de datos.
        public TransportController(AppDbContext context)
        {
            _context = context; // Inicializa el contexto de la base de datos.
        }

        // Método para obtener la lista de transportes.
        [HttpGet] // Indica que este método maneja las solicitudes GET.
        public async Task<ActionResult<IEnumerable<Transport>>> GetTransports()
        {
            return await _context.Transports.ToListAsync(); // Devuelve todos los transportes en forma de lista asíncrona.
        }

        // Método para obtener un transporte específico por su ID.
        [HttpGet("{id}")] // Indica que este método maneja las solicitudes GET para un transporte específico.
        public async Task<ActionResult<Transport>> GetTransport(int id)
        {
            var transport = await _context.Transports.FindAsync(id); // Busca el transporte por ID.

            if (transport == null) // Si no se encuentra el transporte.
            {
                return NotFound(); // Devuelve un resultado 404 Not Found.
            }

            return transport; // Devuelve el transporte encontrado.
        }

        // Método para crear un nuevo transporte.
        [HttpPost] // Indica que este método maneja las solicitudes POST.
        public async Task<ActionResult<Transport>> PostTransport(Transport transport)
        {
            _context.Transports.Add(transport); // Agrega el nuevo transporte al contexto.
            await _context.SaveChangesAsync(); // Guarda los cambios en la base de datos.

            return CreatedAtAction(nameof(GetTransport), new { id = transport.Id }, transport); // Devuelve un resultado 201 Created con la ubicación del nuevo recurso.
        }

        // Método para actualizar un transporte existente.
        [HttpPut("{id}")] // Indica que este método maneja las solicitudes PUT para un transporte específico.
        public async Task<IActionResult> PutTransport(int id, Transport transport)
        {
            if (id != transport.Id) // Verifica si el ID proporcionado coincide con el del transporte.
            {
                return BadRequest(); // Devuelve un resultado 400 Bad Request si hay discrepancias.
            }

            _context.Entry(transport).State = EntityState.Modified; // Marca el transporte como modificado.

            try
            {
                await _context.SaveChangesAsync(); // Intenta guardar los cambios en la base de datos.
            }
            catch (DbUpdateConcurrencyException) // Captura excepciones de concurrencia.
            {
                if (!_context.Transports.Any(e => e.Id == id)) // Verifica si el transporte existe.
                {
                    return NotFound(); // Devuelve 404 Not Found si no existe.
                }
                else
                {
                    throw; // Si hay otro error, vuelve a lanzar la excepción.
                }
            }

            return NoContent(); // Devuelve un resultado 204 No Content si la actualización fue exitosa.
        }

        // Método para eliminar un transporte.
        [HttpDelete("{id}")] // Indica que este método maneja las solicitudes DELETE para un transporte específico.
        public async Task<IActionResult> DeleteTransport(int id)
        {
            var transport = await _context.Transports.FindAsync(id); // Busca el transporte por ID.
            if (transport == null) // Si no se encuentra el transporte.
            {
                return NotFound(); // Devuelve un resultado 404 Not Found.
            }

            _context.Transports.Remove(transport); // Elimina el transporte del contexto.
            await _context.SaveChangesAsync(); // Guarda los cambios en la base de datos.

            return NoContent(); // Devuelve un resultado 204 No Content si la eliminación fue exitosa.
        }
    }
}
