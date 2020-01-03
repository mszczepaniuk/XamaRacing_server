using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Data;
using Infrastructure.Entities;
using API.BindingModels;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Infrastructure.Interfaces;

namespace API.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]
    [Authorize]
    public class RaceResultsController : ControllerBase
    {
        private readonly AppDbContext appDbContext;
        private readonly ITokenService tokenService;

        public RaceResultsController(AppDbContext appDbContext,
            ITokenService tokenService)
        {
            this.appDbContext = appDbContext;
            this.tokenService = tokenService;
        }

        // POST: v1/RaceResults
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<RaceResult>> PostRaceResult(RaceResultBindingModel raceResultBindingModel)
        {
            if (!ModelState.IsValid) { return BadRequest(raceResultBindingModel); }

            var contextUserId = tokenService.GetUserId(HttpContext.User);
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

            return CreatedAtAction("PostRaceResult", raceResult);
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

            var contextUserId = tokenService.GetUserId(HttpContext.User);
            if (contextUserId == null) { return BadRequest(new { message = "Couldnt determine user from context" }); }
            if (contextUserId != raceResult.UserId) { return Unauthorized(); }

            appDbContext.RaceResults.Remove(raceResult);
            await appDbContext.SaveChangesAsync();

            return Ok();
        }
    }
}
