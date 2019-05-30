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
            PasswordHelpers.CreatePasswordHash("User101Password",
                out byte[] user101PasswordHash, out byte[] user101PasswordSalt);
            var user101 = new User
            {
                Id = 101,
                Username = "User101",
                PasswordHash = user101PasswordHash,
                PasswordSalt = user101PasswordSalt
            };
            PasswordHelpers.CreatePasswordHash("User102Password", 
                out byte[] user102PasswordHash, out byte[] user102PasswordSalt);
            var user102 = new User
            {
                Id = 102,
                Username = "User102",
                PasswordHash = user102PasswordHash,
                PasswordSalt = user102PasswordSalt
            };
            PasswordHelpers.CreatePasswordHash("User103Password", 
                out byte[] user103PasswordHash, out byte[] user103PasswordSalt);
            var user103 = new User
            {
                Id = 103,
                Username = "User103",
                PasswordHash = user103PasswordHash,
                PasswordSalt = user103PasswordSalt
            };

            PasswordHelpers.CreatePasswordHash("Room101Password", 
                out byte[] room101PasswordHash, out byte[] room101PasswordSalt);
            var room101 = new Room
            {
                Id = 101,
                Title = "Room101",
                PasswordHash = room101PasswordHash,
                PasswordSalt = room101PasswordSalt
            };
            PasswordHelpers.CreatePasswordHash("Room102Password", 
                out byte[] room102PasswordHash, out byte[] room102PasswordSalt);
            var room102 = new Room
            {
                Id = 102,
                Title = "Room102",
                PasswordHash = room102PasswordHash,
                PasswordSalt = room102PasswordSalt
            };
            PasswordHelpers.CreatePasswordHash("Room103Password",
                out byte[] room103PasswordHash, out byte[] room103PasswordSalt);
            var room103 = new Room
            {
                Id = 103,
                Title = "Room103",
                PasswordHash = room103PasswordHash,
                PasswordSalt = room103PasswordSalt
            };
            PasswordHelpers.CreatePasswordHash("Room104Password",
                out byte[] room104PasswordHash, out byte[] room104PasswordSalt);
            var room104 = new Room
            {
                Id = 104,
                Title = "Room104",
                PasswordHash = room104PasswordHash,
                PasswordSalt = room104PasswordSalt
            };
            PasswordHelpers.CreatePasswordHash("Room105Password",
                out byte[] room105PasswordHash, out byte[] room105PasswordSalt);
            var room105 = new Room
            {
                Id = 105,
                Title = "Room105",
                PasswordHash = room105PasswordHash,
                PasswordSalt = room105PasswordSalt
            };

            var user101Room101 = new UserRoom { User = user101, Room = room101, Owner = false };
            var user101Room102 = new UserRoom { User = user101, Room = room102, Owner = false };
            var user101Room103 = new UserRoom { User = user101, Room = room103, Owner = true };
            var user102Room101 = new UserRoom { User = user102, Room = room101, Owner = true };
            var user102Room104 = new UserRoom { User = user102, Room = room104, Owner = true };
            var user102Room105 = new UserRoom { User = user102, Room = room105, Owner = true };

            user101.UserRooms = new List<UserRoom> { user101Room101, user101Room102, user101Room103 };
            user102.UserRooms = new List<UserRoom> { user102Room101, user102Room104, user102Room105 };
            room101.RoomUsers = new List<UserRoom> { user101Room101, user102Room101 };
            room102.RoomUsers = new List<UserRoom> { user101Room102 };
            room103.RoomUsers = new List<UserRoom> { user101Room103 };
            room104.RoomUsers = new List<UserRoom> { user102Room104 };
            room105.RoomUsers = new List<UserRoom> { user102Room105 };

            users = new List<User> { user101, user102, user103 };
            rooms = new List<Room> { room101, room102, room103, room104, room105 };
            userRooms = new List<UserRoom> { user101Room101, user101Room102, user101Room103, user102Room101, user102Room104, user102Room105 };
        }

        public static void CreateEntities(
            out List<User> users,
            out List<Room> rooms,
            out List<UserRoom> userRooms,
            out List<Roll> rolls,
            out List<RollValue> rollValues)
        {
            CreateEntities(out users, out rooms, out userRooms);
            var user101 = users.Single(x => x.Id == 101);
            var user102 = users.Single(x => x.Id == 102);
            var user103 = users.Single(x => x.Id == 103);

            var room101 = rooms.Single(x => x.Id == 101);
            var room102 = rooms.Single(x => x.Id == 102);

            var roll101 = new Roll { Id = 101, User = user101, Room = room101, Modifier = 0 };
            var roll102 = new Roll { Id = 102, User = user102, Room = room101, Modifier = 5 };
            var roll103 = new Roll { Id = 103, User = user101, Room = room101, Modifier = 0 };
            var roll104 = new Roll { Id = 104, User = user102, Room = room102, Modifier = 0 };

            var rollValue101 = new RollValue { Id = 1, Roll = roll101, MaxValue = 6, Value = 4 };
            var rollValue102 = new RollValue { Id = 2, Roll = roll102, MaxValue = 6, Value = 1 };
            var rollValue103 = new RollValue { Id = 3, Roll = roll103, MaxValue = 6, Value = 6 };
            var rollValue104 = new RollValue { Id = 4, Roll = roll103, MaxValue = 4, Value = 3 };
            var rollValue105 = new RollValue { Id = 5, Roll = roll104, MaxValue = 10, Value = 9 };

            roll101.RollValues = new List<RollValue> { rollValue101 };
            roll102.RollValues = new List<RollValue> { rollValue102 };
            roll103.RollValues = new List<RollValue> { rollValue103, rollValue104 };
            roll104.RollValues = new List<RollValue> { rollValue105 };

            rolls = new List<Roll> { roll101, roll102, roll103, roll104 };
            rollValues = new List<RollValue> { rollValue101, rollValue102, rollValue103, rollValue104, rollValue105 };
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
