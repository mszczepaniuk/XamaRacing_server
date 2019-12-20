using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Data;
using Infrastructure.Data.Entities;

namespace API.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]
    public class RaceResultsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RaceResultsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/RaceResults
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RaceResult>>> GetRaceResults()
        {
            return await _context.RaceResults.ToListAsync();
        }

        // GET: api/RaceResults/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RaceResult>> GetRaceResult(int id)
        {
            var raceResult = await _context.RaceResults.FindAsync(id);

            if (raceResult == null)
            {
                return NotFound();
            }

            return raceResult;
        }


        // POST: api/RaceResults
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<RaceResult>> PostRaceResult(RaceResult raceResult)
        {
            _context.RaceResults.Add(raceResult);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRaceResult", new { id = raceResult.Id }, raceResult);
        }

        // DELETE: api/RaceResults/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<RaceResult>> DeleteRaceResult(int id)
        {
            var raceResult = await _context.RaceResults.FindAsync(id);
            if (raceResult == null)
            {
                return NotFound();
            }

            _context.RaceResults.Remove(raceResult);
            await _context.SaveChangesAsync();

            return raceResult;
        }
    }
}
