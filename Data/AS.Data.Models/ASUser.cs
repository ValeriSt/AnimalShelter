using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AS.Data.Models
{
    public class ASUser : IdentityUser<string>
    {
        public string Address { get; set; }
        public uint Age { get; set; }
        public List<ASAnimals> Animals { get; set; }
        public List<ASUserEvents> UserEvents { get; set; }
        public List<ASComments> Comments { get; set; }

    }
}
