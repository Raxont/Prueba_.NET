using System; // Espacio de nombres para las funcionalidades básicas del sistema.
using System.Collections.Generic; // Espacio de nombres para usar colecciones genéricas.
using System.Linq; // Espacio de nombres para usar LINQ (Language Integrated Query).
using System.Threading.Tasks; // Espacio de nombres para manejar tareas asíncronas.

namespace api.Dto // Definición del espacio de nombres para los Data Transfer Objects (DTOs) de la API.
{
    public class TransportDto // Clase que representa un objeto de transferencia de datos para el transporte.
    {
        public required string FlightCarrier { get; set; } // Propiedad que representa el transportador del vuelo (por ejemplo, el nombre de la aerolínea). Se marca como "required", indicando que es obligatorio.

        public required string FlightNumber { get; set; } // Propiedad que representa el número del vuelo (por ejemplo, "8040"). También se marca como "required".
    }
}
