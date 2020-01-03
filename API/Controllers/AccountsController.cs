using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.BindingModels;
using Infrastructure.Data;
using Infrastructure.Entities;
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
                var accessToken = new JwtSecurityToken(issuer: issuer,
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(2),
                    signingCredentials: signingCred);

                // TODO : Remove RefreshToken logic to new class.
                // Consider ITokenService
                var refreshToken = CreateRefreshToken(user.Id);
                var currentRefreshToken = appDbContext.RefreshTokens.Where(x => x.UserId == user.Id).FirstOrDefault();
                if (currentRefreshToken == null)
                {
                    appDbContext.Remove(currentRefreshToken);
                    await appDbContext.SaveChangesAsync();
                }
                await appDbContext.AddAsync(refreshToken);
                await appDbContext.SaveChangesAsync();

                return Ok(new
                {
                    accessToken = new JwtSecurityTokenHandler().WriteToken(accessToken),
                    refreshToken = refreshToken.Value,
                    userId = user.Id
                });
            }
            else
            {
                return BadRequest(new { message = "Wrong password" });
            }
        }

        //[HttpPost("RefreshToken")]
        //[AllowAnonymous]
        //public async Task<IActionResult> RefreshAccessToken(RefreshTokenBindingModel model)
        //{
        //    var handler = new JwtSecurityTokenHandler();
        //    var tokenValidationParameters = new TokenValidationParameters
        //    {
        //        ValidIssuer = configuration.GetSection("JWTSettings").GetSection("Issuer").Value,
        //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetSection("JWTSettings").GetSection("Secret").Value)),
        //        ValidateAudience = false,
        //        ValidateIssuer = true,
        //        ValidateLifetime = false,
        //        ClockSkew = TimeSpan.Zero
        //    };
        //    SecurityToken securityToken;
        //    var principal = handler.ValidateToken(model.AccessToken, tokenValidationParameters, out securityToken);
        //    var jwtSecurityToken = securityToken as JwtSecurityToken;

        //    if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        //    {
        //        throw new SecurityTokenException("Invalid token!");
        //    }

        //    return Ok();
        //}

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

        private RefreshToken CreateRefreshToken(string userId)
        {
            string value;
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                value = Convert.ToBase64String(randomNumber);
            }

            var token = new RefreshToken
            {
                UserId = userId,
                ExpirationDate = DateTime.Now.AddMonths(6),
                Value = value
            };
            return token;
        }
    }
}