using Microsoft.AspNetCore.TestHost;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DiceApiTest.Helpers
{
    public static class RequestExtensions
    {

        public static async Task<HttpResponseMessage> GetAsync(this TestServer server, string requestUri)
        {
            return await server.CreateRequest(requestUri)
                .GetAsync();
        }

        public static async Task<HttpResponseMessage> GetAuthorizedAsync(this TestServer server, string requestUri, string token)
        {
            return await server.CreateRequest(requestUri)
                .AddHeader("Authorization", "Bearer " + token)
                .GetAsync();
        }

        public static async Task<HttpResponseMessage> PostAsync(this TestServer server, string requestUri)
        {
            return await server.CreateRequest(requestUri)
                .PostAsync();
        }

        public static async Task<HttpResponseMessage> PostAsync(this TestServer server, string requestUri, StringContent stringContent)
        {
            return await server.CreateRequest(requestUri)
                .And(x => x.Content = stringContent)
                .PostAsync();
        }

        public static async Task<HttpResponseMessage> PostAuthorizedAsync(this TestServer server, string requestUri, string token)
        {
            return await server.CreateRequest(requestUri)
                .AddHeader("Authorization", "Bearer " + token)
                .PostAsync();
        }

        public static async Task<HttpResponseMessage> PostAuthorizedAsync(
            this TestServer server,
            string requestUri,
            string token,
            StringContent stringContent)
        {
            return await server.CreateRequest(requestUri)
                .AddHeader("Authorization", "Bearer " + token)
                .And(x => x.Content = stringContent)
                .PostAsync();
        }
    }
}
