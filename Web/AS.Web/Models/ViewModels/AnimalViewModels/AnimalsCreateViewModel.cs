using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AS.Web.Models.ViewModels.AnimalViewModels
{
    public class AnimalsCreateViewModel
    {
        public string Id { get; set; }
        public string AnimalType { get; set; }
        public string Name { get; set; }
        public uint Age { get; set; }
        public string Color { get; set; }
        public string ImageURL { get; set; }
        public string Sex { get; set; }
        public string Status { get; set; }
        public string Location { get; set; }
        public DateTime DateTime { get; set; }
    }
}
