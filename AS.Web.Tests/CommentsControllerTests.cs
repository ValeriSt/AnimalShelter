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
    public class CommentsControllerTests
    {
        private ASDbContext _asDbContext;
        private Mock<UserManager<ASUser>> _userManager;
        private CommentsController _commentsController;

        [SetUp]
        public void Setup()
        {
            DbContextOptionsBuilder<ASDbContext> optionsBuilder = new DbContextOptionsBuilder<ASDbContext>()
                .UseInMemoryDatabase(databaseName: "AnimalShelter_Test_Database");
            this._asDbContext = new ASDbContext(optionsBuilder.Options);
            var UserStoreMock = Mock.Of<IUserStore<ASUser>>();
            this._userManager = new Mock<UserManager<ASUser>>(UserStoreMock, null, null, null, null, null, null, null, null);
            this._commentsController = new CommentsController(this._asDbContext, _userManager.Object);
        }

        [Test]
        public async Task Create_CreatesCommentAndReturnsARedirect_WhenModelStateIsValid()
        {
            _userManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new ASUser());

            ASComments expectedEntity = new ASComments
            {          
                DateTime = new System.DateTime(2008, 3, 1, 7, 0, 0),
                Text = "Idk"
            };
            IActionResult result = await this._commentsController.Create(expectedEntity);
            ASComments actualEntity = await this._asDbContext.ASComments.FirstOrDefaultAsync();

            expectedEntity.Id = actualEntity.Id;
            Assert.AreEqual(JsonSerializer.Serialize(expectedEntity), JsonSerializer.Serialize(actualEntity), "Create() method does not map properties correctly.");
            Assert.NotNull(actualEntity.Id, "Create() method does not set Id property correctly.");
            Assert.IsInstanceOf<RedirectResult>(result);
        }
        [Test]
        public async Task Create_ReturnsView_WhenModelStateIsntValid()
        {
            ASComments comments = new ASComments();
            this._commentsController.ModelState.AddModelError("test", "test");
            var result = await this._commentsController.Create(comments);
            Assert.IsInstanceOf<ViewResult>(result);
        }
        [Test]
        public async Task Edit_ReturnsNotFound_WhenIdIsNull()
        {
            var result = await this._commentsController.Edit(null);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }
        [Test]
        public async Task Edit_ReturnsUnauthorized_WhenCommentIsntAuthorized()
        {
            _userManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new ASUser());
            ASComments comments = new ASComments()
            {
                Id = new Guid().ToString(),
                UserId = new Guid().ToString()
            };
            _asDbContext.ASComments.Add(comments);
            _asDbContext.SaveChanges();
            _userManager.Setup(x => x.IsInRoleAsync(It.IsAny<ASUser>(), It.IsAny<string>())).ReturnsAsync(false);
            var result = await this._commentsController.Edit(comments.Id);
            Assert.IsInstanceOf<UnauthorizedResult>(result);
        }
        [Test]
        public async Task Edit_ReturnsNotFound_WhenCommentIsNull()
        {
            _userManager.Setup(x => x.IsInRoleAsync(It.IsAny<ASUser>(), It.IsAny<string>())).ReturnsAsync(true);
            var result = await this._commentsController.Edit("");
            Assert.IsInstanceOf<NotFoundResult>(result);
        }
        [Test]
        public async Task Edit_ReturnsView_WhenCommentIsValid()
        {
            ASComments comments = new ASComments()
            {
                Id = new Guid().ToString()
            };
            _asDbContext.ASComments.Add(comments);
            _asDbContext.SaveChanges();
            _userManager.Setup(x => x.IsInRoleAsync(It.IsAny<ASUser>(), It.IsAny<string>())).ReturnsAsync(true);
            var result = await this._commentsController.Edit(comments.Id);
            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public async Task Edit_ReturnsNotFound_WhenIdIsNotTheSameAsCommentsId()
        {
            ASComments comments = new ASComments()
            {
                Id = new Guid().ToString()
            };
            var result = await this._commentsController.Edit("", comments);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }
        [Test]
        public async Task Edit_ReturnsUnAuthorized_WhenCommentIsntAuthorized()
        {
            _userManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new ASUser());
            ASComments comments = new ASComments()
            {
                Id = new Guid().ToString(),
                UserId = new Guid().ToString()
            };
            _asDbContext.ASComments.Add(comments);
            _asDbContext.SaveChanges();
            _userManager.Setup(x => x.IsInRoleAsync(It.IsAny<ASUser>(), It.IsAny<string>())).ReturnsAsync(false);
            var result = await this._commentsController.Edit(comments.Id, comments);
            Assert.IsInstanceOf<UnauthorizedResult>(result);
        }
        //TODO Edit_ReturnsView and Edit_Try/Catch
        [Test]
        public async Task Delete_ReturnsNotFound_WhenIdIsNull()
        {         
            var result = await this._commentsController.Delete(null);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }
        [Test]
        public async Task Delete_ReturnsUnAuthorized_WhenIdIsNotAuthorized()
        {
            _userManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new ASUser());
            ASComments comments = new ASComments()
            {
                Id = new Guid().ToString(),
                UserId = new Guid().ToString()
            };
            var obj = _asDbContext.ASComments.Add(comments);
            _asDbContext.SaveChanges();
            _userManager.Setup(x => x.IsInRoleAsync(It.IsAny<ASUser>(), It.IsAny<string>())).ReturnsAsync(false);
            var result = await this._commentsController.Delete(obj.Entity.Id);
            Assert.IsInstanceOf<UnauthorizedResult>(result);
        }
        [Test]
        public async Task Delete_ReturnsNotFound_WhenCommentIsNull()
        {
            _userManager.Setup(x => x.IsInRoleAsync(It.IsAny<ASUser>(), It.IsAny<string>())).ReturnsAsync(true);
            var result = await this._commentsController.Delete("");
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task Delete_ReturnsView_WhenAnimalIsValid()
        {
            ASComments comments = new ASComments()
            {
                Id = "foo"
            };
            var obj = this._asDbContext.ASComments.Add(comments);
            _asDbContext.SaveChanges();
            _userManager.Setup(x => x.IsInRoleAsync(It.IsAny<ASUser>(), It.IsAny<string>())).ReturnsAsync(true);
            var result = await this._commentsController.Delete(obj.Entity.Id);
            Assert.IsInstanceOf<ViewResult>(result);
        }
        [Test]
        public async Task DeleteComment_ReturnsUnAuthorized_WhenCommentIdIsUnAuthorized()
        {
            _userManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new ASUser());
            ASComments comments = new ASComments()
            {
                Id = new Guid().ToString(),
                UserId = new Guid().ToString()
            };
            _asDbContext.ASComments.Add(comments);
            _asDbContext.SaveChanges();
            _userManager.Setup(x => x.IsInRoleAsync(It.IsAny<ASUser>(), It.IsAny<string>())).ReturnsAsync(false);
            var result = await this._commentsController.DeleteComment(comments.Id);
            Assert.IsInstanceOf<UnauthorizedResult>(result);
        }
        [Test]
        public async Task DeleteAnimal_ReturnsRedirect_WhenCommentIsDeleted()
        {
            ASComments comments = new ASComments()
            {
                Id = new Guid().ToString()
            };            
            this._asDbContext.ASComments.Add(comments);
            _asDbContext.SaveChanges();
            _userManager.Setup(x => x.IsInRoleAsync(It.IsAny<ASUser>(), It.IsAny<string>())).ReturnsAsync(true);
            var result = await this._commentsController.DeleteComment(comments.Id);
            Assert.IsInstanceOf<RedirectResult>(result);
        }
    }
}
