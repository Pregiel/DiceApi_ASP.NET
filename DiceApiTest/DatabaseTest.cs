using DiceApi.Entities;
using DiceApi.Helpers;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiceApiTest
{
    public abstract class DatabaseTest
    {
        protected static Mock<DbSet<Y>> CreateDbSetMock<Y>(IEnumerable<Y> elements)
            where Y : class
        {
            var elementsAsQueryable = elements.AsQueryable();
            var dbSetMock = new Mock<DbSet<Y>>();

            dbSetMock.As<IQueryable<Y>>().Setup(m => m.Provider).Returns(elementsAsQueryable.Provider);
            dbSetMock.As<IQueryable<Y>>().Setup(m => m.Expression).Returns(elementsAsQueryable.Expression);
            dbSetMock.As<IQueryable<Y>>().Setup(m => m.ElementType).Returns(elementsAsQueryable.ElementType);
            dbSetMock.As<IQueryable<Y>>().Setup(m => m.GetEnumerator()).Returns(elementsAsQueryable.GetEnumerator());

            return dbSetMock;
        }

        public static void CreateEntities(
            out List<User> users,
            out List<Room> rooms,
            out List<UserRoom> userRooms)
        {
            PasswordHelpers.CreatePasswordHash("User001Password",
                out byte[] user001PasswordHash, out byte[] user001PasswordSalt);
            var user1 = new User
            {
                Id = 1,
                Username = "User001",
                PasswordHash = user001PasswordHash,
                PasswordSalt = user001PasswordSalt
            };
            PasswordHelpers.CreatePasswordHash("User002Password", 
                out byte[] user002PasswordHash, out byte[] user002PasswordSalt);
            var user2 = new User
            {
                Id = 2,
                Username = "User002",
                PasswordHash = user002PasswordHash,
                PasswordSalt = user002PasswordSalt
            };
            PasswordHelpers.CreatePasswordHash("User003Password", 
                out byte[] user003PasswordHash, out byte[] user003PasswordSalt);
            var user3 = new User
            {
                Id = 3,
                Username = "User003",
                PasswordHash = user003PasswordHash,
                PasswordSalt = user003PasswordSalt
            };

            PasswordHelpers.CreatePasswordHash("Room001Password", 
                out byte[] room001PasswordHash, out byte[] room001PasswordSalt);
            var room1 = new Room
            {
                Id = 1,
                Title = "Room001",
                PasswordHash = room001PasswordHash,
                PasswordSalt = room001PasswordSalt
            };
            PasswordHelpers.CreatePasswordHash("Room002Password", 
                out byte[] room002PasswordHash, out byte[] room002PasswordSalt);
            var room2 = new Room
            {
                Id = 2,
                Title = "Room002",
                PasswordHash = room002PasswordHash,
                PasswordSalt = room002PasswordSalt
            };
            PasswordHelpers.CreatePasswordHash("Room003Password", 
                out byte[] room003PasswordHash, out byte[] room003PasswordSalt);
            var room3 = new Room
            {
                Id = 3,
                Title = "Room003",
                PasswordHash = room003PasswordHash,
                PasswordSalt = room003PasswordSalt
            };

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

        public static void CreateEntities(
            out List<User> users,
            out List<Room> rooms,
            out List<UserRoom> userRooms,
            out List<Roll> rolls,
            out List<RollValue> rollValues)
        {
            CreateEntities(out users, out rooms, out userRooms);
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

        protected Mock<DataContext> CreateDataContext<U>(IList<U> objects) where U : class
        {
            var mockUserRoomsSet = CreateDbSetMock(objects);
            var dataContextMock = new Mock<DataContext>();

            var type = typeof(U);
            switch (type)
            {
                case Type _ when type == typeof(User):
                    dataContextMock.Setup(x => x.Users).Returns(mockUserRoomsSet.Object as DbSet<User>);
                    break;

                case Type _ when type == typeof(Room):
                    dataContextMock.Setup(x => x.Rooms).Returns(mockUserRoomsSet.Object as DbSet<Room>);
                    break;

                case Type _ when type == typeof(UserRoom):
                    dataContextMock.Setup(x => x.UserRooms).Returns(mockUserRoomsSet.Object as DbSet<UserRoom>);
                    break;

                case Type _ when type == typeof(Roll):
                    dataContextMock.Setup(x => x.Rolls).Returns(mockUserRoomsSet.Object as DbSet<Roll>);
                    break;

                case Type _ when type == typeof(RollValue):
                    dataContextMock.Setup(x => x.RollValues).Returns(mockUserRoomsSet.Object as DbSet<RollValue>);
                    break;
            }

            return dataContextMock;
        }
        protected Mock<DataContext> CreateDataContext(
            IList<User> users,
            IList<Room> rooms,
            IList<UserRoom> userRooms)
        {
            var mockUserRoomsSet = CreateDbSetMock(userRooms);
            var mockUsersSet = CreateDbSetMock(users);
            var mockRoomsSet = CreateDbSetMock(rooms);

            var dataContextMock = new Mock<DataContext>();
            dataContextMock.Setup(x => x.Users).Returns(mockUsersSet.Object);
            dataContextMock.Setup(x => x.Rooms).Returns(mockRoomsSet.Object);
            dataContextMock.Setup(x => x.UserRooms).Returns(mockUserRoomsSet.Object);

            return dataContextMock;
        }
        protected Mock<DataContext> CreateDataContext(
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
    }
}
