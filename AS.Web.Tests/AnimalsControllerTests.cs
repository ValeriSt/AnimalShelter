using AS.Data;
using AS.Data.Models;
using AS.Web.Controllers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AS.Web.Tests
{
    [TestFixture]
    public class AnimalsControllerTests
    {
        private ASDbContext _asDbContext;
        private Mock<UserManager<ASUser>> _userManager;
        private AnimalsController _animalsController;

        [SetUp]
        public void Setup()
        {
            DbContextOptionsBuilder<ASDbContext> optionsBuilder = new DbContextOptionsBuilder<ASDbContext>()
                .UseInMemoryDatabase(databaseName: "AnimalShelter_Test_Database");
            this._asDbContext = new ASDbContext(optionsBuilder.Options);
            var UserStoreMock = Mock.Of<IUserStore<ASUser>>();
            this._userManager = new Mock<UserManager<ASUser>>(UserStoreMock, null, null, null, null, null, null, null, null);
            this._animalsController = new AnimalsController(this._asDbContext, _userManager.Object);
        }

        [TearDown]
        public void Dispose()
        {
            this._asDbContext.Dispose();
            this._animalsController = null;
        }
        [Test]
        public async Task Create_CreatesAnimalAndReturnsARedirect_WhenModelStateIsValid()
        {
            _userManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new ASUser());

            ASAnimals expectedEntity = new ASAnimals
            {
                AnimalType = "Intel",
                Name = "Core I9",
                Age = 8,
                Color = "Blue",
                ImageURL = "https://never-gonna-give-you-up.com/rick.png",
                Sex = "Male",
                Status = "Lost",
                Location = "QWERTY",
                DateTime = new System.DateTime(2008, 3, 1, 7, 0, 0)
            };
            IActionResult result = await this._animalsController.Create(expectedEntity);
            ASAnimals actualEntity = await this._asDbContext.ASAnimals.FirstOrDefaultAsync();

            expectedEntity.Id = actualEntity.Id;
            Assert.AreEqual(JsonSerializer.Serialize(expectedEntity), JsonSerializer.Serialize(actualEntity), "Create() method does not map properties correctly.");
            Assert.NotNull(actualEntity.Id, "Create() method does not set Id property correctly.");
            Assert.IsInstanceOf<RedirectToActionResult>(result);
        }

        [Test]
        public async Task Create_ReturnsView_WhenModelStateIsntValid()
        {
            this._animalsController.ModelState.AddModelError("test", "test");
            var result = await this._animalsController.Create(null);
            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public async Task Details_ReturnsNotFound_WhenIdIsNull()
        {
            var result = await this._animalsController.Details(null);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }
        [Test]
        public async Task Details_ReturnsNotFound_WhenAnimalIsNull()
        {
            var result = await this._animalsController.Details("");
            Assert.IsInstanceOf<NotFoundResult>(result);
        }
        [Test]
        public async Task Details_ReturnsView_WhenAnimalIsValid()
        {
            ASAnimals animals = new ASAnimals()
            {
                Id = new Guid().ToString()
            };
            var obj = _asDbContext.ASAnimals.Add(animals);
            _asDbContext.SaveChanges();
            var result = await this._animalsController.Details(obj.Entity.Id);
            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public async Task Edit_ReturnsNotFound_WhenIdIsNull()
        {
            var result = await this._animalsController.Edit(null);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }
        [Test]
        public async Task Edit_ReturnsUnauthorized_WhenAnimalIsntAuthorized()
        {
            _userManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new ASUser());
            ASAnimals animals = new ASAnimals()
            {
                Id = new Guid().ToString(),
                UserId = new Guid().ToString()
            };
            _asDbContext.ASAnimals.Add(animals);
            _asDbContext.SaveChanges();
            _userManager.Setup(x => x.IsInRoleAsync(It.IsAny<ASUser>(), It.IsAny<string>())).ReturnsAsync(false);
            var result = await this._animalsController.Edit(animals.Id);
            Assert.IsInstanceOf<UnauthorizedResult>(result);
        }
        [Test]
        public async Task Edit_ReturnsNotFound_WhenAnimalIsNull()
        {
            _userManager.Setup(x => x.IsInRoleAsync(It.IsAny<ASUser>(), It.IsAny<string>())).ReturnsAsync(true);
            var result = await this._animalsController.Edit("");
            Assert.IsInstanceOf<NotFoundResult>(result);
        }
        [Test]
        public async Task Edit_ReturnsView_WhenAnimalIsValid()
        {
            ASAnimals animals = new ASAnimals()
            {
                Id = new Guid().ToString()
            };
            _asDbContext.ASAnimals.Add(animals);
            _asDbContext.SaveChanges();
            _userManager.Setup(x => x.IsInRoleAsync(It.IsAny<ASUser>(), It.IsAny<string>())).ReturnsAsync(true);
            var result = await this._animalsController.Edit(animals.Id);
            Assert.IsInstanceOf<ViewResult>(result);
        }
        [Test]
        public async Task Edit_ReturnsNotFound_WhenIdIsNotTheSameAsAnimalsId()
        {
            ASAnimals animals = new ASAnimals()
            {
                Id = new Guid().ToString()
            };
            var result = await this._animalsController.Edit("", animals);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }
        [Test]
        public async Task Edit_ReturnsUnAuthorized_WhenAnimalIsntAuthorized()
        {
            _userManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new ASUser());
            ASAnimals animals = new ASAnimals()
            {
                Id = new Guid().ToString(),
                UserId = new Guid().ToString()
            };
            _asDbContext.ASAnimals.Add(animals);
            _asDbContext.SaveChanges();
            _userManager.Setup(x => x.IsInRoleAsync(It.IsAny<ASUser>(), It.IsAny<string>())).ReturnsAsync(false);
            var result = await this._animalsController.Edit(animals.Id, animals);
            Assert.IsInstanceOf<UnauthorizedResult>(result);
        }
        [Test]
        public async Task Edit_ReturnsRedirectToAction_WhenModelStateIsValid()
        {
            _userManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new ASUser());
            ASAnimals animals = new ASAnimals()
            {
                Id = new Guid().ToString()
            };
            var obj = _asDbContext.ASAnimals.Add(animals);
            _asDbContext.SaveChanges();
            _userManager.Setup(x => x.IsInRoleAsync(It.IsAny<ASUser>(), It.IsAny<string>())).ReturnsAsync(false);
            var result = await this._animalsController.Edit(obj.Entity.Id, animals);
            Assert.IsInstanceOf<RedirectToActionResult>(result);
        }
        [Test]
        public async Task Edit_ReturnsView_WhenModelStateIsInvalid()
        {
            ASAnimals animals = new ASAnimals()
            {
                Id = new Guid().ToString()
            };
            _asDbContext.ASAnimals.Add(animals);
            _asDbContext.SaveChanges();
            this._animalsController.ModelState.AddModelError("test", "test");
            _userManager.Setup(x => x.IsInRoleAsync(It.IsAny<ASUser>(), It.IsAny<string>())).ReturnsAsync(true);
            var result = await this._animalsController.Edit(animals.Id, animals);
            Assert.IsInstanceOf<ViewResult>(result);
        }
        [Test]
        public async Task Delete_ReturnsNotFound_WhenIdIsNull()
        {
            var result = await this._animalsController.Delete(null);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }
        [Test]
        public async Task Delete_ReturnsUnAuthorized_WhenIdIsNotAuthorized()
        {
            _userManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new ASUser());
            ASAnimals animals = new ASAnimals()
            {
                Id = new Guid().ToString(),
                UserId = new Guid().ToString()
            };
            var obj = _asDbContext.ASAnimals.Add(animals);
            _asDbContext.SaveChanges();
            _userManager.Setup(x => x.IsInRoleAsync(It.IsAny<ASUser>(), It.IsAny<string>())).ReturnsAsync(false);
            var result = await this._animalsController.Delete(obj.Entity.Id);
            Assert.IsInstanceOf<UnauthorizedResult>(result);
        }
        [Test]
        public async Task Delete_ReturnsNotFound_WhenAnimalIsNull()
        {
            _userManager.Setup(x => x.IsInRoleAsync(It.IsAny<ASUser>(), It.IsAny<string>())).ReturnsAsync(true);
            var result = await this._animalsController.Delete("");
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task Delete_ReturnsView_WhenAnimalIsValid()
        {
            ASAnimals animals = new ASAnimals()
            {
                Id = "foo"
            };
            var obj = this._asDbContext.ASAnimals.Add(animals);
            _asDbContext.SaveChanges();
            _userManager.Setup(x => x.IsInRoleAsync(It.IsAny<ASUser>(), It.IsAny<string>())).ReturnsAsync(true);
            var result = await this._animalsController.Delete(obj.Entity.Id);
            Assert.IsInstanceOf<ViewResult>(result);
        }
        [Test]
        public async Task DeleteAnimal_ReturnsUnAuthorized_WhenIdIsNotAuthorized()
        {
            _userManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new ASUser());
            ASAnimals animals = new ASAnimals()
            {
                Id = new Guid().ToString(),
                UserId = new Guid().ToString()
            };
            _asDbContext.ASAnimals.Add(animals);
            _asDbContext.SaveChanges();
            _userManager.Setup(x => x.IsInRoleAsync(It.IsAny<ASUser>(), It.IsAny<string>())).ReturnsAsync(false);
            var result = await this._animalsController.DeleteAnimal(animals.Id);
            Assert.IsInstanceOf<UnauthorizedResult>(result);
        }
        [Test]
        public async Task DeleteAnimal_ReturnsRedirectToAction_WhenCommentIsRemoved()
        {
            ASComments comments = new ASComments()
            {
                Id = new Guid().ToString()
            };
            ASAnimals animals = new ASAnimals()
            {
                Id = new Guid().ToString(),
                Comments = new List<ASComments>()
                {
                    comments
                }
            };
            this._asDbContext.ASComments.Add(comments);
            this._asDbContext.ASAnimals.Add(animals);
            _asDbContext.SaveChanges();
            _userManager.Setup(x => x.IsInRoleAsync(It.IsAny<ASUser>(), It.IsAny<string>())).ReturnsAsync(true);
            var result = await this._animalsController.DeleteAnimal(animals.Id);
            Assert.IsInstanceOf<RedirectToActionResult>(result);
        }
    }
}