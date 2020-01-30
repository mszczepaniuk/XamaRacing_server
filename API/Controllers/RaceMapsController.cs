using API.BindingModels;
using Infrastructure.Data;
using Infrastructure.Entities;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]
    [Authorize]
    public class RaceMapsController : ControllerBase
    {
        private readonly AppDbContext appDbContext;
        private readonly ITokenService tokenService;

        public RaceMapsController(AppDbContext appDbContext,
            ITokenService tokenService)
        {
            this.appDbContext = appDbContext;
            this.tokenService = tokenService;
        }

        // TODO: Search by location
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
        public async Task<ActionResult<IEnumerable<object>>> GetRaceMapsResults(int id, int? offset, int? count)
        {
            offset ??= 0;
            if (count == null || count > 30) { count = 30; }
            var raceResults = await appDbContext.RaceResults.Where(x => x.RaceId == id)
                .OrderBy(x => x.Time)
                .Skip(offset.Value)
                .Take(count.Value)
                .Select(raceResult => new { raceResult, raceResult.User.UserName })
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
        [HttpPut("{id}")]
        public async Task<ActionResult<RaceMap>> PutRaceMap(int id, RaceMapBindingModel raceMapBindingModel)
        {
            if (!ModelState.IsValid) { return BadRequest(raceMapBindingModel); }

            var raceMap = await appDbContext.RaceMaps.Include(x => x.RaceCheckpoints)
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();
            if (raceMap == null) { return NotFound(); }

            var contextUserId = tokenService.GetUserId(HttpContext.User);
            if (contextUserId == null) { return BadRequest(new { message = "Couldnt determine user from context" }); }
            if (contextUserId != raceMap.UserId) { return Unauthorized(); }

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
        // TODO: Add race checkpoints validation
        [HttpPost]
        public async Task<ActionResult<RaceMap>> PostRaceMap(RaceMapBindingModel raceMapBindingModel)
        {
            var test = TryValidateModel(raceMapBindingModel.RaceCheckpoints);
            if (!ModelState.IsValid) { return BadRequest(raceMapBindingModel); }

            var contextUserId = tokenService.GetUserId(HttpContext.User);
            if (contextUserId == null) { return BadRequest(new { message = "Couldnt determine user from context" }); }

            var createdDate = DateTime.Now;
            var raceMap = new RaceMap
            {
                Name = raceMapBindingModel.Name,
                Description = raceMapBindingModel.Description,
                UserId = contextUserId,
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

            var contextUserId = tokenService.GetUserId(HttpContext.User);
            if (contextUserId == null) { return BadRequest(new { message = "Couldnt determine user from context" }); }
            if (contextUserId != raceMap.UserId) { return Unauthorized(); }

            appDbContext.RaceMaps.Remove(raceMap);
            await appDbContext.SaveChangesAsync();

            return Ok();
        }
    }
}
