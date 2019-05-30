using DiceApi.Dtos;
using DiceApiTest.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace DiceApiTest.Controllers
{
    public class RoomsControllerTests : ControllerTests
    {
        [Fact]
        public async Task GetAll_Authorized_ReturnOkResultWithAllRoomsAsync()
        {
            var url = "api/rooms";
            var expected = HttpStatusCode.OK;

            var response = await Server.GetAuthorizedAsync(url, User101Token);

            Assert.Equal(expected, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            JArray json = JsonConvert.DeserializeObject(content) as JArray;
            Assert.Equal(Context.Rooms.Count(), json.Count);
        }

        [Fact]
        public async Task GetAll_Unauthorized_ReturnUnauthorizedResult()
        {
            var url = "api/rooms";
            var expected = HttpStatusCode.Unauthorized;

            var response = await Server.GetAsync(url);

            Assert.Equal(expected, response.StatusCode);
        }

        [Fact]
        public async Task CreateRoom_ValidRoom_ReturnOkResult()
        {
            var url = "api/rooms";
            var expected = HttpStatusCode.OK;
            var roomDto = new RoomDto() { Title = "Room999", Password = "Room999Password" };

            var response = await Server.PostAuthorizedAsync(url, User101Token, ContentHelper.GetStringContent(roomDto));

            Assert.Equal(expected, response.StatusCode);
        }

        [Fact]
        public async Task CreateRoom_Unathorized_ReturnUnauthorizedResult()
        {
            var url = "api/rooms";
            var expected = HttpStatusCode.Unauthorized;
            var roomDto = new RoomDto() { Title = "Room999", Password = "Room999Password" };

            var response = await Server.PostAsync(url, ContentHelper.GetStringContent(roomDto));

            Assert.Equal(expected, response.StatusCode);
        }

        [Fact]
        public async Task CreateRoom_TitleNull_ReturnBadRequestResultWithTitleNullError()
        {
            var url = "api/rooms";
            var expected = HttpStatusCode.BadRequest;
            var roomDto = new RoomDto() { Password = "Room999Password" };

            var response = await Server.PostAuthorizedAsync(url, User101Token, ContentHelper.GetStringContent(roomDto));

            Assert.Equal(expected, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.Equal(DiceApi.Properties.resultMessages.TitleNull, content);
        }

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData("titl")]
        public async Task CreateRoom_TitleTooShort_ReturnBadRequestResultWithTitleLengthError(
            string title)
        {
            var url = "api/rooms";
            var expected = HttpStatusCode.BadRequest;
            var roomDto = new RoomDto() { Title = title, Password = "Room999Password" };

            var response = await Server.PostAuthorizedAsync(url, User101Token, ContentHelper.GetStringContent(roomDto));

            Assert.Equal(expected, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.Equal(DiceApi.Properties.resultMessages.TitleLength, content);
        }

        [Fact]
        public async Task CreateRoom_PasswordNull_ReturnBadRequestResultWithPasswordNullError()
        {
            var url = "api/rooms";
            var expected = HttpStatusCode.BadRequest;
            var roomDto = new RoomDto() { Title = "Room999" };

            var response = await Server.PostAuthorizedAsync(url, User101Token, ContentHelper.GetStringContent(roomDto));

            Assert.Equal(expected, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.Equal(DiceApi.Properties.resultMessages.PasswordNull, content);
        }

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData("pass")]
        public async Task CreateRoom_PasswordTooShort_ReturnBadRequestResultWithPasswordLengthError(
            string password)
        {
            var url = "api/rooms";
            var expected = HttpStatusCode.BadRequest;
            var roomDto = new RoomDto() { Title = "Room999", Password = password };

            var response = await Server.PostAuthorizedAsync(url, User101Token, ContentHelper.GetStringContent(roomDto));

            Assert.Equal(expected, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.Equal(DiceApi.Properties.resultMessages.PasswordLength, content);
        }

        [Fact]
        public async Task CreateRoom_TitleAndPasswordNull_ReturnBadRequestResultWithTitleNullAndPasswordNullErrors()
        {
            var url = "api/rooms";
            var expected = HttpStatusCode.BadRequest;
            var roomDto = new RoomDto() { };

            var response = await Server.PostAuthorizedAsync(url, User101Token, ContentHelper.GetStringContent(roomDto));

            Assert.Equal(expected, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            var errors = content.Split(",");
            Assert.Collection(errors,
                item => Assert.Contains(DiceApi.Properties.resultMessages.TitleNull, item),
                item => Assert.Contains(DiceApi.Properties.resultMessages.PasswordNull, item));
        }

        [Fact]
        public async Task CreateRoom_TitleAndPasswordTooShort_ReturnBadRequestResultWithTitleLengthAndPasswordLengthErrors()
        {
            var url = "api/rooms";
            var expected = HttpStatusCode.BadRequest;
            var roomDto = new RoomDto() { Title = "titl", Password = "pass" };

            var response = await Server.PostAuthorizedAsync(url, User101Token, ContentHelper.GetStringContent(roomDto));

            Assert.Equal(expected, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            var errors = content.Split(",");
            Assert.Collection(errors,
                item => Assert.Contains(DiceApi.Properties.resultMessages.TitleLength, item),
                item => Assert.Contains(DiceApi.Properties.resultMessages.PasswordLength, item));
        }

        [Fact]
        public async Task CreateRoom_TitleNullAndPasswordTooShort_ReturnBadRequestResultWithTitleNullAndPasswordTooShortErrors()
        {
            var url = "api/rooms";
            var expected = HttpStatusCode.BadRequest;
            var roomDto = new RoomDto() { Password = "pass" };

            var response = await Server.PostAuthorizedAsync(url, User101Token, ContentHelper.GetStringContent(roomDto));

            Assert.Equal(expected, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            var errors = content.Split(",");
            Assert.Collection(errors,
                item => Assert.Contains(DiceApi.Properties.resultMessages.TitleNull, item),
                item => Assert.Contains(DiceApi.Properties.resultMessages.PasswordLength, item));
        }

        [Fact]
        public async Task CreateRoom_TitleTooShortAndPasswordNull_ReturnBadRequestResultWithTitleLengthAndPasswordNullErrors()
        {
            var url = "api/rooms";
            var expected = HttpStatusCode.BadRequest;
            var roomDto = new RoomDto() { Title = "titl" };

            var response = await Server.PostAuthorizedAsync(url, User101Token, ContentHelper.GetStringContent(roomDto));

            Assert.Equal(expected, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            var errors = content.Split(",");
            Assert.Collection(errors,
                item => Assert.Contains(DiceApi.Properties.resultMessages.TitleLength, item),
                item => Assert.Contains(DiceApi.Properties.resultMessages.PasswordNull, item));
        }

        [Theory]
        [InlineData(101)]
        [InlineData(102)]
        [InlineData(103)]
        public async Task GetById_ValidRoomId_ReturnOkResultWithRoomDetails(int roomId)
        {
            var url = "api/rooms/" + roomId;
            var expected = HttpStatusCode.OK;

            var response = await Server.GetAuthorizedAsync(url, User101Token);

            Assert.Equal(expected, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            var roomDetails = JsonConvert.DeserializeObject<RoomDetailsDto>(content);
            Assert.IsType<RoomDetailsDto>(roomDetails);
        }

        [Fact]
        public async Task GetById_Unauthorized_ReturnUnauthorizedResult()
        {
            var url = "api/rooms/101";
            var expected = HttpStatusCode.Unauthorized;

            var response = await Server.GetAsync(url);

            Assert.Equal(expected, response.StatusCode);
        }

        [Fact]
        public async Task GetById_InvalidRoomId_ReturnBadRequestResultWithRoomNotFoundError()
        {
            var url = "api/rooms/9999";
            var expected = HttpStatusCode.BadRequest;

            var response = await Server.GetAuthorizedAsync(url, User101Token);

            Assert.Equal(expected, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.Equal(DiceApi.Properties.resultMessages.RoomNotFound, content);
        }

        [Fact]
        public async Task GetById_NotJoinedRoom_ReturnUnauthorizedResult()
        {
            var url = "api/rooms/104";
            var expected = HttpStatusCode.BadRequest;

            var response = await Server.GetAuthorizedAsync(url, User101Token);

            Assert.Equal(expected, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.Equal(DiceApi.Properties.resultMessages.UserRoomNotFound, content);
        }

        [Fact]
        public async Task Join_ValidObjectNotJoinedRoom_ReturnOkResultWithRoomDetails()
        {
            var url = "api/rooms/105";
            var expected = HttpStatusCode.OK;
            var roomDto = new RoomDto() { Password = "Room105Password" };

            var response = await Server.PostAuthorizedAsync(url, User101Token, ContentHelper.GetStringContent(roomDto));

            Assert.Equal(expected, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            var roomDetails = JsonConvert.DeserializeObject<RoomDetailsDto>(content);
            Assert.IsType<RoomDetailsDto>(roomDetails);
        }

        [Fact]
        public async Task Join_ValidObjectAlreadyJoinedRoom_ReturnOkResultWithRoomDetails()
        {
            var url = "api/rooms/101";
            var expected = HttpStatusCode.OK;
            var roomDto = new RoomDto() { };

            var response = await Server.PostAuthorizedAsync(url, User101Token, ContentHelper.GetStringContent(roomDto));

            Assert.Equal(expected, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            var roomDetails = JsonConvert.DeserializeObject<RoomDetailsDto>(content);
            Assert.IsType<RoomDetailsDto>(roomDetails);
        }

        [Fact]
        public async Task Join_Unauthorized_ReturnUnauthorizedResult()
        {
            var url = "api/rooms/105";
            var expected = HttpStatusCode.Unauthorized;
            var roomDto = new RoomDto() { Password = "Room105Password" };

            var response = await Server.PostAsync(url, ContentHelper.GetStringContent(roomDto));

            Assert.Equal(expected, response.StatusCode);
        }

        [Fact]
        public async Task Join_InvalidRoomId_ReturnBadRequestResultWithRoomNotFoundError()
        {
            var url = "api/rooms/9999";
            var expected = HttpStatusCode.BadRequest;
            var roomDto = new RoomDto() { Password = "Room105Password" };

            var response = await Server.PostAuthorizedAsync(url, User101Token, ContentHelper.GetStringContent(roomDto));

            Assert.Equal(expected, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.Equal(DiceApi.Properties.resultMessages.RoomNotFound, content);
        }

        [Fact]
        public async Task Join_InvalidPasswordNotJoinedRoom_ReturnBadRequestWithCredentialsInvalid()
        {
            var url = "api/rooms/104";
            var expected = HttpStatusCode.BadRequest;
            var roomDto = new RoomDto() { Password = "InvalidPassword" };

            var response = await Server.PostAuthorizedAsync(url, User101Token, ContentHelper.GetStringContent(roomDto));

            Assert.Equal(expected, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.Equal(DiceApi.Properties.resultMessages.CredentialsInvalid, content);
        }

        [Fact]
        public async Task Join_PasswordNullNotJoinedRoom_ReturnBadRequestWithPasswordNull()
        {
            var url = "api/rooms/104";
            var expected = HttpStatusCode.BadRequest;
            var roomDto = new RoomDto() { };

            var response = await Server.PostAuthorizedAsync(url, User101Token, ContentHelper.GetStringContent(roomDto));

            Assert.Equal(expected, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.Equal(DiceApi.Properties.resultMessages.PasswordNull, content);
        }
    }
}
