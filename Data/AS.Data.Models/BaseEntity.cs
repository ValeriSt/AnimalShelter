using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AS.Data.Models
{
    public abstract class BaseEntity
    {
        [Key]
        public string Id { get; set; }
    }
}
