namespace api.Models // Definición del espacio de nombres para los modelos de datos de la API.
{
    public class Journey // Clase que representa un viaje.
    {
        public int Id { get; set; } // Propiedad que representa el identificador único del viaje.

        public required string Origin { get; set; } // Propiedad que representa la estación de origen del viaje. Se marca como "required", indicando que es obligatorio.

        public required string Destination { get; set; } // Propiedad que representa la estación de destino del viaje. También se marca como "required".

        public required double Price { get; set; } // Propiedad que representa el precio del viaje. Se marca como "required", indicando que es obligatorio.

        public List<JourneyFlight> JourneyFlights { get; set; } // Propiedad que representa la lista de vuelos asociados a este viaje.

        // Constructor de la clase Journey
        public Journey()
        {
            JourneyFlights = new List<JourneyFlight>(); // Inicializa la lista de vuelos asociados a este viaje.
        }
    }
}
