using Microsoft.AspNetCore.Identity;
using SantaList.Models.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SantaList.Data
{
    public class DummyData
    {
        public static async Task Initialize(SantaContext context,
                          UserManager<AppUser> userManager,
                          RoleManager<AppRole> roleManager)
        {
            context.Database.EnsureCreated();

            String adminId1 = "";

            string admin = "Admin";
            string desc1 = "This is the administrator role";

            string role2 = "Child";
            string desc2 = "This is the child role";

            string password = "P@$$w0rd";

            if (await roleManager.FindByNameAsync(admin) == null)
            {
                await roleManager.CreateAsync(new AppRole(admin, desc1, DateTime.Now));
            }
            if (await roleManager.FindByNameAsync(role2) == null)
            {
                await roleManager.CreateAsync(new AppRole(role2, desc2, DateTime.Now));
            }

            if (await userManager.FindByNameAsync("santa@np.com") == null)
            {
                var user = new AppUser
                {
                    UserName = "santa",
                    Email = "santa@np.com",
                    FirstName = "Santa",
                    LastName = "Clause",
                    Street = "North St",
                    City = "North Pole",
                    Province = "Alaska",
                    PostalCode = "V5U K8I",
                    Country = "United States",
                    PhoneNumber = "6902341234",
                    isNaughty = false,
                    Latitude = 64.7511,
                    Longitude = -147.3494,
                    DateCreated = DateTime.Now,
                };

                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, admin);
                }
                adminId1 = user.Id;
            }

            if (await userManager.FindByNameAsync("tim@np.com") == null)
            {
                var user = new AppUser
                {
                    UserName = "tim",
                    Email = "tim@np.com",
                    FirstName = "Tim",
                    LastName = "Khaury",
                    Street = "Vermont St",
                    City = "Hennepin County",
                    Province = "Minnesota",
                    PostalCode = "V1P I5T",
                    Country = "United States",
                    PhoneNumber = "7788951456",
                    isNaughty = false,
                    Latitude = 45.0209,
                    Longitude = -93.5095,
                    DateCreated = DateTime.Now,
                };

                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role2);
                }
            }
        }
    }
}
