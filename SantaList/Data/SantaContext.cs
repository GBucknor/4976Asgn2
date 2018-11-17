using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SantaList.Models.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SantaList.Data
{
    public class SantaContext : IdentityDbContext<AppUser, AppRole, string>
    {
        public SantaContext(DbContextOptions<SantaContext> options)
            : base (options)
        {
        }
        public DbSet<AppRole> AppRoles { get; set; }
    }
}
