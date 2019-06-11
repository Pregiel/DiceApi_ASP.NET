using DiceApi.Dtos;
using DiceApiTest.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace DiceApiTest.Controllers
{
    public class UsersControllerTests : ControllerTests
    { 
        [Fact]
        public async Task Login_ValidUser_ReturnsOkResultWithToken()
        {
            var url = "api/users/login";
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
        public async Task Login_InvalidCredentials_ReturnsBadRequestResultWithCredentialsInvalidError(
            string username, string password)
        {
            var url = "api/users/login";
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
        [InlineData("uuu")]
        [InlineData("TKky97bkQlP16d26TKky97bkQlP16d261")]
        public async Task Register_UsernameInvalidLength_ReturnBadRequestResultWithUsernameNullError(
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
        [InlineData("TKky97bkQlP16d26TKky97bkQlP16d261")]
        public async Task Register_PasswordInvalidLength_ReturnBadRequestResultWithPasswordLengthError(
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

        [Theory]
        [InlineData("uuu", "ppppp")]
        [InlineData("qfA8N94sEJkaEzLqqfA8N94sEJkaEzLq1", "TKky97bkQlP16d26TKky97bkQlP16d262")]
        public async Task Register_UsernameAndPasswordInvalidLenghts_ReturnBadRequestResultWithUsernameLengthAndPasswordLengthErrors(
            string username, string password)
        {
            var url = "api/users";
            var expected = HttpStatusCode.BadRequest;
            var userDto = new UserDto() { Username = username, Password = password };

            var response = await Server.PostAsync(url, ContentHelper.GetStringContent(userDto));

            Assert.Equal(expected, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            var errors = content.Split(",");
            Assert.Collection(errors,
                item => Assert.Contains(DiceApi.Properties.resultMessages.UsernameLength, item),
                item => Assert.Contains(DiceApi.Properties.resultMessages.PasswordLength, item));
        }

        [Theory]
        [InlineData("ppppp")]
        [InlineData("qfA8N94sEJkaEzLqqfA8N94sEJkaEzLq1")]
        public async Task Register_UsernameNullAndPasswordInvalidLength_ReturnBadRequestResultWithUsernameNullAndPasswordLengthErrors(string password)
        {
            var url = "api/users";
            var expected = HttpStatusCode.BadRequest;
            var userDto = new UserDto() { Password = password };

            var response = await Server.PostAsync(url, ContentHelper.GetStringContent(userDto));

            Assert.Equal(expected, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            var errors = content.Split(",");
            Assert.Collection(errors,
                item => Assert.Contains(DiceApi.Properties.resultMessages.UsernameNull, item),
                item => Assert.Contains(DiceApi.Properties.resultMessages.PasswordLength, item));
        }

        [Theory]
        [InlineData("uuu")]
        [InlineData("qfA8N94sEJkaEzLqqfA8N94sEJkaEzLq1")]
        public async Task Register_UsernameInvalidLengthAndPasswordNull_ReturnBadRequestResultWithUsernameLengthAndPasswordNullErrors(string username)
        {
            var url = "api/users";
            var expected = HttpStatusCode.BadRequest;
            var userDto = new UserDto() { Username = username };

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
            var url = "api/users";
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
            var url = "api/users";
            var expected = HttpStatusCode.Unauthorized;

            var response = await Server.GetAsync(url);

            Assert.Equal(expected, response.StatusCode);
        }

        [Fact]
        public async Task GetMyRooms_Unauthorized_ReturnUnauthorizedResult()
        {
            var url = "api/users/myRooms";
            var expected = HttpStatusCode.Unauthorized;

            var response = await Server.GetAsync(url);

            Assert.Equal(expected, response.StatusCode);
        }

        [Fact]
        public async Task GetMyRooms_AuthorizedNoPageAndLimit_ReturnOkResultWithRoomsAndSize()
        {
            var url = "api/users/myRooms";
            var expected = HttpStatusCode.OK;
            var expectedRoomsCount = Context.UserRooms.Where(x => x.UserId == 101).Count();

            var response = await Server.GetAuthorizedAsync(url, User101Token);

            Assert.Equal(expected, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            dynamic json = JsonConvert.DeserializeObject(content);
            JArray rooms = json.rooms;
            int? size = json.size;
            Assert.Equal(expectedRoomsCount, rooms.Count);
            Assert.Equal(expectedRoomsCount, size);
        }

        [Fact]
        public async Task GetMyRooms_AuthorizedWithPageAndLimit_ReturnOkResultWithRoomsAndSize()
        {
            var url = "api/users/myRooms?page=1&limit=2";
            var expected = HttpStatusCode.OK;
            var expectedRoomsCount = Context.UserRooms.Where(x => x.UserId == 101).Count();

            var response = await Server.GetAuthorizedAsync(url, User101Token);

            Assert.Equal(expected, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            dynamic json = JsonConvert.DeserializeObject(content);
            JArray rooms = json.rooms;
            int? size = json.size;
            Assert.Equal(2, rooms.Count);
            Assert.Equal(expectedRoomsCount, size);
        }

        [Fact]
        public async Task GetMyRooms_AuthorizedWithPageAndNoLimit_ReturnOkResultWithRoomsAndSize()
        {
            var url = "api/users/myRooms?page=1";
            var expected = HttpStatusCode.OK;
            var expectedRoomsCount = Context.UserRooms.Where(x => x.UserId == 101).Count();

            var response = await Server.GetAuthorizedAsync(url, User101Token);

            Assert.Equal(expected, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            dynamic json = JsonConvert.DeserializeObject(content);
            JArray rooms = json.rooms;
            int? size = json.size;
            Assert.Equal(expectedRoomsCount, rooms.Count);
            Assert.Equal(expectedRoomsCount, size);
        }

        [Fact]
        public async Task GetMyRooms_AuthorizedWithLimitAndNoPage_ReturnOkResultWithRoomsAndSize()
        {
            var url = "api/users/myRooms?limit=2";
            var expected = HttpStatusCode.OK;
            var expectedRoomsCount = Context.UserRooms.Where(x => x.UserId == 101).Count();

            var response = await Server.GetAuthorizedAsync(url, User101Token);

            Assert.Equal(expected, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            dynamic json = JsonConvert.DeserializeObject(content);
            JArray rooms = json.rooms;
            int? size = json.size;
            Assert.Equal(2, rooms.Count);
            Assert.Equal(expectedRoomsCount, size);
        }
    }
}
