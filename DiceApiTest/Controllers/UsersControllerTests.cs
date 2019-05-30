using DiceApi;
using DiceApi.Controllers;
using DiceApi.Dtos;
using DiceApi.Entities;
using DiceApiTest.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Respawn;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Xunit;

namespace DiceApiTest.Controllers
{
    public class UsersControllerTests : ControllerTests<UsersController>
    {
        [Fact]
        public void Authenticate_ValidUser_ReturnsOkResultWithToken()
        {
            var userController = InitController();
            var userDto = new UserDto() { Username = "User001", Password = "User001Password" };

            var result = userController.Authenticate(userDto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var token = okResult.Value.GetType().GetProperty("Token");
            Assert.NotNull(token);

        }

        [Theory]
        [InlineData("InvalidUser", "User1Password")]
        [InlineData("User1", "InvalidPassword")]
        [InlineData("InvalidUser", "InvalidPassword")]
        [InlineData(null, "User1Password")]
        [InlineData("User001", null)]
        [InlineData(null, null)]
        [InlineData("", "User1Password")]
        [InlineData("User001", "")]
        [InlineData("", "")]
        [InlineData("   ", "User1Password")]
        [InlineData("User001", "   ")]
        [InlineData("   ", " ")]
        public void Authenticate_InvalidCredentials_ReturnsBadRequestResultWithCredentialsInvalidError(
            string username, string password)
        {
            var userController = InitController();
            var userDto = new UserDto() { Username = username, Password = password };

            var result = userController.Authenticate(userDto);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(DiceApi.Properties.resultMessages.CredentialsInvalid, badRequestResult.Value);
        }

        [Fact]
        public void Register_ValidUser_ReturnOkResultWithToken()
        {
            var userController = InitController();
            var userDto = new UserDto() { Username = "User999", Password = "User999Password" };

            var result = userController.Register(userDto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var token = okResult.Value.GetType().GetProperty("Token");
            Assert.NotNull(token);
        }

        [Fact]
        public void Register_UsernameNull_ReturnBadRequestResultWithUsernameNullError()
        {
            var userController = InitController();
            var userDto = new UserDto() { Password = "User999Password" };

            var result = userController.Register(userDto);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(DiceApi.Properties.resultMessages.UsernameNull, badRequestResult.Value);
        }

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData("user")]
        public void Register_UsernameTooShort_ReturnBadRequestResultWithUsernameNullError(
            string username)
        {
            var userController = InitController();
            var userDto = new UserDto() { Username = username, Password = "User999Password" };

            var result = userController.Register(userDto);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(DiceApi.Properties.resultMessages.UsernameLength, badRequestResult.Value);
        }

        [Fact]
        public void Register_UsernameExists_ReturnBadRequestResultWithUsernameExistsError()
        {
            var userController = InitController();
            var userDto = new UserDto() { Username = "User001", Password = "User999Password" };

            var result = userController.Register(userDto);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(DiceApi.Properties.resultMessages.UsernameExists, badRequestResult.Value);
        }

        [Fact]
        public void Register_PasswordNull_ReturnBadRequestResultWithPasswordNullError()
        {
            var userController = InitController();
            var userDto = new UserDto() { Username = "User999" };

            var result = userController.Register(userDto);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(DiceApi.Properties.resultMessages.PasswordNull, badRequestResult.Value);
        }

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData("pass")]
        public void Register_PasswordTooShort_ReturnBadRequestResultWithPasswordLengthError(
            string password)
        {
            var userController = InitController();
            var userDto = new UserDto() { Username = "User999", Password = password };

            var result = userController.Register(userDto);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(DiceApi.Properties.resultMessages.PasswordLength, badRequestResult.Value);
        }

        [Fact]
        public void Register_InvalidUsernameAndOrPassword_ReturnBadRequestResultWithUsernameAndPasswordErrors()
        {
            var userController = InitController();
            var userDto = new UserDto() { };

            var result = userController.Register(userDto);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errors = badRequestResult.Value.ToString().Split(",");
            Assert.Collection(errors,
                item => Assert.Contains(DiceApi.Properties.resultMessages.UsernameNull, item),
                item => Assert.Contains(DiceApi.Properties.resultMessages.PasswordNull, item));
        }

        [Fact]
        public void Register_UsernameAndPasswordTooShort_ReturnBadRequestResultWithUsernameLengthAndPasswordLengthErrors()
        {
            var userController = InitController();
            var userDto = new UserDto() { Username = "user", Password = "pass" };

            var result = userController.Register(userDto);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errors = badRequestResult.Value.ToString().Split(",");
            Assert.Collection(errors,
                item => Assert.Contains(DiceApi.Properties.resultMessages.UsernameLength, item),
                item => Assert.Contains(DiceApi.Properties.resultMessages.PasswordLength, item));
        }

        [Fact]
        public void Register_UsernameNullAndPasswordTooShort_ReturnBadRequestResultWithUsernameNullAndPasswordLengthErrors()
        {
            var userController = InitController();
            var userDto = new UserDto() { Password = "pass" };

            var result = userController.Register(userDto);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errors = badRequestResult.Value.ToString().Split(",");
            Assert.Collection(errors,
                item => Assert.Contains(DiceApi.Properties.resultMessages.UsernameNull, item),
                item => Assert.Contains(DiceApi.Properties.resultMessages.PasswordLength, item));
        }

        [Fact]
        public void Register_UsernameTooShortAndPasswordNull_ReturnBadRequestResultWithUsernameLengthAndPasswordNullErrors()
        {
            var userController = InitController();
            var userDto = new UserDto() { Username = "user" };

            var result = userController.Register(userDto);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errors = badRequestResult.Value.ToString().Split(",");
            Assert.Collection(errors,
                item => Assert.Contains(DiceApi.Properties.resultMessages.UsernameLength, item),
                item => Assert.Contains(DiceApi.Properties.resultMessages.PasswordNull, item));
        }

        [Fact]
        public void GetInfo_Authorized_ReturnOkResultWithUserInfo()
        {
            var userController = InitController(1);

            var result = userController.GetInfo();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var id = okResult.Value.GetType().GetProperty("id").GetValue(okResult.Value) as int?;
            var username = okResult.Value.GetType().GetProperty("username").GetValue(okResult.Value) as string;
            var rooms = okResult.Value.GetType().GetProperty("rooms").GetValue(okResult.Value) as IList<RoomInfoDto>;
            Assert.Equal(1, id);
            Assert.Equal("User001", username);
            Assert.Equal(3, rooms.Count);
        }

        [Fact]
        public void GetInfo_Unauthorized_ReturnUnauthorizedResult()
        {
            var userController = InitController(99);

            var result = userController.GetInfo();

            Assert.IsType<UnauthorizedResult>(result);
        }
    }
}
