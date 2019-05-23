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
    public class RoomServiceTests : ServiceTests<RoomService, Room>
    {
        [Fact]
        public void Authenticate_ValidObjectPassed_ReturnsRoom()
        {
            PasswordHelpers.CreatePasswordHash("password123", out byte[] passwordHash, out byte[] passwordSalt);
            var rooms = new List<Room> { new Room() {
                Id = 1,
                Title = "Room123",
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt}
            };
            var roomService = CreateService(rooms);
            int id = 1;
            string password = "password123";

            var result = roomService.Authenticate(id, password);

            Assert.IsType<Room>(result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Authenticate_NullOrWhiteSpacesPassoword_ThrowsCredentialsInvalidError(string password)
        {
            PasswordHelpers.CreatePasswordHash("password123", out byte[] passwordHash, out byte[] passwordSalt);
            var rooms = new List<Room> { new Room() {
                Id = 1,
                Title = "Room123",
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt}
            };
            var roomService = CreateService(rooms);
            int id = 1;

            var exception = Assert.Throws<ApplicationException>(
                () => roomService.Authenticate(id, password));

            Assert.Equal(DiceApi.Properties.resultMessages.CredentialsInvalid, exception.Message);
        }

        [Fact]
        public void Authenticate_RoomNotExist_ThrowsRoomNotFoundError()
        {
            var rooms = new List<Room> { };
            var roomService = CreateService(rooms);
            int id = 1;
            string password = "password123";

            var exception = Assert.Throws<ApplicationException>(
                () => roomService.Authenticate(id, password));

            Assert.Equal(DiceApi.Properties.resultMessages.RoomNotFound, exception.Message);
        }

        [Fact]
        public void Authenticate_InvalidPassword_ThrowsCredentialsInvalidError()
        {
            PasswordHelpers.CreatePasswordHash("123password", out byte[] passwordHash, out byte[] passwordSalt);
            var rooms = new List<Room> { new Room() {
                Id = 1,
                Title = "Room123",
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt}
            };
            var roomService = CreateService(rooms);
            int id = 1;
            string password = "password123";

            var exception = Assert.Throws<ApplicationException>(
                () => roomService.Authenticate(id, password));

            Assert.Equal(DiceApi.Properties.resultMessages.CredentialsInvalid, exception.Message);
        }

        [Fact]
        public void Create_ValidObject_ReturnsRoom()
        {
            var rooms = new List<Room> { };
            var roomService = CreateService(rooms);
            Room room = new Room { Title = "Room123" };
            string password = "password123";

            var result = roomService.Create(room, password);

            Assert.IsType<Room>(result);
            Assert.NotNull(result.PasswordHash);
            Assert.NotNull(result.PasswordSalt);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_NullOrWhiteSpacesTitle_ThrowsTitleNullError(string title)
        {
            var rooms = new List<Room> { };
            var roomService = CreateService(rooms);
            Room room = new Room { Title = title };
            string password = "password123";

            var exception = Assert.Throws<ApplicationException>(
                () => roomService.Create(room, password));

            Assert.Equal(DiceApi.Properties.resultMessages.TitleNull, exception.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_NullOrWhiteSpacesPassword_ThrowsPasswordNullError(string password)
        {
            var rooms = new List<Room> { };
            var roomService = CreateService(rooms);
            Room room = new Room { Title = "Room123" };

            var exception = Assert.Throws<ApplicationException>(
                () => roomService.Create(room, password));

            Assert.Equal(DiceApi.Properties.resultMessages.PasswordNull, exception.Message);
        }

        public class RoomsList : IEnumerable<object[]>
        {
            private readonly List<object[]> _data = new List<object[]>
            {
                new object[] { },
                new object[] {new Room { Id = 1, Title = "Room1"} },
                new object[] {new Room { Id = 1, Title = "Room1" }, new Room { Id = 2, Title = "Room2" } }
            };
            public IEnumerator<object[]> GetEnumerator() => _data.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [Theory]
        [ClassData(typeof(RoomsList))]
        public void GetAll_ValidRooms_ReturnsRooms(params Room[] roomsArray)
        {
            var rooms = new List<Room>();
            rooms.AddRange(roomsArray);
            var roomService = CreateService(rooms);

            var result = roomService.GetAll();

            Assert.Equal(rooms.Count, result.Count());
        }

        [Fact]
        public void GetById_ValidId_ReturnRoom()
        {
            var rooms = new List<Room>
            {
                new Room { Id = 1, Title = "Room1" },
                new Room { Id = 2, Title = "Room2" },
                new Room { Id = 3, Title = "Room3" }
            };
            var roomService = CreateService(rooms);
            int id = 2;

            var result = roomService.GetById(id);

            Assert.Equal("Room2", result.Title);
        }

        [Fact]
        public void GetById_InvalidId_RetunNull()
        {
            var rooms = new List<Room>
            {
                new Room { Id = 1, Title = "Room1" },
                new Room { Id = 2, Title = "Room2" },
                new Room { Id = 3, Title = "Room3" }
            };
            var roomService = CreateService(rooms);
            int id = 4;

            var result = roomService.GetById(id);

            Assert.Null(result);
        }

        [Fact]
        public void Update_ValidParams_DataUpdated()
        {
            PasswordHelpers.CreatePasswordHash("password123", out byte[] passwordHash, out byte[] passwordSalt);
            var rooms = new List<Room>
            {
                new Room { Id = 1, Title = "Room1", PasswordHash = passwordHash, PasswordSalt = passwordSalt}
            };
            var roomService = CreateService(rooms);
            Room roomParam = new Room { Id = 1, Title = "NewRoom1" };
            string password = "newPassword123";

            roomService.Update(roomParam, password);

            var room = roomService.GetById(1);

            Assert.Equal("NewRoom1", room.Title);
            Assert.True(PasswordHelpers.VerifyPasswordHash("newPassword123", room.PasswordHash, room.PasswordSalt));
        }

        [Fact]
        public void Update_InvalidRoomId_ThrowsRoomNotFoundError()
        {
            var rooms = new List<Room> { new Room { Id = 1, Title = "Room1" } };
            var roomService = CreateService(rooms);
            Room roomParam = new Room { Id = 9, Title = "NewRoom1" };
            string password = "newPassword123";

            var exception = Assert.Throws<ApplicationException>(
                () => roomService.Update(roomParam, password));

            Assert.Equal(DiceApi.Properties.resultMessages.RoomNotFound, exception.Message);
        }

        [Fact]
        public void Delete_ValidId_SaveChangesInvoked()
        {
            var rooms = new List<Room>
            {
                new Room { Id = 1, Title = "Room1" },
                new Room { Id = 2, Title = "Room2" },
                new Room { Id = 3, Title = "Room3" }
            };
            var dataContext = CreateDataContext(rooms);
            var roomService = CreateService(dataContext);
            int id = 2;

            roomService.Delete(id);

            dataContext.Verify(x => x.SaveChanges(), Times.Exactly(1));
        }

        [Fact]
        public void Delete_InvalidId_SaveChangesNotInvoked()
        {
            var rooms = new List<Room>
            {
                new Room { Id = 1, Title = "Room1" },
                new Room { Id = 2, Title = "Room2" },
                new Room { Id = 3, Title = "Room3" }
            };
            var dataContext = CreateDataContext(rooms);
            var roomService = CreateService(dataContext);
            int id = 4;

            roomService.Delete(id);

            dataContext.Verify(x => x.SaveChanges(), Times.Exactly(0));
        }
    }
}
