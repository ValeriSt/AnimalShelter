using System;
using System.Collections.Generic;
using System.Text;

namespace AS.Data.Models
{
    public class ASUserEvents
    {
        public string UserId { get; set; }
        public ASUser User { get; set; }

        public string EventsId { get; set; }
        public ASEvents Event { get; set; }
    }
}
