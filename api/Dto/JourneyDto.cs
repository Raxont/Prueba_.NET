namespace api.Dto // Definición del espacio de nombres para los Data Transfer Objects (DTOs) de la API.
{
    public class JourneyDto // Clase que representa un objeto de transferencia de datos para un viaje.
    {
        public required string Origin { get; set; } // Propiedad que representa la estación de origen del viaje. Se marca como "required", indicando que es obligatorio.

        public required string Destination { get; set; } // Propiedad que representa la estación de destino del viaje. También se marca como "required".

        public required double Price { get; set; } // Propiedad que representa el precio del viaje. Se marca como "required", indicando que es obligatorio.

        public List<FlightDto> Flights { get; set; } = new List<FlightDto>(); // Propiedad que representa una lista de objetos FlightDto asociados a este viaje. Se inicializa como una lista vacía.
    }
}
