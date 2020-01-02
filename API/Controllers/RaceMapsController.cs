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
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace API.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]
    [Authorize]
    public class RaceMapsController : ControllerBase
    {
        private readonly AppDbContext appDbContext;

        public RaceMapsController(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
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
        public async Task<ActionResult<IEnumerable<RaceResult>>> GetRaceMapsResults(int id, int? offset, int? count)
        {
            offset ??= 0;
            if (count == null || count > 30) { count = 30; }
            var raceResults = await appDbContext.RaceResults.Where(x => x.RaceId == id)
                .OrderBy(x => x.Time)
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
        [HttpPut("{id}")]
        public async Task<ActionResult<RaceMap>> PutRaceMap(int id, RaceMapBindingModel raceMapBindingModel)
        {
            if (!ModelState.IsValid) { return BadRequest(raceMapBindingModel); }

            var raceMap = await appDbContext.RaceMaps.Include(x => x.RaceCheckpoints)
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();
            if (raceMap == null) { return NotFound(); }

            var claimsIdentity = HttpContext.User.Identity as ClaimsIdentity;
            // "sub" is mapped to "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier" automatically
            var contextUserId = claimsIdentity.Claims.Where(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").FirstOrDefault().Value;
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

            var claimsIdentity = HttpContext.User.Identity as ClaimsIdentity;
            // "sub" is mapped to "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier" automatically
            var contextUserId = claimsIdentity.Claims.Where(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").FirstOrDefault().Value;
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

            // TODO: Refactor authorization
            var claimsIdentity = HttpContext.User.Identity as ClaimsIdentity;
            // "sub" is mapped to "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier" automatically
            var contextUserId = claimsIdentity.Claims.Where(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").FirstOrDefault().Value;
            if (contextUserId == null) { return BadRequest(new { message = "Couldnt determine user from context" }); }
            if (contextUserId != raceMap.UserId) { return Unauthorized(); }

            appDbContext.RaceMaps.Remove(raceMap);
            await appDbContext.SaveChangesAsync();

            return Ok();
        }

        private bool IsLocationValid(double[] location)
        {
            if (location.Length != 2 ||
                location[0] > 90 ||
                location[0] < -90 ||
                location[1] > 180 ||
                location[1] < -180)
            {
                return false;
            }
            return true;
        }

        private double DistanceBeetweenLocationAndRaceCheckpoint(RaceCheckpoint checkpoint, double[] location)
        {
            var latitudeDiff = checkpoint.Latitude - location[0];
            var longitudeDiff = checkpoint.Longitude - location[1];
            if (Math.Abs(longitudeDiff) > 180)
            {
                longitudeDiff = 360 - longitudeDiff;
            }

            return Math.Sqrt(Math.Pow(latitudeDiff, 2) + Math.Pow(longitudeDiff, 2));
        }
    }
}
