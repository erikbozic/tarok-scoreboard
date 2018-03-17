using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using TarokScoreBoard.Api;
using System.IO;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using TarokScoreBoard.Shared.DTO;
using TarokScoreBoard.Core.Entities;
using Xunit.Abstractions;
using TarokScoreBoard.Tests.IntegrationSetup;

namespace TarokScoreBoard.Tests
{
  public class ControllerTests
  {
    private readonly TestServer _server;
    private readonly HttpClient _client;
    private readonly ITestOutputHelper output;

    public ControllerTests(ITestOutputHelper output)
    {
      // Arrange
      this.output = output;

      const string testingconfigFile = @"appsettings.Testing.json";
      string currentPath = Directory.GetCurrentDirectory();

      var testingConfig = new ConfigurationBuilder()
           .SetBasePath(currentPath)
           .AddJsonFile(testingconfigFile, optional: true)
           .Build();

      _server = new TestServer(new WebHostBuilder()
          .UseEnvironment("Testing")
          .UseConfiguration(testingConfig)
          .UseStartup<Startup>());
      _client = _server.CreateClient();

      // Create a database in known state
      // Use modifed startup so it can execute setup logic (database connection, seed ...)

      var testDbAdapter = new TestDatabaseAdapter(testingConfig.GetConnectionString("tarok"), output);
      testDbAdapter.RecreateDatabase();
      testDbAdapter.SeedData();
      testDbAdapter.Dispose();
    }


    [Fact]
    public async Task GetGamesTest()
    {
      // Act
      var response = await _client.GetAsync("/api/Game");
      response.EnsureSuccessStatusCode();

      var responseString = await response.Content.ReadAsStringAsync();
      var result = JsonConvert.DeserializeObject<ResponseDTO<Game[]>>(responseString);

      // Assert
      Assert.True(result.Data.Length == 1);
      Assert.True(!String.IsNullOrEmpty(responseString));
    }
  }
}
