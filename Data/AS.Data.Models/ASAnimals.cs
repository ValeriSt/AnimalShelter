using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AS.Data.Models
{
    public class ASAnimals : BaseEntity
    {
        [Required]
        public string AnimalType { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public uint Age { get; set; }
        [Required]
        public string Color { get; set; }
        [Required]
        public string ImageURL { get; set; }
        [Required]
        public string Sex { get; set; }
        [Required]
        public string Status { get; set; }
        [Required]
        public string Location { get; set; }
        [Required]
        public DateTime DateTime { get; set; }
        public string UserId { get; set; }
        public ASUser User { get; set; }
        public List<ASComments> Comments { get; set; }
    }
}
