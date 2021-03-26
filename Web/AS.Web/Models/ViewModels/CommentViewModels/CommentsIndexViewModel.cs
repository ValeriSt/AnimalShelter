using AS.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AS.Web.Models.ViewModels.CommentViewModels
{
    public class CommentsIndexViewModel
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public DateTime DateTime { get; set; }
        public string Email { get; set; }
        public string AnimalPostId { get; set; }
    }
}
