using DiceApi.Dtos;
using DiceApiTest.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace DiceApiTest.Controllers
{
    public class UsersControllerTests : ControllerTests
    { 
        [Fact]
        public async Task Authenticate_ValidUser_ReturnsOkResultWithToken()
        {
            var url = "api/users/authenticate";
            var expected = HttpStatusCode.OK;
            var userDto = new UserDto() { Username = "User101", Password = "User101Password" };

            var response = await Server.PostAsync(url, ContentHelper.GetStringContent(userDto));

            Assert.Equal(expected, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            dynamic json = JsonConvert.DeserializeObject(content);
            Assert.NotNull(json.token);
        }

        [Theory]
        [InlineData("InvalidUser", "User1Password")]
        [InlineData("User1", "InvalidPassword")]
        [InlineData("InvalidUser", "InvalidPassword")]
        [InlineData(null, "User1Password")]
        [InlineData("User101", null)]
        [InlineData(null, null)]
        [InlineData("", "User1Password")]
        [InlineData("User101", "")]
        [InlineData("", "")]
        [InlineData("   ", "User1Password")]
        [InlineData("User101", "   ")]
        [InlineData("   ", " ")]
        public async Task Authenticate_InvalidCredentials_ReturnsBadRequestResultWithCredentialsInvalidError(
            string username, string password)
        {
            var url = "api/users/authenticate";
            var expected = HttpStatusCode.BadRequest;
            var userDto = new UserDto() { Username = username, Password = password };

            var response = await Server.PostAsync(url, ContentHelper.GetStringContent(userDto));

            Assert.Equal(expected, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.Equal(DiceApi.Properties.resultMessages.CredentialsInvalid, content);
        }

        [Fact]
        public async Task Register_ValidUser_ReturnOkResultWithToken()
        {
            var url = "api/users";
            var expected = HttpStatusCode.OK;
            var userDto = new UserDto() { Username = "User999", Password = "User999Password" };

            var response = await Client.PostAsync(url, ContentHelper.GetStringContent(userDto));

            Assert.Equal(expected, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            dynamic json = JsonConvert.DeserializeObject(content);
            Assert.NotNull(json.token);
        }

        [Fact]
        public async Task Register_UsernameNull_ReturnBadRequestResultWithUsernameNullError()
        {
            var url = "api/users";
            var expected = HttpStatusCode.BadRequest;
            var userDto = new UserDto() { Password = "User999Password" };

            var response = await Server.PostAsync(url, ContentHelper.GetStringContent(userDto));

            Assert.Equal(expected, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.Equal(DiceApi.Properties.resultMessages.UsernameNull, content);
        }

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData("user")]
        public async Task Register_UsernameTooShort_ReturnBadRequestResultWithUsernameNullError(
            string username)
        {
            var url = "api/users";
            var expected = HttpStatusCode.BadRequest;
            var userDto = new UserDto() { Username = username, Password = "User999Password" };

            var response = await Server.PostAsync(url, ContentHelper.GetStringContent(userDto));

            Assert.Equal(expected, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.Equal(DiceApi.Properties.resultMessages.UsernameLength, content);
        }

        [Fact]
        public async Task Register_UsernameExists_ReturnBadRequestResultWithUsernameExistsError()
        {
            var url = "api/users";
            var expected = HttpStatusCode.BadRequest;
            var userDto = new UserDto() { Username = "User101", Password = "User999Password" };

            var response = await Server.PostAsync(url, ContentHelper.GetStringContent(userDto));

            Assert.Equal(expected, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.Equal(DiceApi.Properties.resultMessages.UsernameExists, content);
        }

        [Fact]
        public async Task Register_PasswordNull_ReturnBadRequestResultWithPasswordNullError()
        {
            var url = "api/users";
            var expected = HttpStatusCode.BadRequest;
            var userDto = new UserDto() { Username = "User999" };

            var response = await Server.PostAsync(url, ContentHelper.GetStringContent(userDto));

            Assert.Equal(expected, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.Equal(DiceApi.Properties.resultMessages.PasswordNull, content);
        }

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData("pass")]
        public async Task Register_PasswordTooShort_ReturnBadRequestResultWithPasswordLengthError(
            string password)
        {
            var url = "api/users";
            var expected = HttpStatusCode.BadRequest;
            var userDto = new UserDto() { Username = "User999", Password = password };

            var response = await Server.PostAsync(url, ContentHelper.GetStringContent(userDto));

            Assert.Equal(expected, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.Equal(DiceApi.Properties.resultMessages.PasswordLength, content);
        }

        [Fact]
        public async Task Register_UsernameAndPasswordNull_ReturnBadRequestResultWithUsernameNullAndPasswordNullErrors()
        {
            var url = "api/users";
            var expected = HttpStatusCode.BadRequest;
            var userDto = new UserDto() { };

            var response = await Server.PostAsync(url, ContentHelper.GetStringContent(userDto));

            Assert.Equal(expected, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            var errors = content.Split(",");
            Assert.Collection(errors,
                item => Assert.Contains(DiceApi.Properties.resultMessages.UsernameNull, item),
                item => Assert.Contains(DiceApi.Properties.resultMessages.PasswordNull, item));
        }

        [Fact]
        public async Task Register_UsernameAndPasswordTooShort_ReturnBadRequestResultWithUsernameLengthAndPasswordLengthErrors()
        {
            var url = "api/users";
            var expected = HttpStatusCode.BadRequest;
            var userDto = new UserDto() { Username = "user", Password = "pass" };

            var response = await Server.PostAsync(url, ContentHelper.GetStringContent(userDto));

            Assert.Equal(expected, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            var errors = content.Split(",");
            Assert.Collection(errors,
                item => Assert.Contains(DiceApi.Properties.resultMessages.UsernameLength, item),
                item => Assert.Contains(DiceApi.Properties.resultMessages.PasswordLength, item));
        }

        [Fact]
        public async Task Register_UsernameNullAndPasswordTooShort_ReturnBadRequestResultWithUsernameNullAndPasswordLengthErrors()
        {
            var url = "api/users";
            var expected = HttpStatusCode.BadRequest;
            var userDto = new UserDto() { Password = "pass" };

            var response = await Server.PostAsync(url, ContentHelper.GetStringContent(userDto));

            Assert.Equal(expected, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            var errors = content.Split(",");
            Assert.Collection(errors,
                item => Assert.Contains(DiceApi.Properties.resultMessages.UsernameNull, item),
                item => Assert.Contains(DiceApi.Properties.resultMessages.PasswordLength, item));
        }

        [Fact]
        public async Task Register_UsernameTooShortAndPasswordNull_ReturnBadRequestResultWithUsernameLengthAndPasswordNullErrors()
        {
            var url = "api/users";
            var expected = HttpStatusCode.BadRequest;
            var userDto = new UserDto() { Username = "user" };

            var response = await Server.PostAsync(url, ContentHelper.GetStringContent(userDto));

            Assert.Equal(expected, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            var errors = content.Split(",");
            Assert.Collection(errors,
                item => Assert.Contains(DiceApi.Properties.resultMessages.UsernameLength, item),
                item => Assert.Contains(DiceApi.Properties.resultMessages.PasswordNull, item));
        }

        [Fact]
        public async Task GetInfo_Authorized_ReturnOkResultWithUserInfo()
        {
            var url = "api/users/info";
            var expected = HttpStatusCode.OK;

            var response = await Server.GetAuthorizedAsync(url, User101Token);

            Assert.Equal(expected, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            dynamic json = JsonConvert.DeserializeObject(content);
            int? id = json.id;
            string username = json.username;
            JArray rooms = json.rooms;
            Assert.Equal(101, id);
            Assert.Equal("User101", username);
            Assert.Equal(3, rooms.Count);
        }

        [Fact]
        public async Task GetInfo_Unauthorized_ReturnUnauthorizedResult()
        {
            var url = "api/users/info";
            var expected = HttpStatusCode.Unauthorized;

            var response = await Server.GetAsync(url);

            Assert.Equal(expected, response.StatusCode);
        }
    }
}
