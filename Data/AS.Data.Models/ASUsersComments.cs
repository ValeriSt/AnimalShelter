using System;
using System.Collections.Generic;
using System.Text;

namespace AS.Data.Models
{
    public class ASUsersComments
    {
        public string UserId { get; set; }
        public ASUser User { get; set; }

        public string CommentsId { get; set; }
        public List<ASComments> Comment { get; set; }
    }
}
