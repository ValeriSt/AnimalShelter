using AS.Data.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AS.Web.Models.ViewModels.RoleViewModels
{
    public class RoleEdit
    {
        public IdentityRole Role { get; set; }
        public IEnumerable<ASUser> Members { get; set; }
        public IEnumerable<ASUser> NonMembers { get; set; }
    }
}
