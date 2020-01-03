using Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces
{
    public interface ITokenService
    {
        public string CreateAccessToken(string userId);
        public RefreshToken CreateRefreshToken(string userId);
        public Task UpdateRefreshToken(string userId, RefreshToken refreshToken);
        public string GetUserId(ClaimsPrincipal claims);
        public Task<string[]> RenewTokens(string accessToken, string refreshToken);
    }
}