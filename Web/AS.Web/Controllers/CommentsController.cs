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

        public async Task<IActionResult> Index()
        {
            return View(await aSDbContext.ASComments.ToListAsync());
        }
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
                var user = await userManager.GetUserAsync(HttpContext.User);
                comments.UserId = user.Id;
                comments.Id = Guid.NewGuid().ToString();
                comments.AnimalPostId = this.RouteData.Values["Id"].ToString();
                aSDbContext.Add(comments);
                await aSDbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return this.View(comments);
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
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

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Edit(string id, [Bind("Id,DateTime,Text,UserId,AnimalPostId")] ASComments comments)
        {

            if (id != comments.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    aSDbContext.Update(comments);
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
                return this.RedirectToAction(nameof(Index));
            }
            return View(comments);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comments = await aSDbContext.ASComments
                .FirstOrDefaultAsync(c => c.Id == id);
            if (comments == null)
            {
                return NotFound();
            }
            return View(comments);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var comments = await aSDbContext.ASComments.FindAsync(id);
            aSDbContext.ASComments.Remove(comments);
            await aSDbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private bool CommentExists(string id)
        {
            return aSDbContext.ASComments.Any(c => c.Id == id);
        }
    }
}
