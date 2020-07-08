using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;

namespace Trakx.IndiceManager.Server.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class AuthentificationController : ControllerBase
    {
        /// <summary>
        /// Use this route to get information about the bearer issued during the request.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(200)]
        public async Task<ActionResult<string>> GetBearerToken()
        {
            var head = HttpContext.Request.Headers["Authorization"].Join(" /// ");

            string accessToken = await HttpContext.GetTokenAsync("access_token");
            string refreshToken = await HttpContext.GetTokenAsync("refresh_token");
            string type = await HttpContext.GetTokenAsync("token_type");

            return Ok($"header content : {head} ; access token : {accessToken} ; refresh token: {refreshToken} ; token type: {type}");
        }
    }
}
