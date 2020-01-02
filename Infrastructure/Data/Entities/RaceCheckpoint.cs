using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace Infrastructure.Data.Entities
{
    public class RaceCheckpoint
    { 
        public int RaceId { get; set; }
        [JsonIgnore]
        public RaceMap Race { get; set; }
        [Required]
        public int NumberInOrder { get; set; }
        [Required]
        public double Latitude { get; set; }
        [Required]
        public double Longitude { get; set; }
    }
}
