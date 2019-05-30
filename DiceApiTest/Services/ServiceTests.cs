using DiceApi.Entities;
using DiceApi.Helpers;
using DiceApi.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;

namespace DiceApiTest.Services
{
    public abstract class ServiceTests<T, U> : DatabaseTest
            where T : Service, new()
            where U : class
    {
        protected T CreateService(IList<U> objects)
        {
            var dataContextMock = CreateDataContext(objects);

            return new T { Context = dataContextMock.Object };
        }
        protected T CreateService(
            IList<User> users,
            IList<Room> rooms,
            IList<UserRoom> userRooms)
        {
            var dataContextMock = CreateDataContext(users, rooms, userRooms);

            return new T { Context = dataContextMock.Object };
        }
        protected T CreateService(
            IList<User> users,
            IList<Room> rooms,
            IList<UserRoom> userRooms,
            IList<Roll> rolls,
            IList<RollValue> rollValues)
        {
            var dataContextMock = CreateDataContext(users, rooms, userRooms, rolls, rollValues);

            return new T { Context = dataContextMock.Object };
        }
        protected T CreateService(Mock<DataContext> dataContextMock)
        {
            return new T { Context = dataContextMock.Object };
        }

        protected Mock<DataContext> CreateDataContext(IList<U> objects)
        {
            return CreateDataContext<U>(objects);
        }
    }
}
