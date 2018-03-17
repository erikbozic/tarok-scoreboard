using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Net.Http;
using TarokScoreBoard.Api;

namespace TarokScoreBoard.Tests.IntegrationSetup
{
  public class BackendTestFixture : IDisposable
  {
    public TestServer Server { get; }
    public HttpClient Client { get; }

    public BackendTestFixture()
    {
      const string testingconfigFile = @"appsettings.Testing.json";
      string currentPath = Directory.GetCurrentDirectory();

      var testingConfig = new ConfigurationBuilder()
           .SetBasePath(currentPath)
           .AddJsonFile(testingconfigFile, optional: true)
           .Build();

      Server = new TestServer(new WebHostBuilder()
          .UseEnvironment("Testing")
          .UseConfiguration(testingConfig)
          .UseStartup<Startup>());
      Client = Server.CreateClient();
      
      var testDbAdapter = new TestDatabaseAdapter(testingConfig.GetConnectionString("tarok"));
      testDbAdapter.RecreateDatabase();
      testDbAdapter.SeedData();
      testDbAdapter.Dispose();
    }

    public void Dispose()
    {
      Server.Dispose();
    }
  }
}
