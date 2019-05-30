using DiceApi;
using DiceApi.Controllers;
using DiceApi.Dtos;
using DiceApi.Entities;
using DiceApi.Helpers;
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
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Xunit;

namespace DiceApiTest.Controllers
{
    public class UsersControllerIntegrationTests : DatabaseTest, IDisposable
    {
        private readonly TestServer _server;
        private readonly DataContext _context;
        private readonly HttpClient _client;

        private string _token;

        public UsersControllerIntegrationTests()
        {
            var builder = new WebHostBuilder()
                .UseContentRoot(@"C:\Users\Pregiel\source\repos\DiceApi\DiceApiTest")
                .UseEnvironment("Testing")
                .UseStartup<Startup>()
                .UseConfiguration(new ConfigurationBuilder()
                    .SetBasePath(@"C:\Users\Pregiel\source\repos\DiceApi\DiceApiTest")
                    .AddJsonFile("appsettings.json")
                    .Build()
                );

            _server = new TestServer(builder);
            _context = _server.Host.Services.GetService(typeof(DataContext)) as DataContext;
            _client = _server.CreateClient();

            PrepareDatabase();
            SetupAuthenticationToken();
        }

        private void PrepareDatabase()
        {
            CreateEntities(
                out List<User> users,
                out List<Room> rooms,
                out List<UserRoom> userRooms,
                out List<Roll> rolls,
                out List<RollValue> rollValues);

            users.ForEach(x => x.Id = 0);
            rooms.ForEach(x => x.Id = 0);
            rolls.ForEach(x => x.Id = 0);
            rollValues.ForEach(x => x.Id = 0);

            _context.Users.AddRange(users);
            _context.Rooms.AddRange(rooms);
            _context.UserRooms.AddRange(userRooms);
            _context.Rolls.AddRange(rolls);
            _context.RollValues.AddRange(rollValues);
            _context.SaveChanges();
        }

        private void SetupAuthenticationToken()
        {
            var url = "api/users/authenticate";
            var userDto = new UserDto() { Username = "User001", Password = "User001Password" };

            var response = _client.PostAsync(url, ContentHelper.GetStringContent(userDto)).Result;

            var content = response.Content.ReadAsStringAsync().Result;
            dynamic json = JsonConvert.DeserializeObject(content);
            _token = json.token;
        }

        public void Dispose()
        {
            _client.Dispose();
            _context.Dispose();
            _server.Dispose();
        }

        private async Task<HttpResponseMessage> GetAuthorizedAsync(string requestUri)
        {
            return await _server.CreateRequest(requestUri)
                .AddHeader("Authorization", "Bearer " + _token)
                .GetAsync();
        }

        private async Task<HttpResponseMessage> PostAuthorizedAsync(string requestUri)
        {
            return await _server.CreateRequest(requestUri)
                .AddHeader("Authorization", "Bearer " + _token)
                .PostAsync();
        }

        [Fact]
        public async Task Authenticate_ValidUser_ReturnsOkResultWithToken()
        {
            var url = "api/users/authenticate";
            var expected = HttpStatusCode.OK;
            var userDto = new UserDto() { Username = "User001", Password = "User001Password" };

            var response = await _client.PostAsync(url, ContentHelper.GetStringContent(userDto));

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
        [InlineData("User001", null)]
        [InlineData(null, null)]
        [InlineData("", "User1Password")]
        [InlineData("User001", "")]
        [InlineData("", "")]
        [InlineData("   ", "User1Password")]
        [InlineData("User001", "   ")]
        [InlineData("   ", " ")]
        public async Task Authenticate_InvalidCredentials_ReturnsBadRequestResultWithCredentialsInvalidError(
            string username, string password)
        {
            var url = "api/users/authenticate";
            var expected = HttpStatusCode.BadRequest;
            var userDto = new UserDto() { Username = username, Password = password };

            var response = await _client.PostAsync(url, ContentHelper.GetStringContent(userDto));

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

            var response = await _client.PostAsync(url, ContentHelper.GetStringContent(userDto));

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

            var response = await _client.PostAsync(url, ContentHelper.GetStringContent(userDto));

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

            var response = await _client.PostAsync(url, ContentHelper.GetStringContent(userDto));

            Assert.Equal(expected, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.Equal(DiceApi.Properties.resultMessages.UsernameLength, content);
        }

        [Fact]
        public async Task Register_UsernameExists_ReturnBadRequestResultWithUsernameExistsError()
        {
            var url = "api/users";
            var expected = HttpStatusCode.BadRequest;
            var userDto = new UserDto() { Username = "User001", Password = "User999Password" };

            var response = await _client.PostAsync(url, ContentHelper.GetStringContent(userDto));

            Assert.Equal(expected, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.Equal(DiceApi.Properties.resultMessages.UsernameExists, content);
        }

        [Fact]
        public async Task Register_PasswordNull_ReturnBadRequestResultWithPasswordNullError()
        {
            var url = "api/users";
            var expected = HttpStatusCode.BadRequest;
            var userDto = new UserDto() { Username = "User001" };

            var response = await _client.PostAsync(url, ContentHelper.GetStringContent(userDto));

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
            var userDto = new UserDto() { Username = "User001", Password = password };

            var response = await _client.PostAsync(url, ContentHelper.GetStringContent(userDto));

            Assert.Equal(expected, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.Equal(DiceApi.Properties.resultMessages.PasswordLength, content);
        }

        [Fact]
        public async Task Register_InvalidUsernameAndOrPassword_ReturnBadRequestResultWithUsernameAndPasswordErrors()
        {
            var url = "api/users";
            var expected = HttpStatusCode.BadRequest;
            var userDto = new UserDto() { };

            var response = await _client.PostAsync(url, ContentHelper.GetStringContent(userDto));

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

            var response = await _client.PostAsync(url, ContentHelper.GetStringContent(userDto));

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

            var response = await _client.PostAsync(url, ContentHelper.GetStringContent(userDto));

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

            var response = await _client.PostAsync(url, ContentHelper.GetStringContent(userDto));

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

            var response = await GetAuthorizedAsync(url);

            Assert.Equal(expected, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            dynamic json = JsonConvert.DeserializeObject(content);
            int? id = json.id;
            string username = json.username;
            JArray rooms = json.rooms;
            Assert.Equal(1, id);
            Assert.Equal("User001", username);
            Assert.Equal(3, rooms.Count);

        }

        [Fact]
        public async Task GetInfo_Unauthorized_ReturnUserInfo()
        {
            var url = "api/users/info";
            var expected = HttpStatusCode.Unauthorized;

            var response = await _client.GetAsync(url);

            Assert.Equal(expected, response.StatusCode);
        }
    }
}
