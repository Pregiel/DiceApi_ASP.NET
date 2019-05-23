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
    public class RollServiceTests
    {
        private static Mock<DbSet<T>> CreateDbSetMock<T>(IEnumerable<T> elements) where T : class
        {
            var elementsAsQueryable = elements.AsQueryable();
            var dbSetMock = new Mock<DbSet<T>>();

            dbSetMock.As<IQueryable<T>>().Setup(m => m.Provider).Returns(elementsAsQueryable.Provider);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(elementsAsQueryable.Expression);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(elementsAsQueryable.ElementType);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(elementsAsQueryable.GetEnumerator());

            return dbSetMock;
        }
        private RollService CreateRollService(IList<Roll> rolls)
        {
            var dataContextMock = CreateDataContext(rolls);

            return new RollService(dataContextMock.Object);
        }
        private RollService CreateRollService(
            IList<User> users,
            IList<Room> rooms,
            IList<UserRoom> userRooms,
            IList<Roll> rolls,
            IList<RollValue> rollValues)
        {
            var dataContextMock = CreateDataContext(users, rooms, userRooms, rolls, rollValues);

            return new RollService(dataContextMock.Object);
        }
        private Mock<DataContext> CreateDataContext(IList<Roll> rolls)
        {
            var mockUserRoomsSet = CreateDbSetMock(rolls);

            var dataContextMock = new Mock<DataContext>();
            dataContextMock.Setup(x => x.Rolls).Returns(mockUserRoomsSet.Object);

            return dataContextMock;
        }
        private Mock<DataContext> CreateDataContext(
            IList<User> users,
            IList<Room> rooms,
            IList<UserRoom> userRooms,
            IList<Roll> rolls,
            IList<RollValue> rollValues)
        {
            var mockUserRoomsSet = CreateDbSetMock(userRooms);
            var mockUsersSet = CreateDbSetMock(users);
            var mockRoomsSet = CreateDbSetMock(rooms);
            var mockRollsSet = CreateDbSetMock(rolls);
            var mockRollValuesSet = CreateDbSetMock(rollValues);

            var dataContextMock = new Mock<DataContext>();
            dataContextMock.Setup(x => x.Users).Returns(mockUsersSet.Object);
            dataContextMock.Setup(x => x.Rooms).Returns(mockRoomsSet.Object);
            dataContextMock.Setup(x => x.UserRooms).Returns(mockUserRoomsSet.Object);
            dataContextMock.Setup(x => x.Rolls).Returns(mockRollsSet.Object);
            dataContextMock.Setup(x => x.RollValues).Returns(mockRollValuesSet.Object);

            return dataContextMock;
        }

        private void CreateUserAndRoomEntities(
            out List<User> users,
            out List<Room> rooms,
            out List<UserRoom> userRooms)
        {

            var user1 = new User { Id = 1, Username = "User1" };
            var user2 = new User { Id = 2, Username = "User2" };
            var user3 = new User { Id = 3, Username = "User3" };

            var room1 = new Room { Id = 1, Title = "Room1" };
            var room2 = new Room { Id = 2, Title = "Room2" };
            var room3 = new Room { Id = 3, Title = "Room3" };

            var user1Room1 = new UserRoom { User = user1, Room = room1, Owner = false };
            var user1Room2 = new UserRoom { User = user1, Room = room2, Owner = false };
            var user2Room1 = new UserRoom { User = user2, Room = room1, Owner = true };
            var user1Room3 = new UserRoom { User = user1, Room = room3, Owner = true };

            user1.UserRooms = new List<UserRoom> { user1Room1, user1Room2, user1Room3 };
            user2.UserRooms = new List<UserRoom> { user2Room1 };
            room1.RoomUsers = new List<UserRoom> { user1Room1, user2Room1 };
            room2.RoomUsers = new List<UserRoom> { user1Room2 };
            room3.RoomUsers = new List<UserRoom> { user1Room3 };

            users = new List<User> { user1, user2, user3 };
            rooms = new List<Room> { room1, room2, room3 };
            userRooms = new List<UserRoom> { user1Room1, user1Room2, user1Room3, user2Room1 };
        }

        private void CreateEntities(
            out List<User> users,
            out List<Room> rooms,
            out List<UserRoom> userRooms,
            out List<Roll> rolls,
            out List<RollValue> rollValues)
        {
            CreateUserAndRoomEntities(out users, out rooms, out userRooms);
            var user1 = users.Single(x => x.Id == 1);
            var user2 = users.Single(x => x.Id == 2);
            var user3 = users.Single(x => x.Id == 3);

            var room1 = rooms.Single(x => x.Id == 1);
            var room2 = rooms.Single(x => x.Id == 2);

            var roll1 = new Roll { Id = 1, User = user1, Room = room1, Modifier = 0 };
            var roll2 = new Roll { Id = 2, User = user2, Room = room1, Modifier = 5 };
            var roll3 = new Roll { Id = 3, User = user1, Room = room1, Modifier = 0 };
            var roll4 = new Roll { Id = 4, User = user2, Room = room2, Modifier = 0 };

            var rollValue1 = new RollValue { Id = 1, Roll = roll1, MaxValue = 6, Value = 4 };
            var rollValue2 = new RollValue { Id = 2, Roll = roll2, MaxValue = 6, Value = 1 };
            var rollValue3 = new RollValue { Id = 3, Roll = roll3, MaxValue = 6, Value = 6 };
            var rollValue4 = new RollValue { Id = 4, Roll = roll3, MaxValue = 4, Value = 3 };
            var rollValue5 = new RollValue { Id = 5, Roll = roll4, MaxValue = 10, Value = 9 };

            roll1.RollValues = new List<RollValue> { rollValue1 };
            roll2.RollValues = new List<RollValue> { rollValue2 };
            roll3.RollValues = new List<RollValue> { rollValue3, rollValue4 };
            roll4.RollValues = new List<RollValue> { rollValue5 };

            rolls = new List<Roll> { roll1, roll2, roll3, roll4 };
            rollValues = new List<RollValue> { rollValue1, rollValue2, rollValue3, rollValue4, rollValue5 };
        }

        [Fact]
        public void Create_ValidObject_ReturnsRoll()
        {
            CreateEntities(
                out List<User> users,
                out List<Room> rooms,
                out List<UserRoom> userRooms,
                out List<Roll> rolls,
                out List<RollValue> rollValues);

            var rollService = CreateRollService(users, rooms, userRooms, rolls, rollValues);
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

            var rollService = CreateRollService(users, rooms, userRooms, rolls, rollValues);
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

            var rollService = CreateRollService(users, rooms, userRooms, rolls, rollValues);
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
            var rollService = CreateRollService(rolls);

            var result = rollService.GetAll();

            Assert.Equal(rolls.Count, result.Count());
        }

        [Theory]
        [InlineData(1, 3)]
        [InlineData(2, 1)]
        [InlineData(3, 0)]
        public void GetRoomRolls_ValidRolls_ReturnsRolls(int roomId, int expectedRollsCount)
        {
            CreateEntities(
                out List<User> users,
                out List<Room> rooms,
                out List<UserRoom> userRooms,
                out List<Roll> rolls,
                out List<RollValue> rollValues);

            var rollService = CreateRollService(users, rooms, userRooms, rolls, rollValues);
            var room = new Room { Id = roomId };

            var result = rollService.GetRoomRolls(room);

            Assert.Equal(expectedRollsCount, result.Count());
        }

        [Theory]
        [InlineData(99, 0)]
        public void GetRoomRolls_InvalidRolls_ReturnsRolls(int roomId, int expectedRollsCount)
        {
            CreateEntities(
                out List<User> users,
                out List<Room> rooms,
                out List<UserRoom> userRooms,
                out List<Roll> rolls,
                out List<RollValue> rollValues);

            var rollService = CreateRollService(users, rooms, userRooms, rolls, rollValues);
            var room = new Room { Id = roomId };

            var result = rollService.GetRoomRolls(room);

            Assert.Equal(expectedRollsCount, result.Count());
        }
    }
}
