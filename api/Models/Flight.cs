namespace api.Models
{
    public class Flight
    {
        public int Id { get; set; } 
        public required string Origin { get; set; } 
        public required string Destination { get; set; } 
        public required double Price { get; set; } 
        public required int TransportId { get; set; } 
        public required Transport Transport { get; set; }
    }
}