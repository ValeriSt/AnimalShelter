using System;
using System.Collections.Generic;
using System.Text;

namespace AS.Data.Models
{
    public class ASAnimalsComments
    {
        public string AnimalsId { get; set; }
        public ASAnimals Animals { get; set; }

        public string CommentsId { get; set; }
        public ASComments Comments { get; set; }
    }
}
