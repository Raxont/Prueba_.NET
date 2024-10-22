namespace api.Models // Definición del espacio de nombres para los modelos de datos de la API.
{
    public class JourneyFlight // Clase que representa la relación entre un viaje y un vuelo.
    {
        public int JourneyId { get; set; } // Propiedad que representa el identificador único del viaje asociado.

        public required Journey Journey { get; set; } // Propiedad que representa el objeto Journey asociado a este viaje. Se marca como "required", indicando que es obligatorio.

        public int FlightId { get; set; } // Propiedad que representa el identificador único del vuelo asociado.

        public required Flight Flight { get; set; } // Propiedad que representa el objeto Flight asociado a este vuelo. También se marca como "required".
    }
}
