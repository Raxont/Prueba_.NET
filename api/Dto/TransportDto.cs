using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dto
{
    public class TransportDto
    {
        public required string FlightCarrier { get; set; }
        public required string FlightNumber { get; set; }
    }
}