using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Newtonsoft.Json;
using TarokScoreBoard.Shared.DTO;
using TarokScoreBoard.Tests.IntegrationSetup;
using System.Text;
using Xunit.Abstractions;

namespace TarokScoreBoard.Tests
{
  [TestCaseOrderer("TarokScoreBoard.Tests.PriorityOrderer", "TarokScoreBoard.Tests")]
  public class ControllerTests : IClassFixture<BackendTestFixture>
  {
    private readonly BackendTestFixture fixture;
    private readonly ITestOutputHelper output;
    private Guid accessToken;
    private TeamDTO team;

    public ControllerTests(BackendTestFixture fixture, ITestOutputHelper output)
    {
      this.fixture = fixture;
      this.output = output;
    }

    [Fact, TestPriority(-10)]
    public async Task DeleteUnauthorized()
    {
      var response = await fixture.Client.DeleteAsync("/api/scoreboard/4ac472ed-5e78-4573-b24e-6506198f1f13");

      var unauthorized = response.StatusCode == System.Net.HttpStatusCode.Unauthorized;

      // Assert
      Assert.True(unauthorized);
    }

    [Fact, TestPriority(0)]
    public async Task GetGamesTest()
    {
      // Act
      var response = await fixture.Client.GetAsync("/api/game");
      response.EnsureSuccessStatusCode();

      var responseString = await response.Content.ReadAsStringAsync();
      output.WriteLine(responseString);
      var result = JsonConvert.DeserializeObject<ResponseDTO<GameDTO[]>>(responseString);

      // Assert
      Assert.True(result.Data.Length == 1);
      Assert.True(!String.IsNullOrEmpty(responseString));
    }

    [Fact, TestPriority(10)]
    public async Task Login()
    {
      var loginDto = new LoginDTO()
      {
        TeamId = "hribovci",
        Passphrase = "g00"
      };

      var content = new StringContent(JsonConvert.SerializeObject(loginDto), Encoding.UTF8, "application/json");

      var response = await fixture.Client.PostAsync("/api/team/login", content);
      response.EnsureSuccessStatusCode();

      var responseString = await response.Content.ReadAsStringAsync();
      output.WriteLine(responseString);
      var result = JsonConvert.DeserializeObject<ResponseDTO<LoginResponseDTO>>(responseString);
      this.accessToken = result.Data.AccessToken;
      this.team = result.Data.Team;
      fixture.Client.DefaultRequestHeaders.Add("access-token", accessToken.ToString());
      // Assert
      Assert.True(!String.IsNullOrEmpty(responseString));
    }

    [Fact, TestPriority(20)]
    public async Task DeleteAuthorized()
    {
      var response = await fixture.Client.DeleteAsync("/api/scoreboard/4ac472ed-5e78-4573-b24e-6506198f1f13");

      var deleted = response.StatusCode == System.Net.HttpStatusCode.NoContent;

      // Assert
      Assert.True(deleted);
    }
  }
}
