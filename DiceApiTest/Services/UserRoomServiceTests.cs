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
    public class UserRoomServiceTests : ServiceTests<UserRoomService, UserRoom>
    {
        [Fact]
        public void Create_ValidObject_ReturnsUserRoom()
        {
            var userRooms = new List<UserRoom> { };
            var userRoomService = CreateService(userRooms);
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
            var userRoomService = CreateService(userRooms);
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
            var userRoomService = CreateService(userRooms);

            var result = userRoomService.GetAll();

            Assert.Equal(userRooms.Count, result.Count());
        }

        [Fact]
        public void GetOwner_ValidParams_ReturnsUser()
        {
            CreateEntities(out List<User> users, out List<Room> rooms, out List<UserRoom> userRooms);
            var userRoomService = CreateService(users, rooms, userRooms);
            Room roomParam = new Room { Id = 1 };

            var result = userRoomService.GetOwner(roomParam);

            Assert.IsType<User>(result);
        }

        [Fact]
        public void GetOwner_RoomNotExists_ThrowsRoomNotFoundError()
        {
            CreateEntities(out List<User> users, out List<Room> rooms, out List<UserRoom> userRooms);
            var userRoomService = CreateService(users, rooms, userRooms);
            Room roomParam = new Room { Id = 5 };

            var exception = Assert.Throws<ApplicationException>(
                () => userRoomService.GetOwner(roomParam));

            Assert.Equal(DiceApi.Properties.resultMessages.RoomNotFound, exception.Message);
        }

        [Fact]
        public void GetOwner_OwnerNotExists_ThrowsUserNotFoundError()
        {
            CreateEntities(out List<User> users, out List<Room> rooms, out List<UserRoom> userRooms);
            var userRoomService = CreateService(users, rooms, userRooms);
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
            var userRoomService = CreateService(users, rooms, userRooms);

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
            var userRoomService = CreateService(users, rooms, userRooms);

            var result = userRoomService.GetByIds(userId, roomId);

            Assert.Null(result);
        }

        [Theory]
        [InlineData(1)]
        public void GetRoomsByUserId_ValidUserId_ReturnsRoom(int userId)
        {
            CreateEntities(out List<User> users, out List<Room> rooms, out List<UserRoom> userRooms);
            var userRoomService = CreateService(users, rooms, userRooms);

            var result = userRoomService.GetRoomsByUserId(userId);

            Assert.IsAssignableFrom<IEnumerable<Room>>(result);
        }

        [Theory]
        [InlineData(99)]
        public void GetRoomsByUserId_InvalidUserId_ThrowsUserNotFoundError(int userId)
        {
            CreateEntities(out List<User> users, out List<Room> rooms, out List<UserRoom> userRooms);
            var userRoomService = CreateService(users, rooms, userRooms);

            var exception = Assert.Throws<ApplicationException>(
                () => userRoomService.GetRoomsByUserId(userId));

            Assert.Equal(DiceApi.Properties.resultMessages.UserNotFound, exception.Message);
        }

        [Theory]
        [InlineData(1)]
        public void GetUsersByRoomId_ValidRoomId_ReturnsUsers(int roomId)
        {
            CreateEntities(out List<User> users, out List<Room> rooms, out List<UserRoom> userRooms);
            var userRoomService = CreateService(users, rooms, userRooms);

            var result = userRoomService.GetUsersByRoomId(roomId);

            Assert.IsAssignableFrom<IEnumerable<User>>(result);
        }

        [Theory]
        [InlineData(99)]
        public void GetUsersByRoomId_InvalidRoomId_ThrowsRoomNotFoundError(int roomId)
        {
            CreateEntities(out List<User> users, out List<Room> rooms, out List<UserRoom> userRooms);
            var userRoomService = CreateService(users, rooms, userRooms);

            var exception = Assert.Throws<ApplicationException>(
                () => userRoomService.GetUsersByRoomId(roomId));

            Assert.Equal(DiceApi.Properties.resultMessages.RoomNotFound, exception.Message);
        }

        [Fact]
        public void ChangeOwner_ValidObjects_OwnerChanged()
        {
            CreateEntities(out List<User> users, out List<Room> rooms, out List<UserRoom> userRooms);
            var userRoomService = CreateService(users, rooms, userRooms);
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
            var userRoomService = CreateService(users, rooms, userRooms);
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
            var dataContext = CreateDataContext(users, rooms, userRooms);
            var userRoomService = CreateService(dataContext);
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
            var dataContext = CreateDataContext(users, rooms, userRooms);
            var userRoomService = CreateService(dataContext);
            var user = users.SingleOrDefault(x => x.Id == userId);
            var room = rooms.SingleOrDefault(x => x.Id == roomId);

            userRoomService.DeleteUserFromRoom(user, room);

            dataContext.Verify(x => x.SaveChanges(), Times.Exactly(0));
        }

        [Fact]
        public void Delete_ValidObjects_SaveChangesInvoked()
        {
            CreateEntities(out List<User> users, out List<Room> rooms, out List<UserRoom> userRooms);
            var dataContext = CreateDataContext(users, rooms, userRooms);
            var userRoomService = CreateService(dataContext);
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
            var dataContext = CreateDataContext(users, rooms, userRooms);
            var userRoomService = CreateService(dataContext);
            UserRoom userRoom = userRooms.SingleOrDefault(x => x.UserId == userId && x.RoomId == roomId);

            userRoomService.Delete(userRoom);

            dataContext.Verify(x => x.SaveChanges(), Times.Exactly(0));
        }
    }
}
