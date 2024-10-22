using System.Text.Json; // Espacio de nombres para trabajar con JSON.
using System.Text.Json.Serialization; // Espacio de nombres para la serialización/deserialización de JSON.
using api.Dto; // Espacio de nombres para los DTOs (Data Transfer Objects) utilizados en la aplicación.
using api.Models; // Espacio de nombres para los modelos de datos de la aplicación.
using api.Data; // Espacio de nombres para acceder a la base de datos.
using Microsoft.EntityFrameworkCore; // Espacio de nombres para usar Entity Framework Core.
using Microsoft.Extensions.Caching.Memory; // Espacio de nombres para trabajar con la memoria caché.
using Microsoft.Extensions.Logging; // Espacio de nombres para el registro de eventos.

namespace api.Services // Definición del espacio de nombres para los servicios de la API.
{
    public class FlightService // Clase que gestiona la lógica relacionada con los vuelos.
    {
        private readonly HttpClient _httpClient; // Cliente HTTP para realizar solicitudes a APIs externas.
        private readonly AppDbContext _dbContext; // Contexto de la base de datos para acceder a los datos.
        private readonly IMemoryCache _cache; // Caché en memoria para almacenar datos temporalmente.
        private readonly ILogger<FlightService> _logger; // Registrador para registrar eventos y errores.
        
        private const string FlightsCacheKey = "flightsCache"; // Clave para almacenar vuelos en la caché.
        private readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(30); // Duración de la caché.

        // Constructor de la clase FlightService
        public FlightService(HttpClient httpClient, AppDbContext dbContext, IMemoryCache cache, ILogger<FlightService> logger)
        {
            _httpClient = httpClient; // Asignar el cliente HTTP.
            _dbContext = dbContext; // Asignar el contexto de la base de datos.
            _cache = cache; // Asignar la caché en memoria.
            _logger = logger; // Asignar el registrador.
        }

        // Método asíncrono para obtener la lista de vuelos
        public async Task<List<Flight>> GetFlightsAsync()
        {
            // Asegúrate de que _cache no es null
            if (_cache == null)
            {
                throw new InvalidOperationException("Cache is not initialized."); // Lanzar excepción si la caché no está inicializada.
            }

            // Lista para almacenar los vuelos
            var flights = new List<Flight>();

            // Intenta obtener los vuelos desde la caché
            if (_cache.TryGetValue(FlightsCacheKey, out string? cachedFlightsJson))
            {
                _logger.LogInformation("Flights retrieved from cache."); // Registro de uso del caché

                // Verificar que el valor en caché no sea nulo antes de deserializar
                if (!string.IsNullOrEmpty(cachedFlightsJson))
                {
                    var options = new JsonSerializerOptions
                    {
                        AllowTrailingCommas = true, // Permitir comas al final de listas y objetos en JSON.
                        ReferenceHandler = ReferenceHandler.Preserve // Preservar referencias durante la deserialización.
                    };

                    // Deserializar el JSON almacenado en caché de vuelta a una lista de FlightDto
                    var cachedFlightDtos = JsonSerializer.Deserialize<List<FlightDto>>(cachedFlightsJson, options) ?? new List<FlightDto>();

                    // Procesar cada FlightDto del caché para crear los objetos Flight
                    foreach (var dto in cachedFlightDtos)
                    {
                        // Obtener o crear el Transport
                        var transport = await GetOrCreateTransportAsync(dto); // Método para obtener o crear el objeto Transport.

                        // Crear Flight y asignar TransportId
                        var flight = new Flight
                        {
                            Origin = dto.DepartureStation, // Asignar la estación de origen.
                            Destination = dto.ArrivalStation, // Asignar la estación de destino.
                            Price = dto.Price, // Asignar el precio del vuelo.
                            TransportId = transport.Id, // Asignar el ID del transporte.
                            Transport = transport // Asignar el objeto Transport.
                        };

                        flights.Add(flight); // Agregar el vuelo a la lista.
                    }
                }
            }
            else
            {
                _logger.LogInformation("Fetching flights from external API."); // Registro de la obtención de vuelos desde la API externa.
                var response = await _httpClient.GetStringAsync("https://bitecingcom.ipage.com/testapi/avanzado.js"); // Obtener la respuesta de la API.
                
                var options = new JsonSerializerOptions
                {
                    AllowTrailingCommas = true, // Permitir comas al final de listas y objetos en JSON.
                    ReferenceHandler = ReferenceHandler.Preserve // Preservar referencias durante la deserialización.
                };

                // Deserializar la respuesta de la API en una lista de FlightDto
                var flightDtos = JsonSerializer.Deserialize<List<FlightDto>>(response, options) ?? new List<FlightDto>();

                // Procesar cada FlightDto de la API para crear los objetos Flight
                foreach (var dto in flightDtos)
                {
                    // Obtener o crear el Transport
                    var transport = await GetOrCreateTransportAsync(dto); // Método para obtener o crear el objeto Transport.

                    // Crear Flight y asignar TransportId
                    var flight = new Flight
                    {
                        Origin = dto.DepartureStation, // Asignar la estación de origen.
                        Destination = dto.ArrivalStation, // Asignar la estación de destino.
                        Price = dto.Price, // Asignar el precio del vuelo.
                        TransportId = transport.Id, // Asignar el ID del transporte.
                        Transport = transport // Asignar el objeto Transport.
                    };

                    flights.Add(flight); // Agregar el vuelo a la lista.
                }

                // Serializar la lista de FlightDto a JSON antes de almacenarla en caché
                var flightsJson = JsonSerializer.Serialize(flightDtos, options); // Convertir la lista de vuelos en JSON.
                
                // Almacenar los vuelos en la caché con la duración especificada
                _cache.Set(FlightsCacheKey, flightsJson, CacheDuration); // Almacenar el JSON en la caché.
                _logger.LogInformation("Flights cached successfully."); // Registro de almacenamiento en caché.
            }

            return flights; // Devolver la lista de vuelos.
        }

        // Método privado asíncrono para obtener o crear un objeto Transport
        private async Task<Transport> GetOrCreateTransportAsync(FlightDto dto)
        {
            // Buscar si ya existe el Transport
            var transport = await _dbContext.Transports
                .FirstOrDefaultAsync(t => t.FlightCarrier == dto.FlightCarrier && t.FlightNumber == dto.FlightNumber);

            if (transport == null) // Si no existe, crear uno nuevo
            {
                transport = new Transport
                {
                    FlightCarrier = dto.FlightCarrier, // Asignar el transportador de vuelo.
                    FlightNumber = dto.FlightNumber // Asignar el número de vuelo.
                };

                // Guardar en la base de datos
                await _dbContext.Transports.AddAsync(transport); // Agregar el nuevo transport a la base de datos.
                await _dbContext.SaveChangesAsync(); // Guardar cambios en la base de datos.
            }

            return transport; // Devolver el objeto Transport.
        }
    }
}
