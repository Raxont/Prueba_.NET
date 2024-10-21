// Services/FlightService.cs
using System.Text.Json;
using api.Dto;
using api.Models;

namespace api.Services{
    public class FlightService
    {
        private readonly HttpClient _httpClient;

        public FlightService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Flight>> GetFlightsAsync()
        {
            var response = await _httpClient.GetStringAsync("https://bitecingcom.ipage.com/testapi/avanzado.js");
            
            var options = new JsonSerializerOptions
            {
                AllowTrailingCommas = true // Permitir comas finales en el JSON
            };
            
            var flightDtos = JsonSerializer.Deserialize<List<FlightDto>>(response, options);
            if (flightDtos == null) return new List<Flight>();

            return flightDtos.Select(dto => new Flight
            {
                Origin = dto.DepartureStation,
                Destination = dto.ArrivalStation,
                Price = dto.Price,
                Transport = new Transport
                {
                    FlightCarrier = dto.FlightCarrier,
                    FlightNumber = dto.FlightNumber
                }
            }).ToList();
        }
    }

}
