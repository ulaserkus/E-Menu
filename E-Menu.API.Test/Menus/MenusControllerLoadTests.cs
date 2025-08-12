using Microsoft.AspNetCore.Mvc.Testing;
using NBomber.CSharp;
using Shared.Kernel.Requests;
using System.Text;
using System.Text.Json;

namespace E_Menu.API.Test.Menus
{
    public class MenusControllerLoadTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private const string BaseUrl = "/api/menus/";
        private static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public MenusControllerLoadTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        private static StringContent CreateJsonContent<T>(T body)
        {
            return new StringContent(JsonSerializer.Serialize(body, JsonSerializerOptions), Encoding.UTF8, "application/json");
        }

        private static HttpRequestMessage CreateHttpRequest<T>(string endpoint, T body, string fakeIp)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, BaseUrl + endpoint)
            {
                Content = CreateJsonContent(body)
            };
            request.Headers.Add("X-Forwarded-For", fakeIp); // Simulate different IPs for load testing
            return request;
        }

        [Fact]
        public void LoadTest_GetAllMenus()
        {
            var body = new GetAllMenuItemsRequest();
            var scenarioName = "Load Test for GetAllMenus";

            // Create a scenario for the load test
            var scenario = Scenario.Create(scenarioName, async context =>
            {
                var fakeIp = $"192.168.0.{context.InvocationNumber % 254 + 1}";
                var response = await _client.SendAsync(CreateHttpRequest("All", body, fakeIp));

                if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                {
                    // Optional: Treat rate limiting as expected behavior
                    return Response.Ok(
                        statusCode: "429 TooManyRequests",
                        message: "Rate limiting as expected"
                    );
                }

                return response.IsSuccessStatusCode
                         ? Response.Ok(
                             statusCode: response.StatusCode.ToString(),
                             sizeBytes: (int)(response.Content?.Headers?.ContentLength ?? 0),
                             payload: await response.Content?.ReadAsStringAsync() ?? string.Empty
                         )
                         : Response.Fail(
                             statusCode: response.StatusCode.ToString(),
                             message: $"Error: {response.ReasonPhrase}",
                             sizeBytes: (int)(response.Content?.Headers?.ContentLength ?? 0)
                         );

            })
            .WithWarmUpDuration(TimeSpan.FromSeconds(30))
            .WithLoadSimulations(
                Simulation.Inject(
                    rate: 300,
                    interval: TimeSpan.FromSeconds(1),
                    during: TimeSpan.FromSeconds(30)
                )
            );

            // Run the load test
            var result = NBomberRunner
                  .RegisterScenarios(scenario)
                  .Run();

            var stats = result.ScenarioStats
                .FirstOrDefault(s => s.ScenarioName == scenarioName);


            if (stats == null)
            {
                throw new Exception("Scenario not found");
            }
            var rate = (double)(stats.AllOkCount / stats.AllRequestCount);

            //Assert
            Assert.NotNull(stats);
            Assert.True(stats.AllOkCount > 0, "No successful requests were made during the load test.");
            Assert.True(rate > 0.8, "Less than 80% of requests were successful during the load test.");
        }
    }
}

