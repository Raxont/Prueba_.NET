namespace api.Models
{
    public class Flight
    {
        public int Id { get; set; } 
        public required string Origin { get; set; } 
        public required string Destination { get; set; } 
        public double Price { get; set; } 
        public int TransportId { get; set; } 
        public required Transport Transport { get; set; }
    }
}