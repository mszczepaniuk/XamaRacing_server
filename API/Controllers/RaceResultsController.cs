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
    public class RaceResultsController : ControllerBase
    {
        private readonly AppDbContext appDbContext;

        public RaceResultsController(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        // GET: v1/RaceResults
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RaceResult>>> GetRaceResults(int? offset, int? count, int? mapId, string userName)
        {
            IQueryable<RaceResult> query = appDbContext.RaceResults;
            offset ??= 0;
            if (count == null || count > 30) { count = 30; }
            if (mapId != null) { query = query.Where(x => x.RaceId == mapId); }

            // TODO: Add user filtering
            var raceResults = await query.OrderBy(x => x.Time)
                .Skip(offset.Value)
                .Take(count.Value)
                .ToArrayAsync();

            return raceResults;
        }

        // GET: v1/RaceResults/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RaceResult>> GetRaceResult(int id)
        {
            var raceResult = await appDbContext.RaceResults.FindAsync(id);

            if (raceResult == null)
            {
                return NotFound();
            }

            return raceResult;
        }


        // POST: v1/RaceResults
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<RaceResult>> PostRaceResult(RaceResultBindingModel raceResultBindingModel)
        {
            var raceResult = new RaceResult
            {
                RaceId = raceResultBindingModel.RaceId,
                UserId = raceResultBindingModel.UserId,
                Time = raceResultBindingModel.Time,
                CreatedDate = DateTime.Now
            };
            appDbContext.RaceResults.Add(raceResult);
            await appDbContext.SaveChangesAsync();

            return CreatedAtAction("GetRaceResult", new { id = raceResult.Id }, raceResult);
        }

        // DELETE: v1/RaceResults/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<RaceResult>> DeleteRaceResult(int id)
        {
            var raceResult = await appDbContext.RaceResults.FindAsync(id);
            if (raceResult == null)
            {
                return NotFound();
            }

            appDbContext.RaceResults.Remove(raceResult);
            await appDbContext.SaveChangesAsync();

            return Ok();
        }
    }
}
