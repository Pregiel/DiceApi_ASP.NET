using AutoMapper;
using DiceApi.Controllers;
using DiceApi.Dtos;
using DiceApi.Helpers;
using DiceApi.Hubs;
using DiceApi.Services;
using DiceApiTest.Helpers;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace DiceApiTest.Controllers
{
    public class RollsControllerTests : ControllerTests
    {
        [Fact]
        public async Task GetRolls_Authorized_ReturnOkResultWithRoomRolls()
        {
            var url = "api/rooms/101/rolls";
            var expectedResult = HttpStatusCode.OK;
            var expectedRollsCount = Context.Rooms.Single(x => x.Id == 101).Rolls.Count();

            var response = await Server.GetAuthorizedAsync(url, User101Token);

            Assert.Equal(expectedResult, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            JArray json = JsonConvert.DeserializeObject(content) as JArray;
            var rollDtos = json.ToObject<IList<RollDto>>();
            Assert.Equal(expectedRollsCount, rollDtos.Count());
        }

        [Fact]
        public async Task GetRolls_Unauthorized_ReturnUnauthorizedResult()
        {
            var url = "api/rooms/101/rolls";
            var expectedResult = HttpStatusCode.Unauthorized;

            var response = await Server.GetAsync(url);

            Assert.Equal(expectedResult, response.StatusCode);
        }

        [Fact]
        public async Task GetRolls_InvalidRoomId_ReturnBadRequestResultWithRoomNotFoundError()
        {
            var url = "api/rooms/999/rolls";
            var expectedResult = HttpStatusCode.BadRequest;

            var response = await Server.GetAuthorizedAsync(url, User101Token);

            Assert.Equal(expectedResult, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.Equal(DiceApi.Properties.resultMessages.RoomNotFound, content);
        }

        private RollDto CreateRollDto()
        {
            var rollValues = new List<RollValueDto>();
            rollValues.Add(new RollValueDto { MaxValue = 6 });
            rollValues.Add(new RollValueDto { MaxValue = 6 });
            rollValues.Add(new RollValueDto { MaxValue = 20 });
            var rollDto = new RollDto
            {
                Modifier = 9,
                RollValues = rollValues
            };
            return rollDto;
        }

        [Fact]
        public async Task NewRoll_ValidObjects_ReturnOkResultWithRoll()
        {
            var url = "api/rooms/101/rolls";
            var expected = HttpStatusCode.OK;
            var rollDto = CreateRollDto();

            var response = await Server.PostAuthorizedAsync(url, User101Token,
                ContentHelper.GetStringContent(rollDto));

            Assert.Equal(expected, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            JObject json = JsonConvert.DeserializeObject(content) as JObject;
            var returnedRollDto = json.ToObject<RollDto>();
            Assert.NotNull(returnedRollDto);
            Assert.All(returnedRollDto.RollValues,
                x => Assert.True(x.Value <= x.MaxValue));
        }

        [Fact]
        public async Task NewRoll_Unauthorized_ReturnUnauthorizedResult()
        {
            var url = "api/rooms/101/rolls";
            var expectedResult = HttpStatusCode.Unauthorized;
            var rollDto = CreateRollDto();

            var response = await Server.PostAsync(url,
                ContentHelper.GetStringContent(rollDto));

            Assert.Equal(expectedResult, response.StatusCode);
        }

        [Fact]
        public async Task NewRoll_InvalidRoomId_ReturnBadRequestResultWithRoomNotFoundError()
        {
            var url = "api/rooms/999/rolls";
            var expectedResult = HttpStatusCode.BadRequest;
            var rollDto = CreateRollDto();

            var response = await Server.PostAuthorizedAsync(url, User101Token,
                ContentHelper.GetStringContent(rollDto));

            Assert.Equal(expectedResult, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.Equal(DiceApi.Properties.resultMessages.RoomNotFound, content);
        }

        [Fact]
        public async Task NewRoll_UserNotRoomMember_ReturnBadRequestResultWithUserRoomNotFoundError()
        {
            var url = "api/rooms/105/rolls";
            var expectedResult = HttpStatusCode.BadRequest;
            var rollDto = CreateRollDto();

            var response = await Server.PostAuthorizedAsync(url, User101Token,
                ContentHelper.GetStringContent(rollDto));

            Assert.Equal(expectedResult, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.Equal(DiceApi.Properties.resultMessages.UserRoomNotFound, content);
        }
    }
}
