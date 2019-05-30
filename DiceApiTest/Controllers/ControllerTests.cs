using AutoMapper;
using DiceApi.Controllers;
using DiceApi.Entities;
using DiceApi.Helpers;
using DiceApi.Hubs;
using DiceApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Text;

namespace DiceApiTest.Controllers
{
    public abstract class ControllerTests<T> : DatabaseTest
            where T : ControllerBase
    {
        protected U CreateService<U>(Mock<DataContext> dataContextMock)
            where U : Service, new()
        {
            return new U { Context = dataContextMock.Object };
        }

        protected T InitController()
        {
            return InitController(null);
        }

        protected T InitController(int? id)
        {
            CreateEntities(
                out List<User> users,
                out List<Room> rooms,
                out List<UserRoom> userRooms,
                out List<Roll> rolls,
                out List<RollValue> rollValues);

            var dataContextMock = CreateDataContext(users, rooms, userRooms, rolls, rollValues);

            var autoMapperProfile = new AutoMapperProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(autoMapperProfile));
            var mapper = new Mapper(configuration);

            dynamic appsettingsJson;
            using (StreamReader file = File.OpenText("appsettings.json"))
            {
                appsettingsJson = JsonConvert.DeserializeObject<dynamic>(file.ReadToEnd());
            }
            string secret = appsettingsJson.AppSettings.Secret;

            var appSettings = new AppSettings() { Secret = secret };
            var appSettingsOptions = Options.Create(appSettings);

            ControllerBase controller = null;
            var type = typeof(T);
            switch (type)
            {
                case Type _ when type == typeof(UsersController):
                    {
                        var userService = CreateService<UserService>(dataContextMock);
                        var userRoomService = CreateService<UserRoomService>(dataContextMock);
                        controller = new UsersController(
                            userService,
                            userRoomService,
                            mapper,
                            appSettingsOptions);

                        break;
                    }

                case Type _ when type == typeof(RoomsController):
                    {
                        var roomService = CreateService<RoomService>(dataContextMock);
                        var userService = CreateService<UserService>(dataContextMock);
                        var userRoomService = CreateService<UserRoomService>(dataContextMock);
                        var rollService = CreateService<RollService>(dataContextMock);

                        var roomHub = new RoomHub(roomService, userService, rollService, mapper);

                        controller = new RoomsController(
                            roomService,
                            userService,
                            userRoomService,
                            mapper,
                            appSettingsOptions);

                        break;
                    }

                case Type _ when type == typeof(RollsController):
                    {
                        break;
                    }
            }

            if (id != null)
            {
                controller.ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                        {
                            new Claim(ClaimTypes.Name, id.ToString())
                        }, "AuthenticationTypes.Federation"))
                    }
                };
            }

            return (T)controller;
        }
    }
}
