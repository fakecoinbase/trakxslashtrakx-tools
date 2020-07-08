using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Trakx.Common.Utils;

namespace Trakx.IndiceManager.Server.Middlewares
{
    public class GetLabelsFromBearer
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _cache;

        
        public GetLabelsFromBearer(RequestDelegate next, IMemoryCache cache)
        {
            _next = next;
            _cache = cache;
        }

        public async Task Invoke(HttpContext context)
        {
            var authHeader = await context.GetTokenAsync("access_token")??  context.Request.Headers["Authorization"];

            if (authHeader != null)
            {
                authHeader = authHeader.Contains("Bearer ") ? authHeader.Substring(7) : authHeader;
                var handler = new JwtSecurityTokenHandler();
                var tokens = handler.ReadJwtToken(authHeader);
                var userId = tokens.Claims.FirstOrDefault(c => c.Type == "uid")?.Value;
                GetOrCreateFromCache(userId);
            }
            
            await _next(context);
        }
        private void GetOrCreateFromCache(string? userId)
        {
            if (string.IsNullOrEmpty(userId)) return;
            if (_cache.TryGetValue("bearer_labels_" + userId, out _)) return;

            object? cacheEntry = MockJwtTokens.GenerateLabelsList();

            _cache.Set("bearer_labels_" + userId, cacheEntry, TimeSpan.FromMinutes(2));
        }
    }
}
