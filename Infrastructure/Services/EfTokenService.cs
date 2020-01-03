using Infrastructure.Data;
using Infrastructure.Entities;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class EfTokenService : ITokenService
    {
        private readonly IConfiguration configuration;
        private readonly AppDbContext appDbContext;
        private readonly UserManager<ApplicationUser> userManager;

        public EfTokenService(IConfiguration configuration,
            AppDbContext appDbContext,
            UserManager<ApplicationUser> userManager)
        {
            this.configuration = configuration;
            this.appDbContext = appDbContext;
            this.userManager = userManager;
        }

        public string CreateAccessToken(string userId)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetSection("JWTSettings").GetSection("Secret").Value));
            var signingCred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                    new Claim(JwtRegisteredClaimNames.Sub, userId),
            };
            var issuer = configuration.GetSection("JWTSettings").GetSection("Issuer").Value;
            var accessToken = new JwtSecurityToken(issuer: issuer,
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: signingCred);
            return new JwtSecurityTokenHandler().WriteToken(accessToken);
        }

        public RefreshToken CreateRefreshToken(string userId)
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

        public async Task UpdateRefreshToken(string userId, RefreshToken refreshToken)
        {
            var currentRefreshToken = appDbContext.RefreshTokens.Where(x => x.UserId == userId).FirstOrDefault();
            if (currentRefreshToken != null)
            {
                appDbContext.Remove(currentRefreshToken);
                await appDbContext.SaveChangesAsync();
            }
            await appDbContext.AddAsync(refreshToken);
            await appDbContext.SaveChangesAsync();
        }

        public string GetUserId(ClaimsPrincipal claims)
        {
            var claimsIdentity = claims.Identity as ClaimsIdentity;
            // "sub" is mapped to "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier" automatically
            var userId = claimsIdentity.Claims.Where(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").FirstOrDefault().Value;
            return userId;
        }

        public async Task<string[]> RenewTokens(string accessToken, string refreshToken)
        {
            var handler = new JwtSecurityTokenHandler();
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = configuration.GetSection("JWTSettings").GetSection("Issuer").Value,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetSection("JWTSettings").GetSection("Secret").Value)),
                ValidateAudience = false,
                ValidateIssuer = true,
                ValidateLifetime = false,
                ClockSkew = TimeSpan.Zero
            };
            SecurityToken securityToken;
            var claimsPrincipal = handler.ValidateToken(accessToken, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;

            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return new string[] { };
            }
            var userId = GetUserId(claimsPrincipal);

            RefreshToken currentRefreshToken = appDbContext.RefreshTokens.Where(x => x.Value == refreshToken).FirstOrDefault();

            if (currentRefreshToken == null 
                || currentRefreshToken.UserId != userId
                || DateTime.Compare(currentRefreshToken.ExpirationDate, DateTime.Now) > 0)
            {
                return new string[] { };
            }

            var refreshTokenObject = CreateRefreshToken(userId);
            await UpdateRefreshToken(userId, refreshTokenObject);

            return new string[] { CreateAccessToken(userId), refreshTokenObject.Value };
        }

    }
}
