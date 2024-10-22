namespace api.Models
{
    public class JourneyFlight
    {
        public int JourneyId { get; set; }
        public required Journey Journey { get; set; }

        public int FlightId { get; set; }
        public required Flight Flight { get; set; }
    }
}
