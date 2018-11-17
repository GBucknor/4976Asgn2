using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SantaList.Models.Auth
{
    public class UserListModel
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
