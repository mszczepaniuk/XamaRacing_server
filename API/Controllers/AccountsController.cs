using API.BindingModels;
using Infrastructure.Data;
using Infrastructure.Entities;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly AppDbContext appDbContext;
        private readonly ITokenService tokenService;

        public AccountsController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            AppDbContext appDbContext,
            ITokenService tokenService)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.appDbContext = appDbContext;
            this.tokenService = tokenService;
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
                var accessToken = tokenService.CreateAccessToken(user.Id);

                var refreshToken = tokenService.CreateRefreshToken(user.Id);
                await tokenService.UpdateRefreshToken(user.Id, refreshToken);

                return Ok(new
                {
                    accessToken,
                    refreshToken = refreshToken.Value,
                    userId = user.Id
                });
            }
            else
            {
                return BadRequest(new { message = "Wrong password" });
            }
        }

        [HttpPost("RefreshAccessToken")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshAccessToken(RefreshTokenBindingModel model)
        {
            var tokenArray = tokenService.RenewTokens(model.AccessToken, model.RefreshToken).Result;
            // TODO: Error message, get validation errors from RenewTokens().
            if (tokenArray.Length != 2) { return BadRequest(); }

            return Ok(new { accessToken = tokenArray[0], refreshToken = tokenArray[1] });
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

        [HttpGet("{userId}/RaceMaps/{mapId}/RaceResults")]
        public async Task<ActionResult<IEnumerable<RaceResult>>> GetUserRaceResultsOnSpecificMap(string userId, int mapId, int? offset, int? count)
        {
            offset ??= 0;
            if (count == null || count > 30) { count = 30; }
            var raceResults = await appDbContext.RaceResults
                .OrderBy(x => x.Time)
                .Where(x => x.RaceId == mapId)
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

            var contextUserId = tokenService.GetUserId(HttpContext.User);
            if (contextUserId == null) { return BadRequest(new { message = "Couldnt determine user from context" }); }
            if (contextUserId != id) { return Unauthorized(); }

            await userManager.DeleteAsync(user);
            return Ok();
        }
    }
}