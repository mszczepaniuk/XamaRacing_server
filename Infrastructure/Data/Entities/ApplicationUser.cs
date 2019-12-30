using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Data.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<RaceMap> CreatedMaps { get; set; }
        public ICollection<RaceResult> RaceResults { get; set; }
    }
}
