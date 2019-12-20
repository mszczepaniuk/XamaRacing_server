using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Data.Entities
{
    public class RaceResult
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        // TODO: public AppliactionUser User { get; set; }
        public int RaceId { get; set; }
        public RaceMap Race { get; set; }
        public double TimeInSeconds { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}