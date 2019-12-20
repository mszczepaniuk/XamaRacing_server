using Infrastructure.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.BindingModels
{
    public class RaceMapBindingModel
    {
        public string Name { get; set; }
        public int CreatorId { get; set; }
        public IList<RaceCheckpoint> RaceCheckpoints { get; set; }
    }
}
