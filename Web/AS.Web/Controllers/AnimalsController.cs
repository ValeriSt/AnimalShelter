using System;
using System.Collections.Generic;
using System.Web;
using System.Linq;
using System.Threading.Tasks;
using AS.Data;
using AS.Data.Models;
using AS.Web.Models.ViewModels;
using AS.Web.Models.ViewModels.AnimalViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AS.Web.Controllers
{
    public class AnimalsController : Controller
    {
        private readonly ASDbContext aSDbContext;

        private UserManager<ASUser> userManager;

        public AnimalsController(ASDbContext aSDbContext, UserManager<ASUser> userMrg)
        {
            this.aSDbContext = aSDbContext;
            this.userManager = userMrg;
        }


        public async Task<IActionResult> Index()
        {
            var animals = this.aSDbContext.ASAnimals.Select(animal => new AnimalsIndexViewModel
            {
                Id = animal.Id,
                Name = animal.Name,
                Status = animal.Status,
                ImageUrl = animal.ImageURL,
                Location = animal.Location,
                UserId = animal.UserId,
            }).ToList();
            
            foreach (var animal in animals)
            {
                animal.IsAuthorized = await IsAuthorized(animal.Id);
            }
            return this.View(animals);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("Id,AnimalType,Name,Age,Color,ImageURL,Sex,Status,Location,DateTime,UserId")] ASAnimals animals)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.GetUserAsync(HttpContext?.User);
                animals.UserId = user.Id;
                animals.Id = Guid.NewGuid().ToString();
                aSDbContext.Add(animals);
                await aSDbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return this.View(animals);
        }
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var animal = await aSDbContext.ASAnimals.Include(m => m.User).FirstOrDefaultAsync(m => m.Id == id);
            if (animal == null)
            {
                return NotFound();
            }
            var viewmodel = new AnimalsVM
            {
                Animals = animal,
                //comments = new List<ASComments>(aSDbContext.ASComments)
            };
            return this.View(viewmodel);
        }
        //Get Animals/Edit/Id
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
            var animal = await aSDbContext.ASAnimals.FindAsync(id);
            if (animal == null)
            {
                return NotFound();
            }
            var viewModel = new AnimalsVM
            {
                Animals = animal
            };
            return this.View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(string id, [Bind("Id,AnimalType,Name,Age,Color,ImageURL,Sex,Status,Location,DateTime,UserId")] ASAnimals animals)
        {

            if (id != animals.Id)
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
                    var dbAnimals = await aSDbContext.ASAnimals.FindAsync(id);
                    aSDbContext.Entry(dbAnimals).CurrentValues.SetValues(animals);
                    await aSDbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AnimalExists(animals.Id))
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
            return View(animals);
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
            var animal = await aSDbContext.ASAnimals
                .FirstOrDefaultAsync(m => m.Id == id);
            if (animal == null)
            {
                return NotFound();
            }
            return View(animal);
        }
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteAnimal(string id)
        {
            if (!await IsAuthorized(id))
            {
                return Unauthorized();
            }
            var animal = await aSDbContext.ASAnimals.Include(m => m.Comments).FirstOrDefaultAsync(m => m.Id == id);
            aSDbContext.ASAnimals.Remove(animal);
            foreach (var comment in animal.Comments)
            {
                aSDbContext.ASComments.Remove(comment);
            }
            await aSDbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        private bool AnimalExists(string id)
        {
            return aSDbContext.ASAnimals.Any(e => e.Id == id);
        }

        public async Task<bool> IsAuthorized(string animalId) 
        {
            var user = await userManager.GetUserAsync(HttpContext?.User);
            var animals = await aSDbContext.ASAnimals.FindAsync(animalId);
            return await userManager.IsInRoleAsync(user, "Admin") || animals.UserId == user.Id;
        }
    }
}
