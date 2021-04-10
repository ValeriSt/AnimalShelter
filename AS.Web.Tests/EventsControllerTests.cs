using AS.Data;
using AS.Data.Models;
using AS.Web.Controllers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AS.Web.Tests
{
    [TestFixture]
    public class EventsControllerTests
    {
        private ASDbContext _asDbContext;
        private Mock<UserManager<ASUser>> _userManager;
        private EventsController _eventsController;

        [SetUp]
        public void Setup()
        {
            DbContextOptionsBuilder<ASDbContext> optionsBuilder = new DbContextOptionsBuilder<ASDbContext>()
                .UseInMemoryDatabase(databaseName: "AnimalShelter_Test_Database");
            this._asDbContext = new ASDbContext(optionsBuilder.Options);
            var UserStoreMock = Mock.Of<IUserStore<ASUser>>();
            this._userManager = new Mock<UserManager<ASUser>>(UserStoreMock, null, null, null, null, null, null, null, null);
            this._eventsController = new EventsController(this._asDbContext, _userManager.Object);
        }
        public void Dispose()
        {
            this._asDbContext.Dispose();
            this._eventsController = null;
        }
        [Test]
        public async Task Create_CreatesEventAndReturnsARedirect_WhenModelStateIsValid()
        {
            _userManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new ASUser());

            ASEvents expectedEntity = new ASEvents
            {
                Location = "U nas",
                Description = "Nosete Piene",
                ImageUrl = "https://never-gonna-give-you-up.com/rick.png",
                DateTime = new System.DateTime(2008, 3, 1, 7, 0, 0),
                
            };
            IActionResult result = await this._eventsController.Create(expectedEntity);
            ASEvents actualEntity = await this._asDbContext.ASEvents.FirstOrDefaultAsync();

            expectedEntity.Id = actualEntity.Id;
            Assert.AreEqual(JsonSerializer.Serialize(expectedEntity), JsonSerializer.Serialize(actualEntity), "Create() method does not map properties correctly.");
            Assert.NotNull(actualEntity.Id, "Create() method does not set Id property correctly.");
            Assert.IsInstanceOf<RedirectToActionResult>(result);
        }
        [Test]
        public async Task Create_ReturnsView_WhenModelStateIsntValid()
        {
            this._eventsController.ModelState.AddModelError("test", "test");
            var result = await this._eventsController.Create(null);
            Assert.IsInstanceOf<ViewResult>(result);
        }
        [Test]
        public async Task Edit_ReturnsNotFound_WhenIdIsNull()
        {
            var result = await this._eventsController.Edit(null);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }
        [Test]
        public async Task Edit_ReturnsUnauthorized_WhenIdIsNotOwnedAndRoleIsNotAdmin()
        {
            _userManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new ASUser());
            ASEvents events = new ASEvents()
            {
                Id = new Guid().ToString(),
                UserId = new Guid().ToString()
            };
            _asDbContext.ASEvents.Add(events);
            _asDbContext.SaveChanges();
            _userManager.Setup(x => x.IsInRoleAsync(It.IsAny<ASUser>(), It.IsAny<string>())).ReturnsAsync(false);
            var result = await this._eventsController.Edit(events.Id);
            Assert.IsInstanceOf<UnauthorizedResult>(result);
        }
        [Test]
        public async Task Edit_ReturnsNotFound_WhenEventIsNull()
        {
            _userManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new ASUser());
            ASEvents events = new ASEvents()
            {
                Id = new Guid().ToString()
            };
            _asDbContext.ASEvents.Add(events);
            _asDbContext.SaveChanges();
            _userManager.Setup(x => x.IsInRoleAsync(It.IsAny<ASUser>(), It.IsAny<string>())).ReturnsAsync(true);
            var result = await this._eventsController.Edit("");
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task Edit_ReturnsView_WhenEventIsValid()
        {
            _userManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new ASUser());
            ASEvents events = new ASEvents()
            {
                Id = new Guid().ToString()
            };
            _asDbContext.ASEvents.Add(events);
            _asDbContext.SaveChanges();
            _userManager.Setup(x => x.IsInRoleAsync(It.IsAny<ASUser>(), It.IsAny<string>())).ReturnsAsync(true);
            var result = await this._eventsController.Edit(events.Id);
            Assert.IsInstanceOf<ViewResult>(result);
        }
        [Test]
        public async Task Edit_ReturnsNotFound_WhenIdIsNotTheSameAsEventId()
        {
            ASEvents events = new ASEvents()
            {
                Id = new Guid().ToString()
            };
            var result = await this._eventsController.Edit("", events);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }
        [Test]
        public async Task Edit_ReturnsUnAuthorized_WhenIdIsNotOwnedAndRoleIsNotAdmin()
        {
            _userManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new ASUser());
            ASEvents events = new ASEvents()
            {
                Id = new Guid().ToString(),
                UserId = new Guid().ToString()
            };
            _asDbContext.ASEvents.Add(events);
            _asDbContext.SaveChanges();
            _userManager.Setup(x => x.IsInRoleAsync(It.IsAny<ASUser>(), It.IsAny<string>())).ReturnsAsync(false);
            var result = await this._eventsController.Edit(events.Id, events);
            Assert.IsInstanceOf<UnauthorizedResult>(result);
        }
        [Test]
        public async Task Edit_ReturnsRedirectToAction_WhenModelStateIsValid()
        {
            _userManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new ASUser());
            ASEvents events = new ASEvents()
            {
                Id = new Guid().ToString()
            };
            var obj = _asDbContext.ASEvents.Add(events);
            _asDbContext.SaveChanges();
            _userManager.Setup(x => x.IsInRoleAsync(It.IsAny<ASUser>(), It.IsAny<string>())).ReturnsAsync(false);
            var result = await this._eventsController.Edit(obj.Entity.Id, events);
            Assert.IsInstanceOf<RedirectToActionResult>(result);
        }
        [Test]
        public async Task Edit_ReturnsView_WhenModelStateIsInvalid()
        {
            _userManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new ASUser());
            ASEvents events = new ASEvents()
            {
                Id = new Guid().ToString()
            };
            _asDbContext.ASEvents.Add(events);
            _asDbContext.SaveChanges();
            this._eventsController.ModelState.AddModelError("test", "test");
            _userManager.Setup(x => x.IsInRoleAsync(It.IsAny<ASUser>(), It.IsAny<string>())).ReturnsAsync(true);
            var result = await this._eventsController.Edit(events.Id, events);
            Assert.IsInstanceOf<ViewResult>(result);
        }
        [Test]
        public async Task Delete_ReturnsNotFound_WhenIdIsNull()
        {
            var result = await this._eventsController.Delete(null);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }
        [Test]
        public async Task Delete_ReturnsUnAuthorized_WhenIdIsNotOwnedAndRoleIsNotAdmin()
        {
            _userManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new ASUser());
            ASEvents events = new ASEvents()
            {
                Id = new Guid().ToString(),
                UserId = new Guid().ToString()
            };
            var obj = _asDbContext.ASEvents.Add(events);
            _asDbContext.SaveChanges();
            _userManager.Setup(x => x.IsInRoleAsync(It.IsAny<ASUser>(), It.IsAny<string>())).ReturnsAsync(false);
            var result = await this._eventsController.Delete(obj.Entity.Id);
            Assert.IsInstanceOf<UnauthorizedResult>(result);
        }
        [Test]
        public async Task Delete_ReturnsNotFound_WhenEventIsNull()
        {
            _userManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new ASUser());
            ASEvents events = new ASEvents()
            {
                Id = new Guid().ToString()
            };
            _asDbContext.ASEvents.Add(events);
            _asDbContext.SaveChanges();
            _userManager.Setup(x => x.IsInRoleAsync(It.IsAny<ASUser>(), It.IsAny<string>())).ReturnsAsync(true);
            var result = await this._eventsController.Delete("");
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task Delete_ReturnsView_WhenEventIsValid()
        {
            _userManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new ASUser());
            ASEvents events = new ASEvents()
            {
                Id = new Guid().ToString()
            };
            _asDbContext.ASEvents.Add(events);
            _asDbContext.SaveChanges();
            _userManager.Setup(x => x.IsInRoleAsync(It.IsAny<ASUser>(), It.IsAny<string>())).ReturnsAsync(true);
            var result = await this._eventsController.Delete(events.Id);
            Assert.IsInstanceOf<ViewResult>(result);
        }
        [Test]
        public async Task DeleteEvent_ReturnsUnAuthorized_WhenIdIsNotOwnedAndRoleIsNotAdmin()
        {
            _userManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new ASUser());
            ASEvents events = new ASEvents()
            {
                Id = new Guid().ToString(),
                UserId = new Guid().ToString()
            };
            _asDbContext.ASEvents.Add(events);
            _asDbContext.SaveChanges();
            _userManager.Setup(x => x.IsInRoleAsync(It.IsAny<ASUser>(), It.IsAny<string>())).ReturnsAsync(false);
            var result = await this._eventsController.DeleteEvent(events.Id);
            Assert.IsInstanceOf<UnauthorizedResult>(result);
        }
        [Test]
        public async Task DeleteEvent_ReturnsRedirectToAction_WhenEventIsDeleted()
        {
            _userManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new ASUser());
            ASEvents events = new ASEvents()
            {
                Id = new Guid().ToString()
            };
            this._asDbContext.ASEvents.Add(events);
            _asDbContext.SaveChanges();
            _userManager.Setup(x => x.IsInRoleAsync(It.IsAny<ASUser>(), It.IsAny<string>())).ReturnsAsync(true);
            var result = await this._eventsController.DeleteEvent(events.Id);
            Assert.IsInstanceOf<RedirectToActionResult>(result);
        }
        [Test]
        public async Task Subscribe_ReturnsRedirectToAction_WhenUserIsSubscribed()
        {
            _userManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new ASUser() {Id = new Guid().ToString() });
            ASEvents events = new ASEvents()
            {
                Id = new Guid().ToString(),
                UserId = new Guid().ToString()
            };
            this._asDbContext.ASEvents.Add(events);
            _asDbContext.SaveChanges();
            var result = await this._eventsController.Subscribe(events.Id);
            Assert.IsInstanceOf<RedirectToActionResult>(result);
        }
        [Test]
        public async Task UnSubscribe_ReturnsRedirectToAction_WhenUserIsNotSubscribed()
        {
            _userManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new ASUser() { Id = new Guid().ToString() });
            ASEvents events = new ASEvents()
            {
                Id = new Guid().ToString(),
                UserId = new Guid().ToString()
            };
            this._asDbContext.ASEvents.Add(events);
            _asDbContext.SaveChanges();
            var result = await this._eventsController.UnSubscribe(events.Id);
            Assert.IsInstanceOf<RedirectToActionResult>(result);
        }
    }
}
