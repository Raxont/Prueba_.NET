namespace api.Models
{
    public class Journey 
    {
        public int Id { get; set; }
        public required string Origin { get; set; } 
        public required string Destination { get; set; }
        public required double Price { get; set; } 

        public List<JourneyFlight> JourneyFlights { get; set; }

        public Journey()
        {
            JourneyFlights = new List<JourneyFlight>();
        }
    }
}
