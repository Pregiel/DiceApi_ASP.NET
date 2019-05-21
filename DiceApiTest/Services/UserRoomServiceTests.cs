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
    public class UserRoomServiceTests
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
        private UserRoomService CreateUserRoomService(IList<UserRoom> userRooms)
        {
            var dataContextMock = CreateDataContext(userRooms);

            return new UserRoomService(dataContextMock.Object);
        }
        private UserRoomService CreateUserRoomService(IList<UserRoom> userRooms, IList<User> users)
        {
            var dataContextMock = CreateDataContext(userRooms, users);

            return new UserRoomService(dataContextMock.Object);
        }
        private UserRoomService CreateUserRoomService(IList<UserRoom> userRooms, IList<Room> rooms)
        {
            var dataContextMock = CreateDataContext(userRooms, rooms);

            return new UserRoomService(dataContextMock.Object);
        }
        private UserRoomService CreateUserRoomService(IList<UserRoom> userRooms, IList<User> users, IList<Room> rooms)
        {
            var dataContextMock = CreateDataContext(userRooms, users, rooms);

            return new UserRoomService(dataContextMock.Object);
        }
        private UserRoomService CreateUserRoomService(Mock<DataContext> dataContextMock)
        {
            return new UserRoomService(dataContextMock.Object);
        }
        private Mock<DataContext> CreateDataContext(IList<UserRoom> userRooms)
        {
            var mockUserRoomsSet = CreateDbSetMock(userRooms);

            var dataContextMock = new Mock<DataContext>();
            dataContextMock.Setup(x => x.UserRooms).Returns(mockUserRoomsSet.Object);

            return dataContextMock;
        }
        private Mock<DataContext> CreateDataContext(IList<UserRoom> userRooms, IList<User> users)
        {
            var mockUserRoomsSet = CreateDbSetMock(userRooms);
            var mockUsersSet = CreateDbSetMock(users);

            var dataContextMock = new Mock<DataContext>();
            dataContextMock.Setup(x => x.UserRooms).Returns(mockUserRoomsSet.Object);
            dataContextMock.Setup(x => x.Users).Returns(mockUsersSet.Object);

            return dataContextMock;
        }
        private Mock<DataContext> CreateDataContext(IList<UserRoom> userRooms, IList<Room> rooms)
        {
            var mockUserRoomsSet = CreateDbSetMock(userRooms);
            var mockRoomsSet = CreateDbSetMock(rooms);

            var dataContextMock = new Mock<DataContext>();
            dataContextMock.Setup(x => x.UserRooms).Returns(mockUserRoomsSet.Object);
            dataContextMock.Setup(x => x.Rooms).Returns(mockRoomsSet.Object);

            return dataContextMock;
        }
        private Mock<DataContext> CreateDataContext(IList<UserRoom> userRooms, IList<User> users, IList<Room> rooms)
        {
            var mockUserRoomsSet = CreateDbSetMock(userRooms);
            var mockUsersSet = CreateDbSetMock(users);
            var mockRoomsSet = CreateDbSetMock(rooms);

            var dataContextMock = new Mock<DataContext>();
            dataContextMock.Setup(x => x.UserRooms).Returns(mockUserRoomsSet.Object);
            dataContextMock.Setup(x => x.Users).Returns(mockUsersSet.Object);
            dataContextMock.Setup(x => x.Rooms).Returns(mockRoomsSet.Object);

            return dataContextMock;
        }

        [Fact]
        public void Create_ValidObject_ReturnsUserRoom()
        {
            var userRooms = new List<UserRoom> { };
            var userRoomService = CreateUserRoomService(userRooms);
            User user = new User { Id = 1, Username = "User123" };
            Room room = new Room { Id = 1, Title = "Room123" };
            bool owner = true;

            var result = userRoomService.Create(user, room, owner);

            Assert.IsType<UserRoom>(result);
        }

        [Fact]
        public void Create_AlreadyExists_ThrowsUserRoomExistsError()
        {
            var userRooms = new List<UserRoom> { new UserRoom { UserId = 1, RoomId = 1 } };
            var userRoomService = CreateUserRoomService(userRooms);
            User user = new User { Id = 1, Username = "User123" };
            Room room = new Room { Id = 1, Title = "Room123" };
            bool owner = true;

            var exception = Assert.Throws<ApplicationException>(
                    () => userRoomService.Create(user, room, owner));

            Assert.Equal(DiceApi.Properties.resultMessages.UserRoomExists, exception.Message);
        }

        public class UsersRoomList : IEnumerable<object[]>
        {
            private readonly List<object[]> _data = new List<object[]>
            {
                new object[] { },
                new object[] {new UserRoom { UserId = 1, RoomId = 1 } },
                new object[] { new UserRoom { UserId = 1, RoomId = 1 }, new UserRoom { UserId = 2, RoomId = 1 } }
            };
            public IEnumerator<object[]> GetEnumerator() => _data.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [Theory]
        [ClassData(typeof(UsersRoomList))]
        public void GetAll_ValidUsers_ReturnsUsers(params UserRoom[] userRoomsArray)
        {
            var userRooms = new List<UserRoom>();
            userRooms.AddRange(userRoomsArray);
            var userRoomService = CreateUserRoomService(userRooms);

            var result = userRoomService.GetAll();

            Assert.Equal(userRooms.Count, result.Count());
        }

        private void CreateEntities(out List<User> users, out List<Room> rooms, out List<UserRoom> userRooms)
        {
            var user1 = new User { Id = 1, Username = "User1" };
            var user2 = new User { Id = 2, Username = "User2" };
            var user3 = new User { Id = 3, Username = "User3" };
            var room1 = new Room { Id = 1, Title = "Room1" };
            var room2 = new Room { Id = 2, Title = "Room2" };
            var user1Room1 = new UserRoom { UserId = user1.Id, User = user1, RoomId = room1.Id, Room = room1, Owner = false };
            var user1Room2 = new UserRoom { UserId = user1.Id, User = user1, RoomId = room2.Id, Room = room2, Owner = false };
            var user2Room1 = new UserRoom { UserId = user2.Id, User = user2, RoomId = room1.Id, Room = room1, Owner = true };

            user1.UserRooms = new List<UserRoom> { user1Room1, user1Room2 };
            user2.UserRooms = new List<UserRoom> { user2Room1 };
            room1.RoomUsers = new List<UserRoom> { user1Room1, user2Room1 };
            room2.RoomUsers = new List<UserRoom> { user1Room2 };

            users = new List<User> { user1, user2, user3 };
            rooms = new List<Room> { room1, room2 };
            userRooms = new List<UserRoom> { user1Room1, user1Room2, user2Room1 };
        }

        [Fact]
        public void GetOwner_ValidParams_ReturnsUser()
        {
            CreateEntities(out List<User> users, out List<Room> rooms, out List<UserRoom> userRooms);
            var userRoomService = CreateUserRoomService(userRooms, users, rooms);
            Room roomParam = new Room { Id = 1 };

            var result = userRoomService.GetOwner(roomParam);

            Assert.IsType<User>(result);
        }

        [Fact]
        public void GetOwner_RoomNotExists_ThrowsRoomNotFoundError()
        {
            CreateEntities(out List<User> users, out List<Room> rooms, out List<UserRoom> userRooms);
            var userRoomService = CreateUserRoomService(userRooms, users, rooms);
            Room roomParam = new Room { Id = 5 };

            var exception = Assert.Throws<ApplicationException>(
                () => userRoomService.GetOwner(roomParam));

            Assert.Equal(DiceApi.Properties.resultMessages.RoomNotFound, exception.Message);
        }

        [Fact]
        public void GetOwner_OwnerNotExists_ThrowsUserNotFoundError()
        {
            CreateEntities(out List<User> users, out List<Room> rooms, out List<UserRoom> userRooms);
            var userRoomService = CreateUserRoomService(userRooms, users, rooms);
            Room roomParam = new Room { Id = 2 };

            var exception = Assert.Throws<ApplicationException>(
                () => userRoomService.GetOwner(roomParam));

            Assert.Equal(DiceApi.Properties.resultMessages.UserNotFound, exception.Message);
        }

        [Theory]
        [InlineData(1, 1)]
        public void GetByIds_ValidIds_ReturnsUserRoom(int userId, int roomId)
        {
            CreateEntities(out List<User> users, out List<Room> rooms, out List<UserRoom> userRooms);
            var userRoomService = CreateUserRoomService(userRooms, users, rooms);

            var result = userRoomService.GetByIds(userId, roomId);

            Assert.IsType<UserRoom>(result);
        }

        [Theory]
        [InlineData(99, 1)]
        [InlineData(1, 99)]
        [InlineData(99, 99)]
        public void GetByIds_InvalidIds_ReturnsNull(int userId, int roomId)
        {
            CreateEntities(out List<User> users, out List<Room> rooms, out List<UserRoom> userRooms);
            var userRoomService = CreateUserRoomService(userRooms, users, rooms);

            var result = userRoomService.GetByIds(userId, roomId);

            Assert.Null(result);
        }

        [Theory]
        [InlineData(1)]
        public void GetRoomsByUserId_ValidUserId_ReturnsRoom(int userId)
        {
            CreateEntities(out List<User> users, out List<Room> rooms, out List<UserRoom> userRooms);
            var userRoomService = CreateUserRoomService(userRooms, users, rooms);

            var result = userRoomService.GetRoomsByUserId(userId);

            Assert.IsAssignableFrom<IEnumerable<Room>>(result);
        }

        [Theory]
        [InlineData(99)]
        public void GetRoomsByUserId_InvalidUserId_ThrowsUserNotFoundError(int userId)
        {
            CreateEntities(out List<User> users, out List<Room> rooms, out List<UserRoom> userRooms);
            var userRoomService = CreateUserRoomService(userRooms, users, rooms);

            var exception = Assert.Throws<ApplicationException>(
                () => userRoomService.GetRoomsByUserId(userId));

            Assert.Equal(DiceApi.Properties.resultMessages.UserNotFound, exception.Message);
        }

        [Theory]
        [InlineData(1)]
        public void GetUsersByRoomId_ValidRoomId_ReturnsUsers(int roomId)
        {
            CreateEntities(out List<User> users, out List<Room> rooms, out List<UserRoom> userRooms);
            var userRoomService = CreateUserRoomService(userRooms, users, rooms);

            var result = userRoomService.GetUsersByRoomId(roomId);

            Assert.IsAssignableFrom<IEnumerable<User>>(result);
        }

        [Theory]
        [InlineData(99)]
        public void GetUsersByRoomId_InvalidRoomId_ThrowsRoomNotFoundError(int roomId)
        {
            CreateEntities(out List<User> users, out List<Room> rooms, out List<UserRoom> userRooms);
            var userRoomService = CreateUserRoomService(userRooms, users, rooms);

            var exception = Assert.Throws<ApplicationException>(
                () => userRoomService.GetUsersByRoomId(roomId));

            Assert.Equal(DiceApi.Properties.resultMessages.RoomNotFound, exception.Message);
        }

        [Fact]
        public void ChangeOwner_ValidObjects_OwnerChanged()
        {
            CreateEntities(out List<User> users, out List<Room> rooms, out List<UserRoom> userRooms);
            var userRoomService = CreateUserRoomService(userRooms, users, rooms);
            var newOwner = users.Single(x => x.Id == 1);
            var room = rooms.Single(x => x.Id == 1);

            userRoomService.ChangeOwner(newOwner, room);

            var owner = userRoomService.GetOwner(room);

            Assert.Equal(newOwner.Id, owner.Id);
        }

        [Fact]
        public void ChangeOwner_UserNotInRoom_ThrowsUserRoomNotFoundError()
        {
            CreateEntities(out List<User> users, out List<Room> rooms, out List<UserRoom> userRooms);
            var userRoomService = CreateUserRoomService(userRooms, users, rooms);
            var newOwner = users.Single(x => x.Id == 3);
            var room = rooms.Single(x => x.Id == 1);

            var exception = Assert.Throws<ApplicationException>(
                () => userRoomService.ChangeOwner(newOwner, room));

            Assert.Equal(DiceApi.Properties.resultMessages.UserRoomNotFound, exception.Message);
        }

        [Fact]
        public void DeleteUserFromRoom_ValidObjects_SaveChangesInvoked()
        {
            CreateEntities(out List<User> users, out List<Room> rooms, out List<UserRoom> userRooms);
            var dataContext = CreateDataContext(userRooms, users, rooms);
            var userRoomService = CreateUserRoomService(dataContext);
            var user = users.Single(x => x.Id == 1);
            var room = rooms.Single(x => x.Id == 1);

            userRoomService.DeleteUserFromRoom(user, room);

            dataContext.Verify(x => x.SaveChanges(), Times.Exactly(1));
        }

        [Theory]
        [InlineData(3, 1)] //user not in room
        [InlineData(99, 1)] //user not exist
        [InlineData(1, 99)] //room not exist
        public void DeleteUserFromRoom_InvalidObjects_SaveChangesNotInvoked(int userId, int roomId)
        {
            CreateEntities(out List<User> users, out List<Room> rooms, out List<UserRoom> userRooms);
            var dataContext = CreateDataContext(userRooms, users, rooms);
            var userRoomService = CreateUserRoomService(dataContext);
            var user = users.SingleOrDefault(x => x.Id == userId);
            var room = rooms.SingleOrDefault(x => x.Id == roomId);

            userRoomService.DeleteUserFromRoom(user, room);

            dataContext.Verify(x => x.SaveChanges(), Times.Exactly(0));
        }

        [Fact]
        public void Delete_ValidObjects_SaveChangesInvoked()
        {
            CreateEntities(out List<User> users, out List<Room> rooms, out List<UserRoom> userRooms);
            var dataContext = CreateDataContext(userRooms, users, rooms);
            var userRoomService = CreateUserRoomService(dataContext);
            UserRoom userRoom = userRooms.Single(x => x.UserId == 1 && x.RoomId == 1);

            userRoomService.Delete(userRoom);

            dataContext.Verify(x => x.SaveChanges(), Times.Exactly(1));
        }

        [Theory]
        [InlineData(3, 1)] //user not in room
        [InlineData(99, 1)] //user not exist
        [InlineData(1, 99)] //room not exist
        public void Delete_InvalidObjects_SaveChangesNotInvoked(int userId, int roomId)
        {
            CreateEntities(out List<User> users, out List<Room> rooms, out List<UserRoom> userRooms);
            var dataContext = CreateDataContext(userRooms, users, rooms);
            var userRoomService = CreateUserRoomService(dataContext);
            UserRoom userRoom = userRooms.SingleOrDefault(x => x.UserId == userId && x.RoomId == roomId);

            userRoomService.Delete(userRoom);

            dataContext.Verify(x => x.SaveChanges(), Times.Exactly(0));
        }
    }
}
