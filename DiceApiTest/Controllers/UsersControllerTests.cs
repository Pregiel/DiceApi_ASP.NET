using AutoMapper;
using DiceApi.Controllers;
using DiceApi.Helpers;
using DiceApi.Services;
using Microsoft.Extensions.Options;
using Moq;
using System;
using Xunit;

namespace DiceApiTest.Controllers
{
    public class UsersControllerTests : IDisposable
    {
        private MockRepository mockRepository;

        private Mock<IUserService> mockUserService;
        private Mock<IUserRoomService> mockUserRoomService;
        private Mock<IMapper> mockMapper;
        private Mock<IOptions<AppSettings>> mockOptions;

        public UsersControllerTests()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);

            this.mockUserService = this.mockRepository.Create<IUserService>();
            this.mockUserRoomService = this.mockRepository.Create<IUserRoomService>();
            this.mockMapper = this.mockRepository.Create<IMapper>();
            this.mockOptions = this.mockRepository.Create<IOptions<AppSettings>>();
        }

        public void Dispose()
        {
            this.mockRepository.VerifyAll();
        }

        private UsersController CreateUsersController()
        {
            return new UsersController(
                this.mockUserService.Object,
                this.mockUserRoomService.Object,
                this.mockMapper.Object,
                this.mockOptions.Object);
        }

        //[Fact]
        //public void Authenticate_StateUnderTest_ExpectedBehavior()
        //{
        //    // Arrange
        //    var unitUnderTest = this.CreateUsersController();
        //    UserDto userDto = TODO;

        //    // Act
        //    var result = unitUnderTest.Authenticate(
        //        userDto);

        //    // Assert
        //    Assert.True(false);
        //}

        //[Fact]
        //public void Register_StateUnderTest_ExpectedBehavior()
        //{
        //    // Arrange
        //    var unitUnderTest = this.CreateUsersController();
        //    UserDto userDto = TODO;

        //    // Act
        //    var result = unitUnderTest.Register(
        //        userDto);

        //    // Assert
        //    Assert.True(false);
        //}

        //[Fact]
        //public void GetAll_StateUnderTest_ExpectedBehavior()
        //{
        //    // Arrange
        //    var unitUnderTest = this.CreateUsersController();

        //    // Act
        //    var result = unitUnderTest.GetAll();

        //    // Assert
        //    Assert.True(false);
        //}

        //[Fact]
        //public void GetInfo_StateUnderTest_ExpectedBehavior()
        //{
        //    // Arrange
        //    var unitUnderTest = this.CreateUsersController();

        //    // Act
        //    var result = unitUnderTest.GetInfo();

        //    // Assert
        //    Assert.True(false);
        //}
    }
}
