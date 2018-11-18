using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoogleMaps.LocationServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SantaList.Data;
using SantaList.Models.Auth;

namespace SantaList.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly SantaContext _context;

        public UserController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager
            , SantaContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        // GET: api/User/role/5
        [HttpGet("role/{id}", Name = "Role")]
        public async Task<IActionResult> GetUserRole(string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            string role = _userManager.GetRolesAsync(user).Result.Single();

            return Ok(new {
                role = new String(role)
            });
        }

        // GET: api/User
        [HttpGet]
        public IEnumerable<UserListModel> Get()
        {
            List<UserListModel> model = new List<UserListModel>();
            model = _userManager.Users.Select(user => new UserListModel
            {
                Id = user.Id,
                Username = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                IsNaughty = user.isNaughty
            }).Where(user => user.Username != "santa").ToList();
            return model;
        }

        // GET: api/User/5
        [HttpGet("{id}", Name = "Get")]
        public async Task<IActionResult> Get([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // PUT: api/User/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] EditUserListModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != model.Id)
            {
                return BadRequest();
            }

            var address = model.Street + ", " + model.City + ", " + model.Province + ", " + model.Country;
            var locationService = new GoogleLocationService("AIzaSyDWn7EHZxQxaEeyjJYJbsvWyL1Neo-cJaE");
            var point = locationService.GetLatLongFromAddress(address);

            double latitude = point.Latitude;
            double longitude = point.Longitude;

            AppUser user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                user.UserName = model.UserName;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Email = model.Email;
                user.Street = model.Street;
                user.City = model.City;
                user.Province = model.Province;
                user.PostalCode = model.PostalCode;
                user.Country = model.Country;
                user.Phone = model.Phone;
                user.isNaughty = model.IsNaughty;
                user.Latitude = latitude;
                user.Longitude = longitude;

                IdentityResult result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded) return new BadRequestResult();
            }
            return new OkObjectResult("User info updated");
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (!String.IsNullOrEmpty(id))
            {
                AppUser applicationUser = await _userManager.FindByIdAsync(id);
                if (applicationUser != null)
                {
                    IdentityResult result = await _userManager.DeleteAsync(applicationUser);
                    if (result.Succeeded)
                    {
                        return new OkObjectResult("User deleted");
                    }
                }
            }
            return new BadRequestObjectResult("Must pass an id.");
        }
    }
}
