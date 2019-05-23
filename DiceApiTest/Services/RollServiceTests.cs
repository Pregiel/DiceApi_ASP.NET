using DiceApi.Entities;
using DiceApi.Helpers;
using DiceApi.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DiceApiTest.Services
{
    public class RollServiceTests : ServiceTests<RollService, Roll>
    {
        [Fact]
        public void Create_ValidObject_ReturnsRoll()
        {
            CreateEntities(
                out List<User> users,
                out List<Room> rooms,
                out List<UserRoom> userRooms,
                out List<Roll> rolls,
                out List<RollValue> rollValues);

            var rollService = CreateService(users, rooms, userRooms, rolls, rollValues);
            Roll roll = new Roll { UserId = 1, RoomId = 1 };

            var result = rollService.Create(roll);

            Assert.IsType<Roll>(result);
        }

        [Fact]
        public void Create_UserNotExist_ThrowUserNotFoundError()
        {
            CreateEntities(
                out List<User> users,
                out List<Room> rooms,
                out List<UserRoom> userRooms,
                out List<Roll> rolls,
                out List<RollValue> rollValues);

            var rollService = CreateService(users, rooms, userRooms, rolls, rollValues);
            Roll roll = new Roll { UserId = 99, RoomId = 1 };

            var exception = Assert.Throws<ApplicationException>(
                () => rollService.Create(roll));

            Assert.Equal(DiceApi.Properties.resultMessages.UserNotFound, exception.Message);
        }

        [Fact]
        public void Create_RoomNotExist_ThrowRoomNotFoundError()
        {
            CreateEntities(
                out List<User> users,
                out List<Room> rooms,
                out List<UserRoom> userRooms,
                out List<Roll> rolls,
                out List<RollValue> rollValues);

            var rollService = CreateService(users, rooms, userRooms, rolls, rollValues);
            Roll roll = new Roll { UserId = 1, RoomId = 99 };

            var exception = Assert.Throws<ApplicationException>(
                () => rollService.Create(roll));

            Assert.Equal(DiceApi.Properties.resultMessages.RoomNotFound, exception.Message);
        }

        public class RollsList : IEnumerable<object[]>
        {
            private readonly List<object[]> _data = new List<object[]>
            {
                new object[] { },
                new object[] {new Roll { Id = 1, UserId = 1, RoomId = 1} },
                new object[] {new Roll { Id = 1, UserId = 1, RoomId = 1},
                    new Roll { Id = 2, UserId = 2, RoomId = 1} },
            };
            public IEnumerator<object[]> GetEnumerator() => _data.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [Theory]
        [ClassData(typeof(RollsList))]
        public void GetAll_ValidRolls_ReturnsRolls(params Roll[] rollsArray)
        {
            var rolls = new List<Roll>();
            rolls.AddRange(rollsArray);
            var rollService = CreateService(rolls);

            var result = rollService.GetAll();

            Assert.Equal(rolls.Count, result.Count());
        }

        [Theory]
        [InlineData(1, 3)]
        [InlineData(2, 1)]
        [InlineData(3, 0)]
        public void GetRoomRolls_ValidRoom_ReturnsRolls(int roomId, int expectedRollsCount)
        {
            CreateEntities(
                out List<User> users,
                out List<Room> rooms,
                out List<UserRoom> userRooms,
                out List<Roll> rolls,
                out List<RollValue> rollValues);

            var rollService = CreateService(users, rooms, userRooms, rolls, rollValues);
            var room = new Room { Id = roomId };

            var result = rollService.GetRoomRolls(room);

            Assert.Equal(expectedRollsCount, result.Count());
        }

        [Theory]
        [InlineData(99, 0)]
        public void GetRoomRolls_InvalidRoom_ReturnsEmptyRolls(int roomId, int expectedRollsCount)
        {
            CreateEntities(
                out List<User> users,
                out List<Room> rooms,
                out List<UserRoom> userRooms,
                out List<Roll> rolls,
                out List<RollValue> rollValues);

            var rollService = CreateService(users, rooms, userRooms, rolls, rollValues);
            var room = new Room { Id = roomId };

            var result = rollService.GetRoomRolls(room);

            Assert.Equal(expectedRollsCount, result.Count());
        }
    }
}
