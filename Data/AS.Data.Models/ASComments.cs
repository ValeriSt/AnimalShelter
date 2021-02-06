using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AS.Data.Models
{
    public class ASComments : BaseEntity
    {
        [Required]
        public DateTime DateTime { get; set; }
        [Required]
        [MaxLength(200)]
        public string Text { get; set; }
        public string UserId { get; set; }
        public ASUser user { get; set; }
        public string AnimalPostId { get; set; }
        public ASAnimals Animal { get; set; }

    }
}
