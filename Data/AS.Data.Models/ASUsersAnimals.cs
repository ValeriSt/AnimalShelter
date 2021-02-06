using System;
using System.Collections.Generic;
using System.Text;

namespace AS.Data.Models
{
    public class ASUsersAnimals
    {
        public string UserId { get; set; }
        public ASUser User { get; set; }

        public string AnimalsId { get; set; }
        public ASAnimals Animals { get; set; }
    }
}
