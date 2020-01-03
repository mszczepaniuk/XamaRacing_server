using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Entities
{
    public class RefreshToken
    {
        public ApplicationUser User { get; set; }
        public string UserId { get; set; }
        public string Value { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}