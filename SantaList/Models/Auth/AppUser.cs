using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SantaList.Models.Auth
{
    public class AppUser : IdentityUser
    {
        public AppUser() : base() { }
        [Display(Name = "First Name")]
        [MinLength(2)]
        [MaxLength(20)]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        [MinLength(2)]
        [MaxLength(20)]
        public string LastName { get; set; }
        [StringLength(30, MinimumLength = 5)]
        public string Street { get; set; }
        [StringLength(20, MinimumLength = 2)]
        public string City { get; set; }
        [MinLength(2)]
        public string Province { get; set; }
        [Display(Name = "ZIP Code")]
        [StringLength(7, MinimumLength = 7, ErrorMessage = "The postal code must be six characters.")]
        public string PostalCode { get; set; }
        [MinLength(2)]
        public string Country { get; set; }
        [Display(Name = "Mobile Phone")]
        [StringLength(10, MinimumLength = 10)]
        public string Phone { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public bool isNaughty { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
