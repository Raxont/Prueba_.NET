using System.Text.Json;
using System.Text.Json.Serialization;
using api.Dto;
using api.Models;
using api.Data;
using Microsoft.EntityFrameworkCore; 
using Microsoft.Extensions.Caching.Memory;

namespace api.Services
{
    public class FlightService
    {
        private readonly HttpClient _httpClient;
        private readonly AppDbContext _dbContext;
        private readonly IMemoryCache _cache;
        private const string FlightsCacheKey = "flightsCache";
        private readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);

        public FlightService(HttpClient httpClient, AppDbContext dbContext, IMemoryCache cache)
        {
            _httpClient = httpClient;
            _dbContext = dbContext;
            _cache = cache;
        }


        public async Task<List<Flight>> GetFlightsAsync()
        {
             // Asegúrate de que _cache no es null
            if (_cache == null)
            {
                throw new InvalidOperationException("Cache is not initialized.");
            }

            // Intenta obtener los vuelos desde la caché
            if (_cache.TryGetValue(FlightsCacheKey, out List<Flight>? cachedFlights))
            {
                return cachedFlights ?? [];
            }
            
            var response = await _httpClient.GetStringAsync("https://bitecingcom.ipage.com/testapi/avanzado.js");
            var options = new JsonSerializerOptions
            {
                AllowTrailingCommas = true,
                ReferenceHandler = ReferenceHandler.Preserve 
            };

            var flightDtos = JsonSerializer.Deserialize<List<FlightDto>>(response, options) ?? [];

            var flights = new List<Flight>();

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
            // Almacenar los vuelos en la caché con la duración especificada
            _cache.Set(FlightsCacheKey, flights, CacheDuration);
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
