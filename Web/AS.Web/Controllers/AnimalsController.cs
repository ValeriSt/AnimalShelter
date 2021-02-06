using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AS.Data;
using AS.Data.Models;
using AS.Web.Models.ViewModels;
using AS.Web.Models.ViewModels.AnimalViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AS.Web.Controllers
{
    public class AnimalsController : Controller
    {
        private readonly ASDbContext aSDbContext;

        public AnimalsController(ASDbContext aSDbContext)
        {
            this.aSDbContext = aSDbContext;
        }


        public ActionResult Index()
        {
            var animals = this.aSDbContext.ASAnimals.Select(animal => new AnimalsIndexViewModel
            {
                Id = animal.Id,
                Name = animal.Name,
                Status = animal.Status,
                ImageUrl = animal.ImageURL,
                Location = animal.Location
            }).ToList();
            return this.View(animals);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("Id,AnimalType,Name,Age,Color,ImageURL,Sex,Status,Location,DateTime")] ASAnimals animals)
        {
            if (ModelState.IsValid)
            {
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
            var animal = await aSDbContext.ASAnimals.FirstOrDefaultAsync(m => m.Id == id);
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
        [Authorize(Roles = "Admin")]
        //Get Animals/Edit/Id
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
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
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Edit(string id, [Bind("Id,AnimalType,Name,Age,Color,ImageURL,Sex,Status,Location,DateTime")] ASAnimals animals)
        {
            
            if (id != animals.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {              
                try
                {
                    aSDbContext.Update(animals);
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var animal = await aSDbContext.ASAnimals
                .FirstOrDefaultAsync(m => m.Id == id);
            if (animal == null)
            {
                return NotFound();
            }
            return View(animal);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var animal = await aSDbContext.ASAnimals.FindAsync(id);
            aSDbContext.ASAnimals.Remove(animal);
            await aSDbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private bool AnimalExists(string id)
        {
            return aSDbContext.ASAnimals.Any(e => e.Id == id);
        }
    }
}
