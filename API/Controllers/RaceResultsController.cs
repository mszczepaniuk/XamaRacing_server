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

        // POST: v1/RaceResults
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<RaceResult>> PostRaceResult(RaceResultBindingModel raceResultBindingModel)
        {
            var raceResult = new RaceResult
            {
                RaceId = raceResultBindingModel.RaceId,
                Nickname = raceResultBindingModel.Nickname,
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
