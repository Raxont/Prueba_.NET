namespace api.Dto
{
    // Dto/JourneyRequestDto.cs
    public class JourneyRequestDto
    {
        public required string Origin { get; set; }
        public required string Destination { get; set; }
    }
}