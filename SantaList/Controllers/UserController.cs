using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoogleMaps.LocationServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SantaList.Data;
using SantaList.Models.Auth;

namespace SantaList.Controllers
{
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
            }).ToList();
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

        // POST: api/User
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] RegistrationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var address = model.Street + ", " + model.City + ", " + model.Province + ", " + model.Country;
            var locationService = new GoogleLocationService("AIzaSyDsExQ1KTIx9MwJw917_vGHzt27xKKcUuA");
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
                PhoneNumber = model.Phone,
                Latitude = latitude,
                Longitude = longitude,
                DateCreated = DateTime.Now,
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return new BadRequestObjectResult(user);
            } else
            {
                await _userManager.AddToRoleAsync(user, "Child");
            }

            return new OkObjectResult("Account created");
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
            var locationService = new GoogleLocationService("AIzaSyDsExQ1KTIx9MwJw917_vGHzt27xKKcUuA");
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
                user.PhoneNumber = model.Phone;
                user.Latitude = latitude;
                user.Longitude = longitude;

                IdentityResult result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded) return new BadRequestResult();
            }
            return new OkResult();
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
