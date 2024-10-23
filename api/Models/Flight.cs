namespace api.Models // Definición del espacio de nombres para los modelos de datos de la API.
{
    public class Flight // Clase que representa un vuelo.
    {
        public int Id { get; set; } // Propiedad que representa el identificador único del vuelo.

        public required string Origin { get; set; } // Propiedad que representa la estación de origen del vuelo. Se marca como "required", indicando que es obligatorio.

        public required string Destination { get; set; } // Propiedad que representa la estación de destino del vuelo. También se marca como "required".

        public required double Price { get; set; } // Propiedad que representa el precio del vuelo. Se marca como "required", indicando que es obligatorio.

        public int TransportId { get; set; } // Propiedad que representa el identificador del transporte asociado al vuelo. Se marca como "required".

        public Transport? Transport { get; set; } // Propiedad que representa el objeto Transport asociado a este vuelo. También se marca como "required".
    }
}
