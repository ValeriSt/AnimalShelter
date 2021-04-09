using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AS.Data;
using AS.Data.Models;
using AS.Web.Models.ViewModels.AnimalViewModels;
using AS.Web.Models.ViewModels.CommentViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AS.Web.Controllers
{
    public class CommentsController : Controller
    {
        private readonly ASDbContext aSDbContext;

        private UserManager<ASUser> userManager;
        public CommentsController(ASDbContext aSDbContext, UserManager<ASUser> userMrg)
        {
            this.aSDbContext = aSDbContext;
            this.userManager = userMrg;
        }

        //public IActionResult Index()
        //{
        //    var comments = this.aSDbContext.ASComments.Select(comment => new CommentsIndexViewModel
        //    {
        //        Id = comment.Id,
        //        Text = comment.Text,
        //        DateTime = comment.DateTime,
        //        Email = comment.user.Email
                
        //    }).ToList();
        //    return this.View(comments);
        //}
        [HttpGet("/Comments/{Id}/Create")]
        public IActionResult Create(string id)
        {
            return View();
        }
        [HttpPost(("/Comments/{Id}/Create"))]
        public async Task<IActionResult> Create([Bind("Id,DateTime,Text,UserId,AnimalPostId")] ASComments comments)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.GetUserAsync(HttpContext?.User);
                comments.UserId = user.Id;
                comments.Id = Guid.NewGuid().ToString();
                comments.AnimalPostId = this.RouteData?.Values["Id"].ToString();
                aSDbContext.Add(comments);
                await aSDbContext.SaveChangesAsync();
                return this.Redirect($"/Comments/{comments.AnimalPostId}");
            }
            return this.View(comments);
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            if (!await IsAuthorized(id))
            {
                return Unauthorized();
            }
            var comment = await aSDbContext.ASComments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }
            var viewModel = new CommentsVM
            {
                Comments = comment
            };
            return this.View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, [Bind("Id,DateTime,Text,UserId,AnimalPostId")] ASComments comments)
        {
            if (id != comments.Id)
            {
                return NotFound();
            }
            if (!await IsAuthorized(id))
            {
                return Unauthorized();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var dbComments = await aSDbContext.ASComments.FindAsync(id);
                    aSDbContext.Entry(dbComments).CurrentValues.SetValues(comments);
                    await aSDbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommentExists(comments.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw new Exception("");
                    }
                }
                return this.Redirect($"/Comments/{comments.AnimalPostId}");
            }
            return View(comments);
        }
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            if (!await IsAuthorized(id))
            {
                return Unauthorized();
            }
            

            var comments = await aSDbContext.ASComments
                .FirstOrDefaultAsync(c => c.Id == id);
            if (comments == null)
            {
                return NotFound();
            }
            return View(comments);
        }
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteComment(string id)
        {
            if (!await IsAuthorized(id))
            {
                return Unauthorized();
            }
            var comments = await aSDbContext.ASComments.FindAsync(id);
            aSDbContext.ASComments.Remove(comments);
            await aSDbContext.SaveChangesAsync();
            return this.Redirect($"/Comments/{comments.AnimalPostId}");
        }
        private bool CommentExists(string id)
        {
            return aSDbContext.ASComments.Any(c => c.Id == id);
        }
        [HttpGet("/Comments/{Id}")]
        public async Task<IActionResult> PostComments(string id)
        {
            
            var comments = this.aSDbContext.ASComments.Where(x => x.AnimalPostId == id).Select(comment => new CommentsIndexViewModel
            {
                Id = comment.Id,
                Text = comment.Text,
                DateTime = comment.DateTime,
                Email = comment.user.Email,
                UserId = comment.UserId

            }).ToList();
            foreach (var comment in comments)
            {
                comment.IsAuthorized = await IsAuthorized(comment.Id);
            }
            return this.View(comments);
        }
        public async Task<bool> IsAuthorized(string commentId)
        {
            var user = await userManager.GetUserAsync(HttpContext?.User);
            var comment = await aSDbContext.ASComments.FindAsync(commentId);
            return await userManager.IsInRoleAsync(user, "Admin") || comment.UserId == user.Id;
        }
    }
}
