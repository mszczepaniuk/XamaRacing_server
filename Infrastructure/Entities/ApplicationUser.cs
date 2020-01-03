using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<RaceMap> CreatedMaps { get; set; }
        public ICollection<RaceResult> RaceResults { get; set; }
        public RefreshToken RefreshToken { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
