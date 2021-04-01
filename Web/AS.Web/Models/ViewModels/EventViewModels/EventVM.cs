using AS.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AS.Web.Models.ViewModels.EventViewModels
{
    public class EventVM
    {
        public string Id { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public DateTime DateTime { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }
        public List<ASUserEvents> GoingUsers { get; set; } = new List<ASUserEvents>();
        public bool IsSubscribed { get; set; }
        public bool IsAuthorized { get; set; }
    }
}
