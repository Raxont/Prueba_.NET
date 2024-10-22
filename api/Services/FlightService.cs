using System.Text.Json;
using System.Text.Json.Serialization;
using api.Dto;
using api.Models;
using api.Data;
using Microsoft.EntityFrameworkCore; 
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace api.Services
{
    public class FlightService
    {
        private readonly HttpClient _httpClient;
        private readonly AppDbContext _dbContext;
        private readonly IMemoryCache _cache;
        private readonly ILogger<FlightService> _logger;
        private const string FlightsCacheKey = "flightsCache";
        private readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(30);

        public FlightService(HttpClient httpClient, AppDbContext dbContext, IMemoryCache cache, ILogger<FlightService> logger)
        {
            _httpClient = httpClient;
            _dbContext = dbContext;
            _cache = cache;
            _logger = logger;
        }


        public async Task<List<Flight>> GetFlightsAsync()
        {
            // Asegúrate de que _cache no es null
            if (_cache == null)
            {
                throw new InvalidOperationException("Cache is not initialized.");
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
                        AllowTrailingCommas = true,
                        ReferenceHandler = ReferenceHandler.Preserve 
                    };

                    // Deserializar el JSON almacenado en caché de vuelta a una lista de FlightDto
                    var cachedFlightDtos = JsonSerializer.Deserialize<List<FlightDto>>(cachedFlightsJson, options) ?? new List<FlightDto>();

                    // Procesar cada FlightDto del caché para crear los objetos Flight
                    foreach (var dto in cachedFlightDtos)
                    {
                        // Obtener o crear el Transport
                        var transport = await GetOrCreateTransportAsync(dto);

                        // Crear Flight y asignar TransportId
                        var flight = new Flight
                        {
                            Origin = dto.DepartureStation,
                            Destination = dto.ArrivalStation,
                            Price = dto.Price,
                            TransportId = transport.Id, 
                            Transport = transport 
                        };

                        flights.Add(flight);
                    }
                }
            }
            else
            {
                _logger.LogInformation("Fetching flights from external API.");
                var response = await _httpClient.GetStringAsync("https://bitecingcom.ipage.com/testapi/avanzado.js");
                var options = new JsonSerializerOptions
                {
                    AllowTrailingCommas = true,
                    ReferenceHandler = ReferenceHandler.Preserve 
                };

                // Deserializar la respuesta de la API en una lista de FlightDto
                var flightDtos = JsonSerializer.Deserialize<List<FlightDto>>(response, options) ?? new List<FlightDto>();

                // Procesar cada FlightDto de la API para crear los objetos Flight
                foreach (var dto in flightDtos)
                {
                    // Obtener o crear el Transport
                    var transport = await GetOrCreateTransportAsync(dto);

                    // Crear Flight y asignar TransportId
                    var flight = new Flight
                    {
                        Origin = dto.DepartureStation,
                        Destination = dto.ArrivalStation,
                        Price = dto.Price,
                        TransportId = transport.Id, 
                        Transport = transport 
                    };

                    flights.Add(flight);
                }

                // Serializar la lista de FlightDto a JSON antes de almacenarla en caché
                var flightsJson = JsonSerializer.Serialize(flightDtos, options);
                // Almacenar los vuelos en la caché con la duración especificada
                _cache.Set(FlightsCacheKey, flightsJson, CacheDuration);
                _logger.LogInformation("Flights cached successfully."); // Registro de almacenamiento en caché
            }

            return flights;
        }

        private async Task<Transport> GetOrCreateTransportAsync(FlightDto dto)
        {
            // Buscar si ya existe el Transport
            var transport = await _dbContext.Transports
                .FirstOrDefaultAsync(t => t.FlightCarrier == dto.FlightCarrier && t.FlightNumber == dto.FlightNumber);

            if (transport == null)
            {
                // Si no existe, crear uno nuevo
                transport = new Transport
                {
                    FlightCarrier = dto.FlightCarrier,
                    FlightNumber = dto.FlightNumber
                };

                // Guardar en la base de datos
                await _dbContext.Transports.AddAsync(transport);
                await _dbContext.SaveChangesAsync();
            }

            return transport;
        }
    }
}
