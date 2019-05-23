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
    public class RollValueServiceTests : ServiceTests<RollValueService, RollValue>
    {
        [Fact]
        public void Create_ValidObject_ReturnsRollValue()
        {
            CreateEntities(
                out List<User> users,
                out List<Room> rooms,
                out List<UserRoom> userRooms,
                out List<Roll> rolls,
                out List<RollValue> rollValues);

            var rollValueService = CreateService(users, rooms, userRooms, rolls, rollValues);
            var rollValue = new RollValue { RollId = 1, MaxValue = 6, Value = 3 };

            var result = rollValueService.Create(rollValue);

            Assert.IsType<RollValue>(result);
        }

        [Fact]
        public void Create_RollNotExist_ThrowRollNotFoundError()
        {
            CreateEntities(
                out List<User> users,
                out List<Room> rooms,
                out List<UserRoom> userRooms,
                out List<Roll> rolls,
                out List<RollValue> rollValues);

            var rollValueService = CreateService(users, rooms, userRooms, rolls, rollValues);
            var rollValue = new RollValue { RollId = 99, MaxValue = 6, Value = 3 };

            Roll roll = new Roll { UserId = 99, RoomId = 1 };

            var exception = Assert.Throws<ApplicationException>(
                () => rollValueService.Create(rollValue));

            Assert.Equal(DiceApi.Properties.resultMessages.RollNotFound, exception.Message);
        }

        public class RollValuesList : IEnumerable<object[]>
        {
            private readonly List<object[]> _data = new List<object[]>
            {
                new object[] { },
                new object[] {new RollValue { Id = 1, RollId = 1, MaxValue = 6, Value = 3} },
                new object[] {new RollValue { Id = 1, RollId = 1, MaxValue = 6, Value = 3},
                    new RollValue { Id = 2, RollId = 2, MaxValue = 6, Value = 1} },
            };
            public IEnumerator<object[]> GetEnumerator() => _data.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [Theory]
        [ClassData(typeof(RollValuesList))]
        public void GetAll_ValidRolls_ReturnsRolls(params RollValue[] rollValuesArray)
        {
            var rollValues = new List<RollValue>();
            rollValues.AddRange(rollValuesArray);
            var rollService = CreateService(rollValues);

            var result = rollService.GetAll();

            Assert.Equal(rollValues.Count, result.Count());
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(3, 2)]
        public void GetRollValues_ValidRoll_ReturnsRollValues(int rollId, int expectedRollValuesCount)
        {
            CreateEntities(
                out List<User> users,
                out List<Room> rooms,
                out List<UserRoom> userRooms,
                out List<Roll> rolls,
                out List<RollValue> rollValues);

            var rollValueService = CreateService(users, rooms, userRooms, rolls, rollValues);
            var roll = new Roll { Id = rollId };

            var result = rollValueService.GetRollValues(roll);

            Assert.Equal(expectedRollValuesCount, result.Count());
        }

        [Theory]
        [InlineData(99, 0)]
        public void GetRollValues_InvalidRoll_ReturnsEmptyRollValues(int rollId, int expectedRollValuesCount)
        {
            CreateEntities(
                out List<User> users,
                out List<Room> rooms,
                out List<UserRoom> userRooms,
                out List<Roll> rolls,
                out List<RollValue> rollValues);

            var rollValueService = CreateService(users, rooms, userRooms, rolls, rollValues);
            var roll = new Roll { Id = rollId };

            var result = rollValueService.GetRollValues(roll);

            Assert.Equal(expectedRollValuesCount, result.Count());
        }
    }
}
