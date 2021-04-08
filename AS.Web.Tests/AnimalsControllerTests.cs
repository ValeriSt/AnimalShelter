using AS.Data;
using AS.Data.Models;
using AS.Web.Controllers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;
using NUnit.Framework;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace AS.Web.Tests
{
    [TestFixture]
    public class AnimalsControllerTests
    {
        private ASDbContext _asDbContext;
        //private Mock<UserManager<ASUser>> _userManager;
        private AnimalsController _animalsController;

        [SetUp]
        public void Setup()
        {
            DbContextOptionsBuilder<ASDbContext> optionsBuilder = new DbContextOptionsBuilder<ASDbContext>()
                .UseInMemoryDatabase(databaseName: "AnimalShelter_Test_Database");
            this._asDbContext = new ASDbContext(optionsBuilder.Options);
            //this._userManager = new Mock<UserManager<ASUser>>();
            //this._animalsController = new AnimalsController(this._asDbContext);
        }
        
        [Test]
        public async Task Create_CreatesAnimalAndReturnsARedirect_WhenModelStateIsValid()
        {
            var UserStoreMock = Mock.Of<IUserStore<ASUser>>();
            var userMgr = new Mock<UserManager<ASUser>>(UserStoreMock, null, null, null, null, null, null, null, null);
            var user = new ASUser() { Id = "f00", Email = "f00@example.com" };
            var tcs = new TaskCompletionSource<ASUser>();
            tcs.SetResult(user);
            userMgr.Setup(x => x.FindByIdAsync("f00")).Returns(tcs.Task);

            ASAnimals model = new ASAnimals
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
            //IActionResult result = await this._animalsController.Create(model);
            ASAnimals actualEntity = await this._asDbContext.ASAnimals.FirstOrDefaultAsync();

            Assert.AreEqual(expectedEntity.AnimalType, actualEntity.AnimalType, "Create() method does not map AnimalType property correctly.");
            Assert.AreEqual(expectedEntity.Name, actualEntity.Name, "Create() method does not map Name property correctly.");
            Assert.AreEqual(expectedEntity.Age, actualEntity.Age, "Create() method does not map Age property correctly.");
            Assert.AreEqual(expectedEntity.Color, actualEntity.Color, "Create() method does not map Color property correctly.");
            Assert.AreEqual(expectedEntity.ImageURL, actualEntity.ImageURL, "Create() method does not map ImageURL property correctly.");
            Assert.AreEqual(expectedEntity.Sex, actualEntity.Sex, "Create() method does not map Sex property correctly.");
            Assert.AreEqual(expectedEntity.Status, actualEntity.Status, "Create() method does not map Status property correctly.");
            Assert.AreEqual(expectedEntity.Location, actualEntity.Location, "Create() method does not map Locartion property correctly.");
            Assert.AreEqual(expectedEntity.DateTime, actualEntity.DateTime, "Create() method does not map DateTime property correctly.");
            Assert.NotNull(actualEntity.Id, "CreateProcessorPart() method does not set Id property correctly.");
        }
        [TearDown]
        public void Dispose()
        {
            this._asDbContext.Dispose();
            this._animalsController = null;
        }

    }
}