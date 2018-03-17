using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using TarokScoreBoard.Api;

namespace TarokScoreBoard.Tests
{
  public class ControllerTests
  {
    private readonly TestServer _server;
    private readonly HttpClient _client;
    public ControllerTests()
    {
      // Arrange
      _server = new TestServer(new WebHostBuilder()
          .UseStartup<Startup>());
      _client = _server.CreateClient();

      // Create a database in known state
      // Use modifed startup so it can execute setup logic (database connection, seed ...)
    }

    [Fact]
    public async Task GetGamesTest()
    {
      // Act
      var response = await _client.GetAsync("/api/Game");
      response.EnsureSuccessStatusCode();

      var responseString = await response.Content.ReadAsStringAsync();

      // Assert
      Assert.True(!String.IsNullOrEmpty(responseString));
    }
  }
}
