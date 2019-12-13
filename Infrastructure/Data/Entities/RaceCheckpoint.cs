using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Data.Entities
{
    class RaceCheckpoint
    {
        public int RaceId { get; set; }
        public Race Race { get; set; }
        public int NumberInOrder { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
