using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using Trakx.Common.Models;

namespace Trakx.Common.Utils
{
    public static class MockJwtTokens
    {
        public static string Issuer { get; } = "barong";
        public static SecurityKey SecurityKey { get; }
        public static SigningCredentials SigningCredentials { get; }

        private static readonly JwtSecurityTokenHandler STokenHandler = new JwtSecurityTokenHandler();

        public static readonly IEnumerable<Claim> TrakxClaims = new List<Claim> { new Claim("iat", $"{DateTime.UtcNow}"), new Claim("sub", "session"), new Claim("jti", Guid.NewGuid().ToString()), new Claim("uid", Guid.NewGuid().ToString()), new Claim("email", "dupond@trakx.io"), new Claim("role", "admin"), new Claim("level", "3"), new Claim("state", "active"), new Claim("referral_id", "null") };

        static MockJwtTokens()
        {
            var sKey = Encoding.ASCII.GetBytes("ThisIsMySecretKey");
            SecurityKey = new SymmetricSecurityKey(sKey);
            SigningCredentials = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256);
        }
        
        public static Func<Task<string>> GenerateJwtToken(IEnumerable<Claim> claims = null)
        {
            var token = new JwtSecurityToken(Issuer, "peatio, barong", claims ?? TrakxClaims, null, DateTime.UtcNow.AddMinutes(20), SigningCredentials);

            return async () => STokenHandler.WriteToken(token);
        }

        public static List<Label> GenerateLabelsList()
        {
            return new List<Label>
            {
                new Label("email", "verified", "private", DateTimeOffset.Now, DateTimeOffset.Now),
                new Label("market", "maker", "private", DateTimeOffset.Now, DateTimeOffset.Now),
                new Label("phone", "verified", "public", DateTimeOffset.Now, DateTimeOffset.Now)
            };
        }
    }
}
