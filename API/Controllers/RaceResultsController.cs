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

namespace API.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]
    [Authorize]
    public class RaceResultsController : ControllerBase
    {
        private readonly AppDbContext appDbContext;

        public RaceResultsController(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        // POST: v1/RaceResults
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<RaceResult>> PostRaceResult(RaceResultBindingModel raceResultBindingModel)
        {
            if (!ModelState.IsValid) { return BadRequest(raceResultBindingModel); }

            var claimsIdentity = HttpContext.User.Identity as ClaimsIdentity;
            // "sub" is mapped to "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier" automatically
            var contextUserId = claimsIdentity.Claims.Where(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").FirstOrDefault().Value;
            if (contextUserId == null) { return BadRequest(new { message = "Couldnt determine user from context" }); }

            var race = await appDbContext.RaceMaps.FindAsync(raceResultBindingModel.RaceId.Value);
            if (race == null) { return NotFound(new { message = $"No race with id = {raceResultBindingModel.RaceId.Value.ToString()}" }); };

            TimeSpan timeSpan;
            var parsingResult = TimeSpan.TryParse(raceResultBindingModel.Time, out timeSpan);
            if (!parsingResult) { return BadRequest(new { message = "Couldn't parse the time" }); }

            var raceResult = new RaceResult
            {
                RaceId = raceResultBindingModel.RaceId.Value,
                Race = race,
                UserId = contextUserId,
                Time = timeSpan,
                CreatedDate = DateTime.Now
            };
            appDbContext.RaceResults.Add(raceResult);
            await appDbContext.SaveChangesAsync();

            return Ok(raceResult);
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

            var claimsIdentity = HttpContext.User.Identity as ClaimsIdentity;
            // "sub" is mapped to "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier" automatically
            var contextUserId = claimsIdentity.Claims.Where(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").FirstOrDefault().Value;
            if (contextUserId == null) { return BadRequest(new { message = "Couldnt determine user from context" }); }
            if (contextUserId != raceResult.UserId) { return Unauthorized(); }

            appDbContext.RaceResults.Remove(raceResult);
            await appDbContext.SaveChangesAsync();

            return Ok();
        }
    }
}
