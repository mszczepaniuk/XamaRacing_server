using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Infrastructure.Data.Entities
{
    public class RaceCheckpoint
    { 
        public int RaceId { get; set; }
        [JsonIgnore]
        public RaceMap Race { get; set; }
        public int NumberInOrder { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
