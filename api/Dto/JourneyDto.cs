namespace api.Dto
{
    public class JourneyDto
    {
        public required int Id { get; set; }
        public required string Origin { get; set; }
        public required string Destination { get; set; }
        public required double Price { get; set; }
        public List<FlightDto> Flights { get; set; } = [];
    }
}