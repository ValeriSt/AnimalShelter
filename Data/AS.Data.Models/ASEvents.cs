using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AS.Data.Models
{
    public class ASEvents : BaseEntity
    {
        public string Location { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public List<ASUserEvents> UserEvents { get; set; }
    }
}
