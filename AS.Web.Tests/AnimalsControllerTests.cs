using AS.Data;
using AS.Data.Models;
using AS.Web.Controllers;
using Couchbase.Management.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;
using NUnit.Framework;
using System.Security.Claims;
using System.Threading;

namespace AS.Web.Tests
{
    [TestFixture]
    public class AnimalsControllerTests
    {
        private ASDbContext _asDbContext;
        private Mock<UserManager<IdentityRole>> _userManager = new Mock<IUserManager<IdentityRole>>();
        private AnimalsController _animalsController;

        [SetUp]
        public void Setup()
        {
            DbContextOptionsBuilder<ASDbContext> optionsBuilder = new DbContextOptionsBuilder<ASDbContext>()
                .UseInMemoryDatabase(databaseName: "AnimalShelter_Test_Database");
            this._asDbContext = new ASDbContext(optionsBuilder.Options);
            this._userManager.Setup(x => x.GetUserAsync)
            this._animalsController = new AnimalsController(this._asDbContext, this._userManager);
        }
        [TearDown]
        public void TearDown()
        {
            this._asDbContext.Database.EnsureDeleted();
        }

        [Test]
        public void Create_CreatesAnimalAndReturnsARedirect_WhenModelStateIsValid()
        {
            ASUser user = new ASUser();
            //_userManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            ASAnimals animals = new ASAnimals();
            //_asDbContext.Setup(x => x.Add(It.IsAny<ASAnimals>())).Returns((EntityEntry<ASAnimals>)null);
            //_asDbContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(0);

            IActionResult result = _animalsController.Index() as IActionResult;
            Assert.IsInstanceOf<RedirectToActionResult>(result);
            //Assert.IsNotNull(result);
            Assert.IsNotNull(result);
            Assert.Equals("Index", ((RedirectToActionResult)result).ActionName);
            
        }
    }
}