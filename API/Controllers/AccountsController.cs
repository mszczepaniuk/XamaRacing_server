using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using API.BindingModels;
using Infrastructure.Data;
using Infrastructure.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace API.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IConfiguration configuration;
        private readonly AppDbContext appDbContext;

        public AccountsController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration,
            AppDbContext appDbContext)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.configuration = configuration;
            this.appDbContext = appDbContext;
        }

        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterUserBindingModel model)
        {
            if (ModelState.IsValid)
            {
                var currentDate = DateTime.Now;

                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    CreatedDate = currentDate,
                    UpdatedDate = currentDate
                };
                var result = await userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    return Ok();
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("UserCreationError", error.Description);
                }
            }
            // Add general data about password, because DataType errors arent added automatically
            ModelState.AddModelError("PasswordInfo", "Minimum password length is 6");
            ModelState.AddModelError("PasswordInfo", "Password requires at least one lowercase character");
            ModelState.AddModelError("PasswordInfo", "Password requires at least one digit");
            return BadRequest(ModelState);
        }

        [HttpPost("Authenticate")]
        [AllowAnonymous]
        public async Task<IActionResult> Authenticate(LoginUserBindingModel model)
        {
            var user = await userManager.FindByNameAsync(model.UserName);
            if (user == null) { return NotFound(new { message = "User wasn't found" }); };
            var result = await signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (result.Succeeded)
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetSection("JWTSettings").GetSection("Secret").Value));
                var signingCred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                };
                var issuer = configuration.GetSection("JWTSettings").GetSection("Issuer").Value;
                var token = new JwtSecurityToken(issuer: issuer,
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: signingCred);
                return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token), userId = user.Id });
            }
            else
            {
                return BadRequest(new { message = "Wrong password" });
            }
        }

        [HttpGet("{id}/RaceMaps")]
        public async Task<ActionResult<IEnumerable<RaceMap>>> GetUserMaps(string id, int? offset, int? count)
        {
            offset ??= 0;
            if (count == null || count > 30) { count = 30; }
            var raceMaps = await appDbContext.RaceMaps
                .Skip(offset.Value)
                .Take(count.Value)
                .Where(x => x.UserId == id)
                .OrderByDescending(x => x.CreatedDate)
                .ToArrayAsync();

            return raceMaps;
        }

        [HttpGet("{userId}/RaceMaps/{raceId}/RaceResults")]
        public async Task<ActionResult<IEnumerable<RaceResult>>> GetUserRaceResultsOnSpecificMap(string userId, int raceId, int? offset, int? count)
        {
            offset ??= 0;
            if (count == null || count > 30) { count = 30; }
            var raceResults = await appDbContext.RaceResults
                .OrderBy(x => x.Time)
                .Where(x => x.RaceId == raceId)
                .Where(x => x.UserId == userId)
                .Skip(offset.Value)
                .Take(count.Value)
                .ToArrayAsync();

            return raceResults;
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null) { return NotFound(new { message = "User wasn't found" }); }

            var claimsIdentity = HttpContext.User.Identity as ClaimsIdentity;
            // "sub" is mapped to "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier" automatically
            var contextUserId = claimsIdentity.Claims.Where(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").FirstOrDefault().Value;
            if (contextUserId == null) { return BadRequest(new { message = "Couldnt determine user from context" }); }
            if (contextUserId != id) { return Unauthorized(); }

            await userManager.DeleteAsync(user);
            return Ok();
        }
    }
}