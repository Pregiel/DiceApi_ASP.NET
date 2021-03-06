using DiceApi.Entities;
using DiceApi.Helpers;
using DiceApi.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace DiceApiTest.Services
{
    public class UserServiceTests : ServiceTests<UserService, User>
    {
        [Fact]
        public void Authenticate_ValidObject_ReturnsUser()
        {
            PasswordHelpers.CreatePasswordHash("password123", out byte[] passwordHash, out byte[] passwordSalt);
            var users = new List<User> { new User() {
                Id = 1,
                Username = "User123",
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt}
            };
            var userService = CreateService(users);
            string username = "User123";
            string password = "password123";

            var result = userService.Authenticate(username, password);

            Assert.IsType<User>(result);
        }

        [Theory]
        [InlineData(null, "password123")]
        [InlineData("User123", null)]
        [InlineData(null, null)]
        [InlineData("", "password123")]
        [InlineData("User123", "")]
        [InlineData("", "")]
        [InlineData("   ", "password123")]
        [InlineData("User123", "   ")]
        [InlineData("   ", " ")]
        public void Authenticate_NullOrWhiteSpacesCredentials_ThrowsCredentialsInvalidError(string username, string password)
        {
            PasswordHelpers.CreatePasswordHash("password123", out byte[] passwordHash, out byte[] passwordSalt);
            var users = new List<User> { new User() {
                Id = 1,
                Username = "User123",
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt}
            };
            var userService = CreateService(users);

            var exception = Assert.Throws<ApplicationException>(
                () => userService.Authenticate(username, password));

            Assert.Equal(DiceApi.Properties.resultMessages.CredentialsInvalid, exception.Message);
        }

        [Fact]
        public void Authenticate_UserNotExist_ThrowsCredentialsInvalidError()
        {
            var users = new List<User> { };
            var userService = CreateService(users);
            string username = "User123";
            string password = "password123";

            var exception = Assert.Throws<ApplicationException>(
                () => userService.Authenticate(username, password));

            Assert.Equal(DiceApi.Properties.resultMessages.CredentialsInvalid, exception.Message);
        }

        [Fact]
        public void Authenticate_InvalidPassword_ThrowsCredentialsInvalidError()
        {
            PasswordHelpers.CreatePasswordHash("123password", out byte[] passwordHash, out byte[] passwordSalt);
            var users = new List<User> { new User() {
                Id = 1,
                Username = "User123",
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt}
            };
            var userService = CreateService(users);
            string username = "User123";
            string password = "password123";

            var exception = Assert.Throws<ApplicationException>(
                () => userService.Authenticate(username, password));

            Assert.Equal(DiceApi.Properties.resultMessages.CredentialsInvalid, exception.Message);
        }

        [Fact]
        public void Create_ValidObjectPassed_ReturnsUser()
        {
            var users = new List<User> { };
            var userService = CreateService(users);
            User user = new User { Username = "User123" };
            string password = "password123";

            var result = userService.Create(user, password);

            Assert.IsType<User>(result);
            Assert.NotNull(result.PasswordHash);
            Assert.NotNull(result.PasswordSalt);
        }

        [Fact]
        public void Create_DuplicatedUsername_ThrowsUsernameDuplicatedError()
        {
            var users = new List<User> { new User() { Username = "User123" } };
            var userService = CreateService(users);
            User user = new User { Username = "User123" };
            string password = "password123";

            var exception = Assert.Throws<ApplicationException>(
                () => userService.Create(user, password));

            Assert.Equal(DiceApi.Properties.resultMessages.UsernameExists, exception.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_NullOrWhiteSpacesUsername_ThrowsUsernameNullError(string username)
        {
            var users = new List<User> { };
            var userService = CreateService(users);
            User user = new User { Username = username };
            string password = "password123";

            var exception = Assert.Throws<ApplicationException>(
                () => userService.Create(user, password));

            Assert.Equal(DiceApi.Properties.resultMessages.UsernameNull, exception.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_NullOrWhiteSpacesPassword_ThrowsPasswordNullError(string password)
        {
            var users = new List<User> { };
            var userService = CreateService(users);
            User user = new User { Username = "User123" };

            var exception = Assert.Throws<ApplicationException>(
                () => userService.Create(user, password));

            Assert.Equal(DiceApi.Properties.resultMessages.PasswordNull, exception.Message);
        }

        public class UsersList : IEnumerable<object[]>
        {
            private readonly List<object[]> _data = new List<object[]>
            {
                new object[] { },
                new object[] {new User { Id = 1, Username = "User1"} },
                new object[] {new User { Id = 1, Username = "User1"}, new User{Id = 2, Username = "User2"} }
            };
            public IEnumerator<object[]> GetEnumerator() => _data.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [Theory]
        [ClassData(typeof(UsersList))]
        public void GetAll_ValidUsers_ReturnsUsers(params User[] usersArray)
        {
            var users = new List<User>();
            users.AddRange(usersArray);
            var userService = CreateService(users);

            var result = userService.GetAll();

            Assert.Equal(users.Count, result.Count());
        }

        [Fact]
        public void GetById_ValidId_ReturnUser()
        {
            var users = new List<User>
            {
                new User { Id = 1, Username = "User1" },
                new User { Id = 2, Username = "User2" },
                new User { Id = 3, Username = "User3" }
            };
            var userService = CreateService(users);
            int id = 2;

            var result = userService.GetById(id);

            Assert.Equal("User2", result.Username);
        }

        [Fact]
        public void GetById_InvalidId_ReturnNull()
        {
            var users = new List<User>
            {
                new User { Id = 1, Username = "User1" },
                new User { Id = 2, Username = "User2" },
                new User { Id = 3, Username = "User3" }
            };
            var userService = CreateService(users);
            int id = 4;

            var result = userService.GetById(id);

            Assert.Null(result);
        }

        [Fact]
        public void Update_ValidParams_DataUpdated()
        {
            PasswordHelpers.CreatePasswordHash("password123", out byte[] passwordHash, out byte[] passwordSalt);
            var users = new List<User>
            {
                new User { Id = 1, Username = "User1", PasswordHash = passwordHash, PasswordSalt = passwordSalt}
            };
            var userService = CreateService(users);
            User userParam = new User { Id = 1, Username = "NewUser1" };
            string password = "newPassword123";

            userService.Update(userParam, password);

            var user = userService.GetById(1);

            Assert.Equal("NewUser1", user.Username);
            Assert.True(PasswordHelpers.VerifyPasswordHash("newPassword123", user.PasswordHash, user.PasswordSalt));
        }

        [Fact]
        public void Update_InvalidUserId_ThrowsUserNotFoundError()
        {
            var users = new List<User> { new User { Id = 1, Username = "User1" } };
            var userService = CreateService(users);
            User userParam = new User { Id = 9, Username = "NewUser1" };
            string password = "newPassword123";

            var exception = Assert.Throws<ApplicationException>(
                () => userService.Update(userParam, password));

            Assert.Equal(DiceApi.Properties.resultMessages.UserNotFound, exception.Message);
        }

        [Fact]
        public void Delete_ValidId_SaveChangesInvoked()
        {
            var users = new List<User>
            {
                new User { Id = 1, Username = "User1"},
                new User { Id = 2, Username = "User2"},
                new User { Id = 3, Username = "User3"}
            };
            var dataContext = CreateDataContext(users);
            var userService = CreateService(dataContext);
            int id = 2;

            userService.Delete(id);
            dataContext.Verify(x => x.SaveChanges(), Times.Exactly(1));
        }

        [Fact]
        public void Delete_InvalidId_SaveChangesNotInvoked()
        {
            var users = new List<User>
            {
                new User { Id = 1, Username = "User1"},
                new User { Id = 2, Username = "User2"},
                new User { Id = 3, Username = "User3"}
            };
            var dataContext = CreateDataContext(users);
            var userService = CreateService(dataContext);
            int id = 5;

            userService.Delete(id);
            dataContext.Verify(x => x.SaveChanges(), Times.Exactly(0));
        }
    }
}
