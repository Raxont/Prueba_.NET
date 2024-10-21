namespace api.Models
{
    public class Transport
    {
        public int Id { get; set; }
        public required string FlightCarrier { get; set; }
        public required string FlightNumber { get; set; }
    }
}