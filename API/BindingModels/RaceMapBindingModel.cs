using Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.BindingModels
{
    public class RaceMapBindingModel
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        public IList<RaceCheckpoint> RaceCheckpoints { get; set; }
    }
}