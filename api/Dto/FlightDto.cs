namespace api.Dto
{
    public class FlightDto
    {
        public required string DepartureStation { get; set; }
        public required string ArrivalStation { get; set; }
        public required double Price { get; set; }
        public required string FlightCarrier { get; set; }
        public required string FlightNumber { get; set; }
    }
}