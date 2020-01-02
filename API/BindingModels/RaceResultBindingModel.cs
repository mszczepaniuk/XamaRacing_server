using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.BindingModels
{
    public class RaceResultBindingModel
    {
        [Required]
        public int? RaceId { get; set; }
        [Required]
        public string Time { get; set; }
    }
}
