using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using GoogleMaps.LocationServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SantaList.Models.Auth;

namespace SantaList.Controllers
{
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly RoleManager<AppRole> _roleManager;

        public AuthController(UserManager<AppUser> userManager, IConfiguration configuration, RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _configuration = configuration;
            _roleManager = roleManager;
        }

        // POST /login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]CredentialsModel credentials)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByNameAsync(credentials.UserName);
            if (user != null && await _userManager.CheckPasswordAsync(user, credentials.Password))
            {
                var claim = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.UserName)
                };
                var signinKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_configuration["Jwt:SigningKey"]));
                int expiryInMinutes = Convert.ToInt32(_configuration["Jwt:ExpiryInMinutes"]);
                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Site"],
                    audience: _configuration["Jwt:Site"],
                    expires: DateTime.UtcNow.AddMinutes(expiryInMinutes),
                    signingCredentials: new SigningCredentials(signinKey, SecurityAlgorithms.HmacSha256)
                );
                string userRole = _roleManager.Roles.Single(r => r.Name == _userManager.GetRolesAsync(user).Result.Single()).Id;
                return Ok(
                    new
                    {
                        user = user.Id,
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        expiration = token.ValidTo,
                        role = userRole
                    }
                );
            }
            return Unauthorized();
        }

        // POST: /register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var address = model.Street + ", " + model.City + ", " + model.Province + ", " + model.Country;
            var locationService = new GoogleLocationService("AIzaSyDR2Q4cDiK8JabHLjuBjWyCrXRkhewxCxw");
            var point = locationService.GetLatLongFromAddress(address);

            double latitude = point.Latitude;
            double longitude = point.Longitude;

            var user = new AppUser
            {
                UserName = model.UserName,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Street = model.Street,
                City = model.City,
                Province = model.Province,
                PostalCode = model.PostalCode,
                Country = model.Country,
                Phone = model.Phone,
                isNaughty = false,
                Latitude = latitude,
                Longitude = longitude,
                DateCreated = DateTime.Now,
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return new BadRequestObjectResult(user);
            }
            else
            {
                await _userManager.AddToRoleAsync(user, "Child");
            }

            return new OkObjectResult("Account created");
        }
    }
}