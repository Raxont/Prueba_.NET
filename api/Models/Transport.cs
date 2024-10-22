namespace api.Models // Definición del espacio de nombres para los modelos de datos de la API.
{
    public class Transport // Clase que representa un medio de transporte asociado a un vuelo.
    {
        public int Id { get; set; } // Propiedad que representa el identificador único del transporte.

        public required string FlightCarrier { get; set; } // Propiedad que representa el transportador del vuelo (por ejemplo, el nombre de la aerolínea). Se marca como "required", indicando que es obligatorio.

        public required string FlightNumber { get; set; } // Propiedad que representa el número del vuelo (por ejemplo, "8040"). También se marca como "required".
    }
}
