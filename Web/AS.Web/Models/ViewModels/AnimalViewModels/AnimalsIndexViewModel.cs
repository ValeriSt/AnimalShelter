using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AS.Web.Models.ViewModels.AnimalViewModels
{
    public class AnimalsIndexViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string ImageUrl { get; set; }
        public string Location { get; set; }
    }
}
