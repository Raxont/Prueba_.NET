namespace api.Dto // Definición del espacio de nombres para los Data Transfer Objects (DTOs) de la API.
{
    public class FlightDto // Clase que representa un objeto de transferencia de datos para un vuelo.
    {
        public required string DepartureStation { get; set; } // Propiedad que representa la estación de salida del vuelo. Se marca como "required", indicando que es obligatorio.

        public required string ArrivalStation { get; set; } // Propiedad que representa la estación de llegada del vuelo. También se marca como "required".

        public required double Price { get; set; } // Propiedad que representa el precio del vuelo. Se marca como "required", indicando que es obligatorio.

        public required string FlightCarrier { get; set; } // Propiedad que representa la aerolínea que opera el vuelo. Se marca como "required".

        public required string FlightNumber { get; set; } // Propiedad que representa el número del vuelo (por ejemplo, "8040"). También se marca como "required".
    }
}
