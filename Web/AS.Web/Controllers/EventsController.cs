using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AS.Data;
using AS.Data.Models;
using AS.Web.Models.ViewModels.EventViewModels;
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
        
        public async Task<IActionResult> Index(string sorting = null)
        {
            var animalEvents = this.aSDbContext.ASEvents.Include(x => x.GoingUsers).Select(events => new EventIndexViewModel
            {
                Id = events.Id,
                Location = events.Location,
                Description = events.Description,
                ImageUrl = events.ImageUrl,
                DateTime = events.DateTime,
                UserId = events.UserId,
                Email = events.User.Email,
                GoingUsers = events.GoingUsers
            }).ToList();

            foreach (var animalEvent in animalEvents)
            {
                animalEvent.IsOwned = await this.IsOwner(animalEvent.Id);
                animalEvent.IsAdmin = await this.IsAdmin();
                animalEvent.IsSubscribed = animalEvent.GoingUsers.Any(x => x.UserId == userManager.GetUserId(HttpContext.User));
            }

            switch (sorting)
            {
                case "Most Subscribers":
                    animalEvents = animalEvents.OrderByDescending(x => x.GoingUsers.Count).ToList();
                    break;
                case "Least Subscribers":
                    animalEvents = animalEvents.OrderBy(x => x.GoingUsers.Count).ToList();
                    break;
                case "Newest":
                    animalEvents = animalEvents.OrderByDescending(x => x.DateTime).ToList();
                    break;
                case "Oldest":
                    animalEvents = animalEvents.OrderBy(x => x.DateTime).ToList();
                    break;
                case "First Subscribed":
                    animalEvents = animalEvents.OrderByDescending(x => x.IsSubscribed).ToList();
                    break;
                case "Last Subscribed":
                    animalEvents = animalEvents.OrderBy(x => x.IsSubscribed).ToList();
                    break;
            }
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
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (!await IsOwner(id) || !await IsAdmin())
            {
                return Unauthorized();
            }
            var events = await aSDbContext.ASEvents.FindAsync(id);
            if (events == null)
            {
                return NotFound();
            }
            var viewModel = new EventVM
            {
                 Events = events
            };
            return this.View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Location,Description,ImageUrl,DateTime,UserId")] ASEvents events)
        {
            if (id != events.Id)
            {
                return NotFound();
            }

            if (!await IsOwner(id) || !await IsAdmin())
            {
                return Unauthorized();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var dbEvents = await aSDbContext.ASEvents.FindAsync(id);
                    aSDbContext.Entry(dbEvents).CurrentValues.SetValues(events);
                    await aSDbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(events.Id))
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
            return View(events);
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (!await IsOwner(id) || !await IsAdmin())
            {
                return Unauthorized();
            }
            if (id == null)
            {
                return NotFound();
            }

            var events = await aSDbContext.ASEvents
                .FirstOrDefaultAsync(c => c.Id == id);
            if (events == null)
            {
                return NotFound();
            }
            return View(events);
        }
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteEvent(string id)
        {
            if (!await IsOwner(id) || !await IsAdmin())
            {
                return Unauthorized();
            }
            var events = await aSDbContext.ASEvents.FindAsync(id);
            aSDbContext.ASEvents.Remove(events);
            await aSDbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private bool EventExists(string id)
        {
            return aSDbContext.ASEvents.Any(c => c.Id == id);
        }
        public async Task<IActionResult> Subscribe(string id)
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

        public async Task<IActionResult> UnSubscribe(string id)
        {
            var user = await userManager.GetUserAsync(HttpContext.User);
            var userEvent = await aSDbContext.ASEvents.Include(x => x.GoingUsers).FirstOrDefaultAsync(x => x.Id == id);

            userEvent.GoingUsers.RemoveAll(x => x.UserId == user.Id);
            var entry = this.aSDbContext.Entry(userEvent);
            if (entry.State == EntityState.Detached)
            {
                this.aSDbContext.Attach(userEvent);
            }
            entry.State = EntityState.Modified;
            await aSDbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<bool> IsOwner(string eventId)
        {
            var user = await userManager.GetUserAsync(HttpContext.User);
            var events = await aSDbContext.ASEvents.FindAsync(eventId);
            return events.UserId == user.Id;
        }
        public async Task<bool> IsAdmin()
        {
            var user = await userManager.GetUserAsync(HttpContext.User);
            return await userManager.IsInRoleAsync(user, "Admin");
        }
    }
}
