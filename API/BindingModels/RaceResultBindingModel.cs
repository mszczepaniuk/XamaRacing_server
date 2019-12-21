using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.BindingModels
{
    public class RaceResultBindingModel
    {
        public string Nickname { get; set; }
        public int RaceId { get; set; }
        public TimeSpan Time { get; set; }
    }
}
