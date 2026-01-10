using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Breizh360.Tests.Integration
{
    public class AuthControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public AuthControllerTests(WebApplicationFactory<Program> factory)
        {
            // Create a test server and client. The underlying Program class is
            // exposed via partial class in the API project.
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Login_InvalidCredentials_ReturnsUnauthorized()
        {
            var request = new
            {
                LoginOrEmail = "unknown_user",
                Password = "wrongpassword"
            };

            var response = await _client.PostAsJsonAsync("/auth/login", request);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}