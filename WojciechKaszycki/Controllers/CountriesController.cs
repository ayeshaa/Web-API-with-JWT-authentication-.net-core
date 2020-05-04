using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace WojciechKaszycki.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CountriesController : ControllerBase
    {
        private IConfiguration config;
        private readonly ILogger<CountriesController> _logger;

        public CountriesController(ILogger<CountriesController> logger, IConfiguration oconfig)
        {
            _logger = logger;
            config = oconfig;
        }
        [HttpGet]
        [Route("signup")]
        public string Get(string username, string password)
        {
            if (CheckUser(username, password))
            {
                var token = GenerateToken(username);
                return token;
            }

            throw new System.Web.Http.HttpResponseException(HttpStatusCode.Unauthorized);
        }

        [Microsoft.AspNetCore.Mvc.Route("checkname")]
        public bool CheckUser(string username, string password)
        {
            // should check in the database
            if (username == null || password == null)
            {
                return false;
            }
            if (username == "ayesha" && password == "ayesha")
            {
                return true;
            }
            return false;
        }
        private const string Secret = "db3OIsj+BXE9NZDy0t8W3TcNekrF+2d/1sFnWG4HnV8TZY30iTOdtVWJG8abWvB1GlOgJuQZdcF2Luqm/hccMw==";
        [Microsoft.AspNetCore.Mvc.Route("generatetoken")]
        private string GenerateToken(string username, int expireMinutes = 30)
        {
            var symmetricKey = Convert.FromBase64String(Secret);
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var tokenHandler = new JwtSecurityTokenHandler();
            var claims = new[] {
        new Claim(JwtRegisteredClaimNames.Sub, username),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };
            var token = new JwtSecurityToken(config["Jwt:Issuer"],
                                             config["Jwt:Issuer"],
                                             claims,
                                             expires: DateTime.Now.AddMinutes(120),
                                             signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);

            //    var now = DateTime.UtcNow;
            //    var tokenDescriptor = new SecurityTokenDescriptor
            //    {
            //        Subject = new ClaimsIdentity(new[]
            //        {
            //    new Claim(ClaimTypes.Name, username)
            //}),

            //        Expires = now.AddMinutes(Convert.ToInt32(expireMinutes)),

            //        SigningCredentials = new SigningCredentials(
            //            new SymmetricSecurityKey(symmetricKey),
            //            SecurityAlgorithms.HmacSha256Signature)
            //    };

            //    var stoken = tokenHandler.CreateToken(tokenDescriptor);
            //    var token = tokenHandler.WriteToken(stoken);

            //    return token;
        }
        


        [Route("getcountries")]
        public async Task<List<Country>> GetAsync(string filter)
        {
            using (var client = new HttpClient())
            {
                var requestUrl = "https://restcountries.eu/rest/v2/all";
                var content = await client.GetStringAsync(requestUrl);
                var response = JsonConvert.DeserializeObject<List<Country>>(content);
                var filteredResults = response.Where(x => x.name.Contains(filter));
                return filteredResults.ToList();
            }
        }
        [Authorize]
        [Route("test")]
        public ActionResult<IEnumerable<string>> Get()
        {
            var currentUser = HttpContext.User;
            int spendingTimeWithCompany = 0;

            if (currentUser.HasClaim(c => c.Type == "DateOfJoing"))
            {
                DateTime date = DateTime.Parse(currentUser.Claims.FirstOrDefault(c => c.Type == "DateOfJoing").Value);
                spendingTimeWithCompany = DateTime.Today.Year - date.Year;
            }

            if (spendingTimeWithCompany > 5)
            {
                return new string[] { "High Time1", "High Time2", "High Time3", "High Time4", "High Time5" };
            }
            else
            {
                return new string[] { "value1", "value2", "value3", "value4", "value5" };
            }
        }
    }
}