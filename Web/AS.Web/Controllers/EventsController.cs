using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AS.Data;
using AS.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AS.Web.Controllers
{
    public class EventsController : Controller
    {
        private readonly ASDbContext aSDbContext;

        private UserManager<ASUser> userManager;

        public EventsController(ASDbContext aSDbContext, UserManager<ASUser> userMrg)
        {
            this.aSDbContext = aSDbContext;
            this.userManager = userMrg;
        }

        public IActionResult Index()
        {
            var animalEvents = this.aSDbContext.ASEvents.Include(x => x.GoingUsers).Select(events => new ASEvents
            {
                Id = events.Id,
                Location = events.Location,
                Description = events.Description,
                ImageUrl = events.ImageUrl,
                DateTime = events.DateTime,
                UserId = events.UserId,
                GoingUsers = events.GoingUsers
            }).ToList();
            return this.View(animalEvents);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("Id,Location,Description,ImageUrl,DateTime,UserId")] ASEvents events)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.GetUserAsync(HttpContext.User);
                events.UserId = user.Id;
                events.Id = Guid.NewGuid().ToString();
                aSDbContext.Add(events);
                await aSDbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return this.View(events);
        }
        public async Task<IActionResult> SignUp (string id) 
        {
            var user = await userManager.GetUserAsync(HttpContext.User);
            var userEvent = await aSDbContext.ASEvents.FindAsync(id);
            var EventUser = new ASUserEvents
            {
                EventsId = userEvent.Id,
                UserId = user.Id
            };
            userEvent.GoingUsers.Add(EventUser);
            await aSDbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //kamen e gei mejdu drugoto, ama ne mu kazvai VS pls
        //i agree!
        //it'll be our little secret
    }
}
