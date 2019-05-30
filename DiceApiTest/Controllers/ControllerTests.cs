using AutoMapper;
using DiceApi;
using DiceApi.Controllers;
using DiceApi.Dtos;
using DiceApi.Entities;
using DiceApi.Helpers;
using DiceApi.Hubs;
using DiceApi.Services;
using DiceApiTest.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DiceApiTest.Controllers
{
    public class ControllerTests : DatabaseTest, IDisposable
    {
        protected TestServer Server { get; }
        protected DataContext Context { get; }
        protected HttpClient Client { get; }

        protected string User101Token { get; set; }
        public ControllerTests()
        {
            var builder = new WebHostBuilder()
                .UseContentRoot(@"C:\Users\Pregiel\source\repos\DiceApi\DiceApiTest")
                .UseEnvironment("Testing")
                .UseStartup<Startup>()
                .UseConfiguration(new ConfigurationBuilder()
                    .SetBasePath(@"C:\Users\Pregiel\source\repos\DiceApi\DiceApiTest")
                    .AddJsonFile("appsettings.json")
                    .Build()
                );

            Server = new TestServer(builder);
            Context = Server.Host.Services.GetService(typeof(DataContext)) as DataContext;
            Client = Server.CreateClient();

            PrepareDatabase();
            SetupAuthenticationUser1Token();
        }
        private void PrepareDatabase()
        {
            CreateEntities(
                out List<User> users,
                out List<Room> rooms,
                out List<UserRoom> userRooms,
                out List<Roll> rolls,
                out List<RollValue> rollValues);

            //users.ForEach(x => x.Id = 0);
            //rooms.ForEach(x => x.Id = 0);
            //rolls.ForEach(x => x.Id = 0);
            //rollValues.ForEach(x => x.Id = 0);

            Context.Users.AddRange(users);
            Context.Rooms.AddRange(rooms);
            Context.UserRooms.AddRange(userRooms);
            Context.Rolls.AddRange(rolls);
            Context.RollValues.AddRange(rollValues);
            Context.SaveChanges();
        }

        private void SetupAuthenticationUser1Token()
        {
            var url = "api/users/authenticate";
            var userDto = new UserDto() { Username = "User101", Password = "User101Password" };

            var response = Client.PostAsync(url, ContentHelper.GetStringContent(userDto)).Result;

            var content = response.Content.ReadAsStringAsync().Result;
            dynamic json = JsonConvert.DeserializeObject(content);
            User101Token = json.token;
        }

        public void Dispose()
        {
            Client.Dispose();
            Context.Dispose();
            Server.Dispose();
        }

    }
}
