using Infrastructure.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<RaceMap> RaceMaps { get; set; }
        public DbSet<RaceCheckpoint> RaceCheckpoints { get; set; }
        public DbSet<RaceResult> RaceResults { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<RaceCheckpoint>().HasKey(x => new { x.RaceId, x.NumberInOrder });
        }
    }
}
