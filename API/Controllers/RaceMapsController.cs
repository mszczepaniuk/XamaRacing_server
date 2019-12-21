using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Data;
using Infrastructure.Data.Entities;
using API.BindingModels;

namespace API.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]
    public class RaceMapsController : ControllerBase
    {
        private readonly AppDbContext appDbContext;

        public RaceMapsController(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        // GET: v1/RaceMaps
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RaceMap>>> GetRaceMaps(int? offset, int? count)
        {
            offset ??= 0;
            if (count == null || count > 30) { count = 30; }
            var raceMaps = await appDbContext.RaceMaps.Include(x => x.RaceCheckpoints)
                .OrderBy(x => x.CreatedDate)
                .Skip(offset.Value)
                .Take(count.Value)
                .ToArrayAsync();

            return raceMaps;
        }

        // GET: v1/RaceMaps/1/RaceResults
        [HttpGet("{id}/RaceResults")]
        public async Task<ActionResult<IEnumerable<RaceResult>>> GetRaceMapsResults(int id, int? offset, int? count)
        {
            offset ??= 0;
            if (count == null || count > 30) { count = 30; }

            var raceResults = await appDbContext.RaceResults.Where(x => x.RaceId == id)
                .OrderByDescending(x => x.Time)
                .Skip(offset.Value)
                .Take(count.Value)
                .ToArrayAsync();
            return raceResults;
        }

        // GET: v1/RaceMaps/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RaceMap>> GetRaceMap(int id)
        {
            var raceMap = await appDbContext.RaceMaps.Include(x => x.RaceCheckpoints)
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            if (raceMap == null)
            {
                return NotFound();
            }

            return raceMap;
        }

        // PUT: v1/RaceMaps/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<ActionResult<RaceMap>> PutRaceMap(int id, RaceMapBindingModel raceMapBindingModel)
        {
            var raceMap = await appDbContext.RaceMaps.Include(x => x.RaceCheckpoints)
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            if (raceMap == null) { return NotFound(); }
            raceMap.Name = raceMapBindingModel.Name;
            raceMap.Description = raceMapBindingModel.Description;

            if (raceMap.RaceCheckpoints != raceMapBindingModel.RaceCheckpoints)
            {
                foreach (var checkpoint in raceMap.RaceCheckpoints)
                {
                    appDbContext.RaceCheckpoints.Remove(checkpoint);
                }
                raceMap.RaceCheckpoints = raceMapBindingModel.RaceCheckpoints;
            }
            raceMap.UpdatedDate = DateTime.Now;

            appDbContext.RaceMaps.Update(raceMap);
            await appDbContext.SaveChangesAsync();
            return raceMap;
        }

        // POST: v1/RaceMaps
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<RaceMap>> PostRaceMap(RaceMapBindingModel raceMapBindingModel)
        {
            var createdDate = DateTime.Now;
            var raceMap = new RaceMap
            {
                Name = raceMapBindingModel.Name,
                Description = raceMapBindingModel.Description,
                RaceCheckpoints = raceMapBindingModel.RaceCheckpoints,
                CreatedDate = createdDate,
                UpdatedDate = createdDate
            };
            appDbContext.RaceMaps.Add(raceMap);
            await appDbContext.SaveChangesAsync();
            return CreatedAtAction("GetRaceMap", new { id = raceMap.Id }, raceMap);
        }


        // DELETE: v1/RaceMaps/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<RaceMap>> DeleteRaceMap(int id)
        {
            var raceMap = await appDbContext.RaceMaps.FindAsync(id);
            if (raceMap == null)
            {
                return NotFound();
            }

            appDbContext.RaceMaps.Remove(raceMap);
            await appDbContext.SaveChangesAsync();

            return Ok();
        }
    }
}
