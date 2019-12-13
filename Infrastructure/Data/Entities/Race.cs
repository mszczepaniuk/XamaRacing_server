using Infrastructure.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Data.Entities
{
    class Race
    {
        public int Id { get; set; }
        public string Name { get; set; }
        // TODO: public AppliactionUser Creator { get; set; }
        public int CreatorId { get; set; }
        public DateTime CreationDate { get; set; }
        public IList<RaceCheckpoint> RaceCheckpoints { get; set; }
    }
}