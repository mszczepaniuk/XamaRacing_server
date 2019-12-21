using Infrastructure.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Infrastructure.Data.Entities
{
    public class RaceMap
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public IList<RaceCheckpoint> RaceCheckpoints { get; set; }
        public IList<RaceResult> RaceResults { get; set; }
    }
}