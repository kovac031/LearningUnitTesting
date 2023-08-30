using Microsoft.IdentityModel.Tokens;
using PlayPalMini.Model;
using PlayPalMini.Service.Common;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace PlayPalMini.WebAPI.Controllers
{
    public class AuthenticationController : ApiController
    {
        public IUserService Service { get; set; }
        public AuthenticationController(IUserService service)
        {
            Service = service;
        }
        [HttpPost]
        [Route("api/authenticate")]
        public async Task<IHttpActionResult> Authenticate(RegisteredUser user)
        {
            (string userRole, string message, Guid userId) = await ValidateUser(user.Username, user.Pass); // prvo trazi usera koristeci metodu od ispod

            if (!string.IsNullOrEmpty(userRole))
            {
                string token = GenerateToken(user.Username, userRole, userId); // ako je nasao usera, poziva drugu metodu od ispod da dobije token
                return Ok(new { Token = token });
            }

            if (!string.IsNullOrEmpty(message))
            {
                return BadRequest(message);
            }

            return Unauthorized();
        }
        private async Task<(string UserRole, string Message, Guid UserId)> ValidateUser(string username, string password)
        {
            try
            {
                (RegisteredUser user, string message) = await Service.FindUserAsync(username, password);
                if (user != null)
                    return (user.UserRole, message, user.Id);
                else
                    return (null, message, Guid.Empty);
            }
            catch (Exception x)
            {
                return (null, x.Message, Guid.Empty);
            }
        }
        private string GenerateToken(string username, string userRole, Guid userId)
        {
            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("123_ThisIsNotAGoodKeyButWhatever_321")); // treba bit nesto dugacko, 12345 ne prolazi
            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: "PlayPalMini",
                audience: "PlayPalMini",
                claims: new[] // ovdje se dodaje sta ce se pamtit pri autentikaciji (username, rola i Id)
                { 
                    new Claim(ClaimTypes.Name, username), 
                    new Claim(ClaimTypes.Role, userRole),
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString())
                },
                expires: DateTime.Now.AddHours(2),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}