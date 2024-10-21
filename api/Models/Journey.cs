namespace api.Models
{
    public class Journey // Cambia Jorney a Journey
    {
        public int Id { get; set; }
        public List<Flight> Flights { get; set; }
        public required string Origin { get; set; } 
        public required string Destination { get; set; }
        public required double Price { get; set; } 
        public Journey()
        {
            Flights = [];
        }
    }
}